using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Networking;

public class J_VideoItem : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;



    //public videoInfo info;

    public ThumbnailInfo videoInfo;

    public Image downloadImage;

    //public VideoPlayer videoPlayer;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void SetItem(ThumbnailInfo thumbInfo)
    {
         videoInfo = thumbInfo;

        title.text = videoInfo.title;
        date.text = videoInfo.date;
        description.text = videoInfo.description;


        //Uri uri = new Uri(videoInfo.url);

        // ���� �ٿ�ε�
        HttpInfo httpInfo = new HttpInfo();
        httpInfo.Set(RequestType.GET, videoInfo.thumbnailUrl, (downloadHandler) =>
        {

            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + videoInfo.title + ".mp4", FileMode.Create);
            //byteData �� file �� ����
            file.Write(videoBytes, 0, videoBytes.Length);
            file.Close();


            //

            //RenderTexture rt = new RenderTexture(512, 256, 24);

            //videoPlayer.targetTexture = rt;
            //videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            //videoPlayer.url = Application.dataPath + "/" + videoInfo.title + ".mp4";
            //videoPlayer.Play();

            //// ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
            //Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");

        }, false);

        HttpManager.Get().SendRequest(httpInfo);
    }

    public void OnClickDownloadImage(ThumbnailInfo thumbInfo)
    {
        videoInfo = thumbInfo;

        title.text = videoInfo.title;
        date.text = videoInfo.date;
        description.text = videoInfo.description;

        HttpInfo info = new HttpInfo();
        info.Set(RequestType.TEXTURE, videoInfo.thumbnailUrl, null,false);
        info.onReceiveImage = OnCompleteDownloadTexture;
        HttpManager.Get().SendRequest(info);
    }
    void OnCompleteDownloadTexture(DownloadHandler downloadHandler, int id)
    {
        //�ٿ�ε�� �̹��� �����͸� sprite�� �����
        Texture2D texture = ((DownloadHandlerTexture)downloadHandler).texture;
        downloadImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    //public void SetItem(videoInfo videoInfo)
    //{
    //    info = videoInfo;

    //    title.text = info.title;
    //    date.text = info.date;
    //    description.text = info.description;


    //    Uri uri = new Uri(info.url);

    //    // ���� �ٿ�ε�
    //    HttpInfo httpInfo = new HttpInfo();
    //    httpInfo.Set(RequestType.GET, uri.AbsoluteUri, (downloadHandler) => {

    //        byte[] videoBytes = downloadHandler.data;

    //        FileStream file = new FileStream(Application.dataPath + "/" + info.title + ".mp4", FileMode.Create);
    //        //byteData �� file �� ����
    //        file.Write(videoBytes, 0, videoBytes.Length);
    //        file.Close();


    //        RenderTexture rt = new RenderTexture(1920, 1080, 24);
            
    //        videoPlayer.targetTexture = rt;
    //        videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
    //        videoPlayer.url = Application.dataPath + "/" + info.title + ".mp4";
    //        videoPlayer.Play();

    //        //// ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
    //        //Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");

    //    }, false);

    //    HttpManager.Get().SendRequest(httpInfo);
    //}
}
