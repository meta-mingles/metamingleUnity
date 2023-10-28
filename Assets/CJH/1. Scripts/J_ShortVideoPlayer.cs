using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class J_ShortVideoPlayer : MonoBehaviour
{

    public VideoPlayer videoPlayer;

    public VideolInfo videoInfo;

    public Action onClickEvent;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //���� ���� ���� 
    public void SetItem(VideolInfo Info)
    {
        videoInfo = Info;
        // ���� �ٿ�ε�
        HttpInfo httpInfo = new HttpInfo();
        httpInfo.Set(RequestType.GET, videoInfo.url, (downloadHandler) =>
        {

            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + videoInfo.title + ".mp4", FileMode.Create);
            //byteData �� file �� ����
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


    public void CloseVideo()
    {
        if (onClickEvent != null)
        {
            onClickEvent();
            Destroy(gameObject);
        }
    }
}
