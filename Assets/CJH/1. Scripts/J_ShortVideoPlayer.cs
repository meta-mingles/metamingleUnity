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

    public ThumbnailInfo thumbnailInfo;

    public Action onClickEvent;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //���� ���� ���� 
    public void SetItem(ThumbnailInfo videoInfo)
    {
        thumbnailInfo = videoInfo;
        // ���� �ٿ�ε�
        HttpInfo httpInfo = new HttpInfo();
        httpInfo.Set(RequestType.GET, thumbnailInfo.url, (downloadHandler) =>
        {

            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + thumbnailInfo.title + ".mp4", FileMode.Create);
            //byteData �� file �� ����
            file.Write(videoBytes, 0, videoBytes.Length);
            file.Close();


            RenderTexture rt = new RenderTexture(1920, 1080, 24);

            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            videoPlayer.url = Application.dataPath + "/" + thumbnailInfo.title + ".mp4";
            videoPlayer.Play();

            //// ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
            //Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");

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
