using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Networking;
using ExitGames.Client.Photon.StructWrapping;

public class J_VideoItem : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;

    public ThumbnailInfo thumbnailInfo;

    public Image downloadImage;

    public Action<ThumbnailInfo> onClickEvent;
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
            onClickEvent(thumbnailInfo);
        }
    }


    

    //썸네일 이미지 다운로드
    public void SetItem(ThumbnailInfo thumbInfo)
    {
        thumbnailInfo = thumbInfo;

        title.text = thumbnailInfo.title;
        date.text = thumbnailInfo.date;
        description.text = thumbnailInfo.description;

        HttpInfo info = new HttpInfo();
        info.Set(RequestType.TEXTURE, thumbnailInfo.thumbnailUrl, null,false);
        info.onReceiveImage = OnCompleteDownloadTexture;
        HttpManager.Get().SendRequest(info);
    }
    void OnCompleteDownloadTexture(DownloadHandler downloadHandler, int id)
    {
        //다운로드된 이미지 데이터를 sprite로 만든다
        Texture2D texture = ((DownloadHandlerTexture)downloadHandler).texture;
        downloadImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}
