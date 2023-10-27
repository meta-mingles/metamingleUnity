using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using TMPro;
using UnityEngine.UI;
using System;
using System.IO;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;


public class J_ShortVideoItem : MonoBehaviour
{
    public VideoInfo info;

    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetItem(VideoInfo videoInfo)
    {
        info = videoInfo;
        Uri uri = new Uri(info.url);

        // 영상 다운로드
        HttpInfo httpInfo = new HttpInfo();
        httpInfo.Set(RequestType.GET, info.url, (downloadHandler) => {

            byte[] videoBytes = downloadHandler.data;

            FileStream file = new FileStream(Application.dataPath + "/" + info.title + ".mp4", FileMode.Create);
            //byteData 를 file 에 쓰자
            file.Write(videoBytes, 0, videoBytes.Length);
            file.Close();


            RenderTexture rt = new RenderTexture(1920, 1080, 24);

            videoPlayer.targetTexture = rt;
            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;
            videoPlayer.url = Application.dataPath + "/" + info.title + ".mp4";
            videoPlayer.Play();

            //// 여기서 videoBytes를 사용하여 동영상을 처리하거나 저장합니다.
            //Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");

        }, false);

        HttpManager.Get().SendRequest(httpInfo);
    }
}
