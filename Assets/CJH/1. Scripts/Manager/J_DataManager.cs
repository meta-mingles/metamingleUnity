using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable] 
public class ShortVideoInfo //비디오 정보
{
    public int shortFormNo; //영상 순서

    public string title; //제목

    public string thumbnailUrl; //썸네일 url

    public string url; //url

    public string description; //영상 설명

    public string memberName; // 닉네임
    public string date; //날짜

    public bool isInteractive;//인터랙티브 여부

    public bool isLike;//  사용자 본인이 해당 숏폼에 좋아요를 했는지 여부

    public int shortFormLikeCnt; //해당 숏폼의 좋아요 수

    public List<InteractiveDTOS> interactiveMovieDTOS;
}

[System.Serializable] 
public class InteractiveDTOS
{
    public string url;
    public string choice;
    public int interactiveMovieNo;

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
    public List<ShortVideoInfo> shortVideoInfoList = new List<ShortVideoInfo>();


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
    //JSON
    public void SetShortVideoInfoListByJSON(string data)
    {
        JsonArray<ShortVideoInfo> arrayData = JsonUtility.FromJson<JsonArray<ShortVideoInfo>>(data);
        shortVideoInfoList = arrayData.data;
    }
    #endregion

}
