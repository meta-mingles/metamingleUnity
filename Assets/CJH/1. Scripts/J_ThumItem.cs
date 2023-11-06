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
    public static J_ThumItem Instance;

    private void Awake()
    {
        Instance = this;
    }
    [Header("Item Info")]
    public TMP_Text title;
    public TMP_Text date;
    public TMP_Text description;
    public TMP_Text memberName;

    public RawImage profileColor;

    public bool isInteractive;

    public ShortVideoInfo videoInfo;

    public Image downloadImage;

    public Action<ShortVideoInfo> onClickEvent;
    //public VideoPlayer videoPlayer;
    void Start()
    {
        profileColor.GetComponentInChildren<RawImage>().color = UnityEngine.Random.ColorHSV(0, 1);
        //처음 생성될 때 랜덤값으로 프로필컬러가 바뀌게 나온다.

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

        //날짜 
        string[] temp = videoInfo.date.Split('T');
        date.text = temp[0];

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
