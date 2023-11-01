using ExitGames.Client.Photon.StructWrapping;
using RootMotion.Demos;
using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_VideoReceiver : MonoBehaviour
{
    public GameObject thumbnailScrollView;

    public GameObject thumbnailFactory;
    public Transform trCtOfSV; //썸네일 스크롤뷰 생성장소


    public GameObject videoFactory;
    public Transform trCtOFVideoSC; //비디오 스크롤뷰 생성장소

    public TextMeshProUGUI nothingText;

    private void Update()
    {
        //tumbnail 리스트
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
            Debug.Log("영상이 나옵니다");
            HttpManager.Get().SendRequest(httpInfo);
        }

        #region 예시
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "ShopInfo.csv");

            // 서버한테 영상 정보 요청
            HttpInfo httpInfo = new HttpInfo();
            httpInfo.Set(RequestType.GET, uri.AbsoluteUri, (downloadHandler) => {

                //데이터 셋팅
                J_DataManager.instance.SetShopInfoList(downloadHandler.text);
            }, false);

            HttpManager.Get().SendRequest(httpInfo);
        }
        #endregion
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            VideolInfo testvideolInfo = new VideolInfo();
            CreateShortVideo(testvideolInfo);
        }
    }


    void OnCompleteSearchVideo(DownloadHandler downloadHandler)
    {
        //데이터 셋팅
        //J_DataManager.instance.SetRealVideoInfoListByCSV(downloadHandler.text);
        J_DataManager.instance.SetRealVideoInfoListByJSON(downloadHandler.text);

        //UI 만들자
        for (int i = 0; i < J_DataManager.instance.videoInfoList.Count; i++)
        {
            // 섬네일(비디오 썸네일)을 만듭니다.
            GameObject video = Instantiate(thumbnailFactory, trCtOfSV);
            J_ThumItem item = video.GetComponent<J_ThumItem>();

            // 항목 설정 - 각 섬네일에 대한 정보를 설정합니다.
            item.SetItem(J_DataManager.instance.videoInfoList[i]);
            //썸네일 클릭 시, 숏폼(짧은 형식의) 영상 창을 만듭니다.
            item.onClickEvent = CreateShortVideo;
        }
    }

    //숏폼비디오 열리는 함수
    void CreateShortVideo(VideolInfo info)
    {
        GameObject video = Instantiate(videoFactory, trCtOFVideoSC);
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

