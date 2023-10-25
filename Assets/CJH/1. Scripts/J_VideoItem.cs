using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;

public class J_VideoItem : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;

    public VideoInfo info;

    public VideoPlayer videoPlayer;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void SetItem(VideoInfo videoInfo)
    {
        info = videoInfo;

        title.text = info.Title;
        date.text = info.date;
        description.text = info.description;


        Uri uri = new Uri(info.url);

        // ���� �ٿ�ε�
        HttpInfo httpInfo = new HttpInfo();
        httpInfo.Set(RequestType.GET, uri.AbsoluteUri, (downloadHandler) => {

            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + info.Title + ".mp4", FileMode.Create);
            //byteData �� file �� ����
            file.Write(videoBytes, 0, videoBytes.Length);
            file.Close();


            RenderTexture rt = new RenderTexture(512, 256, 24);
            
            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            videoPlayer.url = Application.dataPath + "/" + info.Title + ".mp4";
            videoPlayer.Play();

            //// ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
            //Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");

        }, false);

        HttpManager.Get().SendRequest(httpInfo);
    }
}
