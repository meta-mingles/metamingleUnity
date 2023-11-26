using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_ShortVideoPlayer : J_VideoPlayerBase
{
    [Header("ShortVideo info")]
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;
    public TMP_Text membername;
    public TMP_Text like_Count;
    public RawImage profilecolor;
    public Button like_Button; // 좋아요 버튼
    public float colorChangeDuration = 0.5f;//색상변경시간
    private Color[] rainbowColor = new Color[] { Color.green, Color.magenta, Color.yellow, Color.blue, Color.cyan, Color.red }; //무지개 색상 변화
    public J_InteractiveMovieItem interactiveMovieList;
    public Action onColorChangeComplete;
    private void Awake()
    {
        if (like_Button != null)
        {
            like_Button.onClick.AddListener(LikeButton);
        }
        like_Button.onClick.AddListener(() => StartCoroutine(ChangeToRainbowAndThenBlack()));
    }
    //숏폼 비디오 서버 
    public override void SetItem(ShortVideoInfo info)
    {
        base.SetItem(info);
        //제목
        title.text = videoInfo.title;
        //날짜 
        string[] temp = videoInfo.date.Split('T');
        date.text  = "DATE : " + temp[0];
        //영상 설명
        description.text = "DESCRIPTION : " + videoInfo.description;
        //크리에이터
        membername.text = videoInfo.memberName;
        //좋아요 수
        like_Count.text = videoInfo.shortFormLikeCnt.ToString();
    }
    //영상이 끝날때 인터렉티브 UI 생성
    protected override void MakeInteractiveUI(VideoPlayer source)
    {
        if (videoInfo.isInteractive)
        {
            interactiveMovieList.SetInteractiveInfo(ClickInteractiveMovieBt, videoInfo.interactiveMovieDTOS[0].choice, videoInfo.interactiveMovieDTOS[1].choice);
        }
        //interactiveMovieList.gameObject.SetActive(true);
        //interactiveMovieList.onClickInteractive = ClickInteractiveMovieBt;
        //interactiveMovieList.SetInteractiveItem(videoInfo.interactiveMovieDTOS[0].choice, videoInfo.interactiveMovieDTOS[1].choice);
    }
    //인터렉티브 버튼
    void ClickInteractiveMovieBt(int index)
    {
        ShortVideoInfo info = new ShortVideoInfo();

        info.isInteractive = true;
        info.title = videoInfo.interactiveMovieDTOS[index].choice;
        info.url = videoInfo.interactiveMovieDTOS[index].url;

        J_VideoReceiver.instance.CreateInteractiveMovie(info);
    }
    void LikeButton()
    {
        int likeCnt = videoInfo.shortFormLikeCnt;
        //좋아요 안 한 영상이라면
        if (!videoInfo.isLike)
        {
            // HttpInfo 인스턴스 생성 및 설정
            HttpInfo httpInfo = new HttpInfo();
            string url = "/short-form/" + videoInfo.shortFormNo + "/like"; // 숏폼 번호를 URL에 포함
            httpInfo.Set(RequestType.POST, url, (DownloadHandler downloadHandler) =>
            {
                JObject jObject = JObject.Parse(downloadHandler.text);
                ShortVideoInfo data = jObject["data"].ToObject<ShortVideoInfo>();
                bool isLike = jObject["data"].ToObject<bool>();
                int like_Count = jObject["data"].ToObject<int>();

            });
            // HttpManager를 통해 요청 보내기
            HttpManager.instance.SendRequest(httpInfo);
        }
        else //좋아요 한 영상이라면
        {

        }

    }

    //무지개색상변경
    IEnumerator ChangeToRainbowAndThenBlack()
    {
        Image likeBtImage = like_Button.GetComponent<Image>();
        //무지개 색상 변경
        foreach(Color color in rainbowColor)
        {
            yield return StartCoroutine(ChangeColor(likeBtImage,color, colorChangeDuration));
        }
        //최종 색깔 검정색으로 바뀐다.
        yield return like_Button.GetComponent<Image>().color = Color.black;
    }
    //색상변경
    IEnumerator ChangeColor(Image image, Color newColor, float duration)
    {
        float elapsedTime = 0; //경과 시간
        Color orignColor = like_Button.image.color;
        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(orignColor,newColor,elapsedTime/duration);
            yield return null;
        }
        image.color = newColor;

        // 색상 변경이 완료되면 콜백 함수 호출
        onColorChangeComplete?.Invoke();

    }
}
