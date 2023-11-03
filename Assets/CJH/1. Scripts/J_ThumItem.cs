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

    public bool isInteractive;

    public ShortVideoInfo videoInfo;

    public Image downloadImage;

    public Action<ShortVideoInfo> onClickEvent;
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

    //비디오 데이터 셋팅
    public void SetItem(ShortVideoInfo Info)
    {
        videoInfo = Info;

        //제목
        title.text = videoInfo.title;
        //인터렉티브 여부
        isInteractive = videoInfo.isInteractive;

        //날짜 빼기
        string mydate= videoInfo.date.Substring(0, 10);
        date.text = mydate;
        //영상 설명
        description.text = videoInfo.description;
        //크리에이터
        memberName.text = videoInfo.memberName;

        HttpInfo info = new HttpInfo();
        info.Set(RequestType.TEXTURE, videoInfo.thumbnailUrl, null,false);
        info.onReceiveImage = OnCompleteDownloadTexture;
        HttpManager.Get().SendRequest(info);
    }
    //썸네일 이미지 다운로드
    void OnCompleteDownloadTexture(DownloadHandler downloadHandler, int id)
    {
        //다운로드된 이미지 데이터를 sprite로 만든다
        Texture2D texture = ((DownloadHandlerTexture)downloadHandler).texture;
        downloadImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

}
