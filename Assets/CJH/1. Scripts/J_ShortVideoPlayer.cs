using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class J_ShortVideoPlayer : J_VideoPlayerBase
{
    [Header("ShortVideo info")]
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;
    public TMP_Text membername;
    public RawImage profilecolor;

    public J_InteractiveMovieItem interactiveMovieList;

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
       
    }



    //영상이 끝날때 인터렉티브 UI 생성
    protected override void OnFinishVideo(VideoPlayer source)
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
}
