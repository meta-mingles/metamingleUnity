﻿using UnityEngine;
using UnityEngine.Networking;

public class J_VideoReceiver : MonoBehaviour
{
    public static J_VideoReceiver instance;

    [Header("ThumNailList")]
    public GameObject thumbnailScrollView;

    public GameObject thumbnailFactory;
    public Transform trCtOfSV; //썸네일 스크롤뷰 생성장소

    [Header("ShortVideo")]
    public GameObject videoFactory;
    public GameObject interactiveVideoFactory;
    public Transform trCtOFVideoSV; //비디오 스크롤뷰 생성장소
    public GameObject videoInfo; //비디오 정보
    //public GameObject errorVideoFactory; //조회실패 팝업창
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    private void Update()
    {
        //씬이 넘어올 때 
        //tumbnail 리스트

        //씬이 처음 로드될때 
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // 서버한테 영상 정보 요청
            HttpInfo httpInfo = new HttpInfo();

            //Debug.Log(httpInfo.url);
            //Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "RealVideoInfo.csv");

            string url = "/short-form";
            //CSV
            //httpInfo.Set(RequestType.GET, uri.AbsoluteUri, OnCompleteSearchVideo, false);
            httpInfo.Set(RequestType.GET, url, OnCompleteSearchVideo, true);

            HttpManager.Get().SendRequest(httpInfo);


        }
    }
    //숏폼 다운
    void OnCompleteSearchVideo(DownloadHandler downloadHandler)
    {
        //데이터 셋팅
        //J_DataManager.instance.SetShortVideoInfoListByCSV(downloadHandler.text);
        J_DataManager.instance.SetShortVideoInfoListByJSON(downloadHandler.text);

        //UI 만들자
        for (int i = 0; i < J_DataManager.instance.shortVideoInfoList.Count; i++)
        {
            // 섬네일(비디오 썸네일)을 만듭니다.
            GameObject video = Instantiate(thumbnailFactory, trCtOfSV);
            J_ThumItem item = video.GetComponent<J_ThumItem>();

            // 항목 설정 - 각 섬네일에 대한 정보를 설정합니다.
            item.SetItem(J_DataManager.instance.shortVideoInfoList[i]);

            //썸네일 클릭 시, 숏폼(짧은 형식의) 영상 창을 만듭니다.
            item.onClickEvent = CreateShortVideo;

        }
    }

    //인터렉티브 비디오 열리는 함수
    public void CreateInteractiveMovie(ShortVideoInfo info)
    {
        //인터렉티브 비디오 생성
        GameObject video = Instantiate(interactiveVideoFactory, trCtOFVideoSV);
        J_InteractiveVideoPlayer item = video.GetComponent<J_InteractiveVideoPlayer>();
        item.SetItem(info);
    }
    //숏폼비디오 열리는 함수
    public void CreateShortVideo(ShortVideoInfo info)
    {
        //숏비디오 생성
        GameObject video = Instantiate(videoFactory, trCtOFVideoSV);
        //
        J_ShortVideoPlayer item = video.GetComponent<J_ShortVideoPlayer>();
        item.SetItem(info);

        thumbnailScrollView.SetActive(false);
        item.onClickEvent = CloseShortVideo;
    }

    //숏폼비디오 닫는 함수
    void CloseShortVideo()
    {
        thumbnailScrollView.SetActive(true);
    }
}

