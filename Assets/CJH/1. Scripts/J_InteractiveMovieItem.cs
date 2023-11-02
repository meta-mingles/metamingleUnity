using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;

public class J_InteractiveMovieItem : MonoBehaviour
{
    public TMP_Text choice;

    public VideoPlayer videoPlayer;

    public InteractiveMovieInfo interactiveMovieInfo;

    //public ShortVideolInfo shortVideolInfo;

    public InteractiveDTOS interactiveData;

    public Action<ShortVideolInfo> onClickEvent;

    void Start()
    {
    }

    void Update()
    {

    }


    //public void OnClick()
    //{
    //    if (onClickEvent != null)
    //    {
    //        onClickEvent(interactiveMovieInfo);
    //    }
    //}

    //인터렉티브 무비 데이터 셋팅
    public void SetItem(InteractiveMovieInfo Info)
    {
        interactiveMovieInfo = Info;

        HttpInfo httpInfo = new HttpInfo();

        httpInfo.Set(RequestType.GET, interactiveData.url, (downloadHandler) =>
        {

            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + interactiveMovieInfo.title + ".mp4", FileMode.Create);
            //byteData 를 file 에 쓰자
            file.Write(videoBytes, 0, videoBytes.Length);
            file.Close();

            RenderTexture rt = new RenderTexture(1920, 1080, 24);

            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            videoPlayer.url = Application.dataPath + "/" + interactiveMovieInfo.title + ".mp4";
            videoPlayer.Play();

        }, false);

        HttpManager.Get().SendRequest(httpInfo);

    }



    //인터랙티브 무비 
    public void CreateInteractiveMovieList()
    {
        ////영상이 다 끝나갈때 객체 활성화
        //if (!videoPlayer.isPlaying)
        //{
        //    isInteractive = true;
        //    interactiveMovieList.SetActive(true);
        //}

    }
}
