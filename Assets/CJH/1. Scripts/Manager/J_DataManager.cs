using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] 
public class InteractiveMovieInfo //비디오 정보
{
    public long interactiveMovieNo; //인터랙티브 무비 번호

    public string title; //영상 제목

    public string thumbnailUrl; //썸네일 url

    public string description; //영상 설명

    public string memberName; // 영상 제작자 이름

    public DateTime date; //영상 제작 날짜

    public int sequence; // 영상 순서 (연결되어있는 3개의 영상 중 순서)
}

[System.Serializable] 
public class ShortVideolInfo //비디오 정보
{
    public int shortFormNo; //영상 순서

    public string title; //제목

    public string thumbnailUrl; //썸네일 url

    public string url; //url

    public string description; //영상 설명

    public string memberName; // 닉네임

    public string date; //날짜

    public bool isInteractive;//인터랙티브 여부

    public InteractiveDTOS interactiveData;
}

[System.Serializable] 
public class InteractiveDTOS
{
    public string url;
    public string choice;
    public int interactiveMovieNo;

}
[System.Serializable] 
public class ShortVideoInfoContainer
{
    public List<ShortVideolInfo> data;
}
[System.Serializable]
public class InteractiveDTOSContainer
{
    public List<InteractiveDTOS> data;
}


[System.Serializable]
public struct JsonArray<T>
{
    public List<T> data;
}

public class J_DataManager : MonoBehaviour
{
    public static J_DataManager instance;

    //숏비디오 정보
    public List<ShortVideolInfo> shortVideoInfoList = new List<ShortVideolInfo>();

    //interactiveDTOS 리스트
    //public List<InteractiveDTOS> interactiveList = new List<InteractiveDTOS>();


    //인터렉티브 비디오 정보
    public List<InteractiveMovieInfo> interactiveMovieInfoList = new List<InteractiveMovieInfo>();


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

    #region 잘되는거 잠깐 주석처리
    //CSV
    //public void SetShortVideoInfoListByCSV(string data)
    //{
    //    shortVideoInfoList = J_CSVLoader.instance.ParseString<ShortVideolInfo>(data);
    //}
    ////JSON
    //public void SetShortVideoInfoListByJSON(string data)
    //{
    //    JsonArray<ShortVideolInfo> arrayData = JsonUtility.FromJson<JsonArray<ShortVideolInfo>>(data);
    //    shortVideoInfoList = arrayData.data;
    //}

    ////인터렉티브 무비일경우 리스트추가
    //public void SetInteractiveListByJSON(string data)
    //{
    //    JsonArray<InteractiveDTOS> arrayData = JsonUtility.FromJson<JsonArray<InteractiveDTOS>>(data);
    //    interactiveList = arrayData.data;
    //}
    #endregion

    public void SetShortVideoInfoListByJSON(string data)
    {
        ShortVideoInfoContainer videoContainer = JsonUtility.FromJson<ShortVideoInfoContainer>(data);
        shortVideoInfoList = videoContainer.data;
    }
    //public void SetInteractiveListByJSON(string data)
    //{
    //    InteractiveDTOSContainer interactiveContainer = JsonUtility.FromJson<InteractiveDTOSContainer>(data);
    //    interactiveList = interactiveContainer.data;
    //}


    //인터렉티브 무비 다운
    public void SetInteractiveMovieInfoListByJSON(string data)
    {
        JsonArray<InteractiveMovieInfo> arrayData = JsonUtility.FromJson<JsonArray<InteractiveMovieInfo>>(data);
        interactiveMovieInfoList = arrayData.data;
    }

}
