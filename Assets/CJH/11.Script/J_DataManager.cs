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
public class VideoInfo //비디오 정보
{
    //제목
    public string Title;

    //날짜
    public string date;

    //내용
    public string description;

    //크리에이터
    public string Creator;

    public string url;
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

    public void SetVideoInfoList(string data)
    {
        videoInfoList = J_CSVLoader.instance.ParseString<VideoInfo>(data);
    }

    public void SetShopInfoList(string data)
    {
        shopInfoList = J_CSVLoader.instance.ParseString<ShopInfo>(data);
    }
}
