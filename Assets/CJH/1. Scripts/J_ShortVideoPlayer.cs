﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_ShortVideoPlayer : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    public VideolInfo videoInfo;

    public Action onClickEvent;

    public Action onPlayEvent;

    public Button likeBt;

    public Text likeCountText;

    private int likeCount = 0;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //숏폼 비디오 서버 
    public void SetItem(VideolInfo Info)
    {
        videoInfo = Info;
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

        }, false);

        HttpManager.Get().SendRequest(httpInfo);
    }
    //좋아요 
    public void LikeVideo()
    {
        //버튼을 누를때마다 텍스트 갯수가 증가한다.

        //서버에다가 좋아요 

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
            Destroy(gameObject);
        }
    }
}
