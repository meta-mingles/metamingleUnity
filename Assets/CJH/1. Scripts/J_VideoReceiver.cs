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
    public Transform trCtOfSV; //����� ��ũ�Ѻ� �������


    public GameObject videoFactory;
    public Transform trCtOFVideoSC; //���� ��ũ�Ѻ� �������

    public TextMeshProUGUI nothingText;

    private void Update()
    {
        //tumbnail ����Ʈ
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // �������� ���� ���� ��û
            HttpInfo httpInfo = new HttpInfo();

            //Debug.Log(httpInfo.url);
            //Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "RealVideoInfo.csv");

            string url = "/short-form";
            //CSV
            //httpInfo.Set(RequestType.GET, uri.AbsoluteUri, OnCompleteSearchVideo, false);
            httpInfo.Set(RequestType.GET, url, OnCompleteSearchVideo, true);
            Debug.Log("������ ���ɴϴ�");
            HttpManager.Get().SendRequest(httpInfo);
        }

        #region ����
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {

            Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "ShopInfo.csv");

            // �������� ���� ���� ��û
            HttpInfo httpInfo = new HttpInfo();
            httpInfo.Set(RequestType.GET, uri.AbsoluteUri, (downloadHandler) => {

                //������ ����
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
        //������ ����
        //J_DataManager.instance.SetRealVideoInfoListByCSV(downloadHandler.text);
        J_DataManager.instance.SetRealVideoInfoListByJSON(downloadHandler.text);

        //UI ������
        for (int i = 0; i < J_DataManager.instance.videoInfoList.Count; i++)
        {
            // ������(���� �����)�� ����ϴ�.
            GameObject video = Instantiate(thumbnailFactory, trCtOfSV);
            J_ThumItem item = video.GetComponent<J_ThumItem>();

            // �׸� ���� - �� �����Ͽ� ���� ������ �����մϴ�.
            item.SetItem(J_DataManager.instance.videoInfoList[i]);
            //����� Ŭ�� ��, ����(ª�� ������) ���� â�� ����ϴ�.
            item.onClickEvent = CreateShortVideo;
        }
    }

    //�������� ������ �Լ�
    void CreateShortVideo(VideolInfo info)
    {
        GameObject video = Instantiate(videoFactory, trCtOFVideoSC);
        J_ShortVideoPlayer item = video.GetComponent<J_ShortVideoPlayer>();
        item.SetItem(info);

        thumbnailScrollView.SetActive(false);

        item.onClickEvent = CloseShortVideo;
    }

    //�������� �ݴ� �Լ�
    void CloseShortVideo()
    {
        thumbnailScrollView.SetActive(true);
    }
}

