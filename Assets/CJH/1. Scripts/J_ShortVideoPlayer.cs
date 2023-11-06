using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_ShortVideoPlayer : MonoBehaviour
{
    [Header("item info")]
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;
    public TMP_Text membername;
    public RawImage profilecolor;

    public VideoPlayer videoPlayer;

    public ShortVideoInfo videoInfo;

    public Action onClickEvent;

    public GameObject interactiveMovieListFactory;

    public TMP_Text choiceText;
    void Start()
    {
        //profilecolor.GetComponentInChildren<RawImage>().color = UnityEngine.Random.ColorHSV(0, 1);
        profilecolor.GetComponentInChildren<RawImage>().color = J_ThumItem.Instance.profileColor.color;
        ////처음 생성될 때 랜덤값으로 프로필컬러가 바뀌게 나온다.
    }

    void Update()
    {

    }

    //숏폼 비디오 서버 
    public void SetItem(ShortVideoInfo Info)
    {
        videoInfo = Info;
        //제목
        title.text = videoInfo.title;

        //날짜 빼기
        string mydate = videoInfo.date.Substring(0, 10);
        date.text = mydate;
        //영상 설명
        description.text = videoInfo.description;
        //크리에이터
        membername.text = videoInfo.memberName;

        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();
        
        httpInfo.Set(RequestType.GET, videoInfo.url, (downloadHandler) =>
        {
            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + videoInfo.title + ".mp4", FileMode.Create);
            //byteData 를 file 에 쓰자
            file.Write(videoBytes, 0, videoBytes.Length);
            file.Close();

            RenderTexture rt = new RenderTexture(1920, 1080, 24);

            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            videoPlayer.url = Application.dataPath + "/" + videoInfo.title + ".mp4";
            videoPlayer.Play();
            //videoPlayer.loopPointReached += OnFinishVideo;

            videoPlayer.loopPointReached += OnFinishVideo;


        }, false);

        HttpManager.Get().SendRequest(httpInfo);
    }

    //영상이 끝날때 인터렉티브 UI 생성
    void OnFinishVideo(VideoPlayer source)
    {
        if(videoInfo.interactiveMovieDTOS != null && videoInfo.interactiveMovieDTOS.Count > 0)
        {
            GameObject interactiveMovieList = Instantiate(interactiveMovieListFactory,transform.parent);
            J_InteractiveMovieItem item  = interactiveMovieList.GetComponent<J_InteractiveMovieItem>();
            item.onClickInteractive = ClickInteractiveMovieBt;
        }
    }
    //인터렉티브 버튼
    void ClickInteractiveMovieBt(int index)
    {
        ShortVideoInfo info = new ShortVideoInfo();

        info.title = videoInfo.interactiveMovieDTOS[index].choice;

        info.url = videoInfo.interactiveMovieDTOS[index].url;


        J_VideoReceiver.instance.CreateInteractiveMovie(info);
    }

    //영상 재생
    public void PlayPauseVideo()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
    }
    //영상 끄기
    public void CloseVideo()
    {
        if (onClickEvent != null)
        {
            onClickEvent();
        }
         Destroy(gameObject);
    }



}
