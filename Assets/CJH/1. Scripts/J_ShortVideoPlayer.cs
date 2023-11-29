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
    public Button Sound_Button; //사운드 버튼
    public float colorChangeDuration = 0.5f;//색상변경시간
    int likeCnt;
    private Color[] rainbowColor = new Color[] { Color.green, Color.magenta, Color.yellow, Color.blue, Color.cyan, Color.red }; //무지개 색상 변화
    public J_InteractiveMovieItem interactiveMovieList;
    private void Start()
    {
        like_Button.onClick.AddListener(() => StartCoroutine(ChangeToRainbowAndThenBlack()));
        //처음엔 0 
        like_Count.text = "0";
        //but 누르고나서 서버에서 받는 좋아요수로 업데이트
        if (like_Button != null)
        {
            like_Button.onClick.AddListener(LikeButton);
        }
    }
    //숏폼 비디오 서버 
    public override void SetItem(ShortVideoInfo info)
    {
        base.SetItem(info);

        //제목
        title.text = videoInfo.title;
        //날짜 
        string[] temp = videoInfo.date.Split('T');
        date.text = "DATE : " + temp[0];

        //영상 설명
        description.text = videoInfo.description;
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
        likeCnt = videoInfo.shortFormLikeCnt;
        likeCnt += 1;
        like_Count.text = likeCnt.ToString();

        //계정당 한번
        if (videoInfo.isLike == true) //숏폼을 좋아요 했다면
        {
            return;
        }
        else
        {

        }
        //서버랑 연동
    }

    //무지개색상변경
    IEnumerator ChangeToRainbowAndThenBlack()
    {
        Image likeBtImage = like_Button.GetComponent<Image>();
        //무지개 색상 변경
        foreach (Color color in rainbowColor)
        {
            yield return StartCoroutine(ChangeColor(likeBtImage, color, colorChangeDuration));
        }
        //최종 색깔 검정색으로 바뀐다.
        yield return like_Button.GetComponent<Image>().color = Color.black;
    }
    //색상변경
    IEnumerator ChangeColor(Image image, Color newColor, float duration)
    {
        float elapsedTime = 0; //경과 시간
        Color orignColor = like_Button.image.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(orignColor, newColor, elapsedTime / duration);
            yield return null;
        }
        image.color = newColor;
    }
}
