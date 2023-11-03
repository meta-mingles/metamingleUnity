using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_ShortVideoPlayer : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    public ShortVideoInfo videoInfo;

    public ShortVideoInfoContainer interactiveInfo;

    public Action onClickEvent;

    public Action onPlayEvent;

    public GameObject interactiveMovieList;

    //public bool isInteractive;


    void Start()
    {
        //interactiveMovieList.SetActive(false);
    }

    void Update()
    {


    }

    //숏폼 비디오 서버 
    public void SetItem(ShortVideoInfo Info)
    {
        videoInfo = Info;
        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();
        if (videoInfo.isInteractive)
        {

        }

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

    void SetInteractiveMovieItem(ShortVideoInfoContainer Info)
    {
        interactiveInfo = Info;
        //영상 다운
        HttpInfo httpInfo = new HttpInfo();
       foreach (ShortVideoInfo videoInfo in interactiveInfo.data)
        {
            //인터렉티브 무비라면
            if (videoInfo.isInteractive)
            {
                for(int i = 0; i < videoInfo.interactiveMovieDTOS.Count; i++)
                {
                    string url = videoInfo.interactiveMovieDTOS[i].url;
                    string choice = videoInfo.interactiveMovieDTOS[i].choice;
                    int number = videoInfo.interactiveMovieDTOS[i].interactiveMovieNo;

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
            }
        }
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
