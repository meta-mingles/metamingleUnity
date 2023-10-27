using ExitGames.Client.Photon.StructWrapping;
using RootMotion.Demos;
using System;
using System.Collections;
using System.IO;
using System.Text;
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


    private void Update()
    {

        //tumbnail ����Ʈ
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // �������� ���� ���� ��û
            HttpInfo httpInfo = new HttpInfo();

            Debug.Log(httpInfo.url);
            Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "RealVideoInfo.csv");

            string url = "short-form";
            httpInfo.Set(RequestType.GET, url, (downloadHandler) =>
            {
                //������ ����
                //J_DataManager.instance.SetRealVideoInfoListByCSV(downloadHandler.text);
                J_DataManager.instance.SetRealVideoInfoListByJSON(downloadHandler.text);

                //UI ������
                for (int i = 0; i < J_DataManager.instance.thumbnailInfoList.Count; i++)
                {
                    GameObject video = Instantiate(thumbnailFactory, trCtOfSV);
                    J_VideoItem item = video.GetComponent<J_VideoItem>();
                    //item.SetItem(J_DataManager.instance.thumbnailInfoList[i]);
                    item.SetItem(J_DataManager.instance.thumbnailInfoList[i]);
                    item.onClickEvent = CreateShortVideo;
                }

            }, true);

            HttpManager.Get().SendRequest(httpInfo);

        
        }
       

        if(Input.GetKeyDown(KeyCode.Alpha2))
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
    }

    void CreateShortVideo(ThumbnailInfo info)
    {
        GameObject video = Instantiate(videoFactory, trCtOFVideoSC);
        J_ShortVideoPlayer item = video.GetComponent<J_ShortVideoPlayer>();
        item.SetItem(info);

        thumbnailScrollView.SetActive(false);

        item.onClickEvent = CloseShortVideo;
    }

    void CloseShortVideo()
    {
        thumbnailScrollView.SetActive(true);
    }



    //int videoNum;
    //IEnumerator Test(string u, string saveName)
    //{
    //    Uri uri = new Uri(u);
    //    //������ ��Ź��� �� ������ �ּ�ó���ϱ�


    //    using (UnityWebRequest www = UnityWebRequest.Get(uri))
    //    {
    //        yield return www.SendWebRequest();

    //        if (www.result != UnityWebRequest.Result.Success)
    //        {
    //            Debug.Log(www.error);
    //        }
    //        else
    //        {                
    //            byte[] videoBytes = www.downloadHandler.data;

    //            FileStream file = new FileStream(Application.dataPath + "/" + saveName, FileMode.Create);                
    //            //byteData �� file �� ����
    //            file.Write(videoBytes, 0, videoBytes.Length);
    //            file.Close();

    //            //���� ���� �Ϸ�
    //            //GameObject Video = new GameObject("Video" + videoNum); //���� ������ ������������ �ִ´�
    //            //GameObject Video = Instantiate(videoFactory, trCtOfSV);
    //            //Video.transform.SetParent(trCtOfSV.transform); //���� ������ ��ġ content ����

    //            videoList = Instantiate(videoFactory, trCtOfSV);

    //            J_VideoItem videoItem = videoList.GetComponent<J_VideoItem>();
    //            //videoItem.SetItem()


    //            //GameObject go = Instantiate(videoFactory, trCtOfSV);
    //            //go.transform.SetParent(Video.transform); //go�� ��ġ video ����
    //            VideoPlayer videoPlayer = videoList.GetComponentInChildren<VideoPlayer>();


    //            RenderTexture rt = new RenderTexture(512, 256, 24);
    //            videoPlayer.targetTexture = rt;
    //            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;


    //            videoPlayer.url = Application.dataPath + "/" + saveName;
    //            videoPlayer.Play();

    //            // ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
    //            Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");
    //        }
    //    }
    //}
}

