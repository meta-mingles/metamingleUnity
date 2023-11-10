using System;
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

    int likeCnt;
    public J_InteractiveMovieItem interactiveMovieList;



    private void Start()
    {
        //처음엔 0 
        like_Count.text = "0";
        //but 누르고나서 서버에서 받는 좋아요수로 업데이트

        if(like_Button != null)
        {
            like_Button.onClick.AddListener(Like_Button);
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
        date.text = temp[0];

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
        interactiveMovieList.SetInteractiveInfo(ClickInteractiveMovieBt, videoInfo.interactiveMovieDTOS[0].choice, videoInfo.interactiveMovieDTOS[1].choice);
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
    void Like_Button()
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
}
