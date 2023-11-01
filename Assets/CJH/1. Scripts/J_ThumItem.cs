using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Networking;

public class J_ThumItem : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;
    public TMP_Text memberName;

    public VideolInfo videoInfo;

    public Image downloadImage;

    public Action<VideolInfo> onClickEvent;
    //public VideoPlayer videoPlayer;
    void Start()
    {
    }

    void Update()
    {
        
    }
 

    public void OnClick()
    {
        if(onClickEvent != null)
        {
            onClickEvent(videoInfo);
        }
    }

    
    

    //����� �̹��� �ٿ�ε�
    public void SetItem(VideolInfo Info)
    {
        videoInfo = Info;

        title.text = videoInfo.title;


        //��¥�Ľ�
        string mydate= videoInfo.date.Substring(0, 10);
        date.text = mydate;
        description.text = videoInfo.description;
        memberName.text = videoInfo.memberName;

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

}
