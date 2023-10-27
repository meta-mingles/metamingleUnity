using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class ShopInfo
{
    public string name;
    public int price;
    public bool sale;
    public string thumbnail;
}


[System.Serializable] // 직렬화
public struct UserInfo //나의 정보
{
    //id
    public string name;

    //크리에이터
    public string Creator;
}


[System.Serializable] // 직렬화
public class VideoInfo //숏폼 비디오 정보
{
    public int shortFormNo; //영상 순서

    public string title; //제목

    public string thumbnailUrl; //썸네일 url

    public string url; //url

    public string description; //영상 설명

    public string memberName; // 닉네임

    public string date; //날짜

    public bool isInteractive; //인터랙티브 여부
}


[System.Serializable] // 직렬화
public class ThumbnailInfo //비디오 정보
{

    public int shortFormNo; //영상 순서

    public string title; //제목

    public string thumbnailUrl; //썸네일 url

    public string url; //url

    public string description; //영상 설명

    public string memberName; // 닉네임

    public string date; //날짜

    public bool isInteractive; //인터랙티브 여부
}




[System.Serializable]
public struct JsonArray<T>
{
    public List<T> data;
}

public class J_DataManager : MonoBehaviour
{
    public static J_DataManager instance;

    //비디오 정보
    public List<VideoInfo> videoInfoList = new List<VideoInfo>();
    //실제 비디오 정보
    public List<ThumbnailInfo> thumbnailInfoList = new List<ThumbnailInfo>();

    //샵 정보
    public List<ShopInfo> shopInfoList = new List<ShopInfo>();

    private void Awake()
    {
        instance = this;

    }

    void Start()
    {
    }

    void Update()
    {
        
    }
    public void SetRealVideoInfoList(string data)
    {
        thumbnailInfoList = J_CSVLoader.instance.ParseString<ThumbnailInfo>(data);
    }


    public void SetVideoInfoList(string data)
    {
        videoInfoList = J_CSVLoader.instance.ParseString<VideoInfo>(data);
    }

    public void SetShopInfoList(string data)
    {
        shopInfoList = J_CSVLoader.instance.ParseString<ShopInfo>(data);
    }
}
