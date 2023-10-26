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
    public GameObject videoFactory;
    public Transform trCtOfSV;

    private void Update()
    {
        //리얼 서버
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // 서버한테 영상 정보 요청
            HttpInfo httpInfo = new HttpInfo();

            Debug.Log(httpInfo.url);
            Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "RealVideoInfo.csv");

          
            httpInfo.Set(RequestType.GET, uri.AbsoluteUri, (downloadHandler) =>
            {

                //데이터 셋팅
                J_DataManager.instance.SetRealVideoInfoList(downloadHandler.text);

                //UI 만들자
                for (int i = 0; i < J_DataManager.instance.realVideoInfoList.Count; i++)
                {
                    GameObject video = Instantiate(videoFactory, trCtOfSV);
                    J_VideoItem item = video.GetComponent<J_VideoItem>();
                    item.SetItem(J_DataManager.instance.realVideoInfoList[i]);
                }

            }, false);

            HttpManager.Get().SendRequest(httpInfo);

            //for(int i = 0; i < 2; i++)
            //{
            //    //서버랑 통신시 아래 링크 넣기
            //    StartCoroutine(Test("C:/Users/user/Videos/Captures/" + (i + 1) + ".mp4", (i + 1) + ".mp4"));

            //}
        }
        //샘플 로컬
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // 서버한테 영상 정보 요청
            HttpInfo httpInfo = new HttpInfo();

            Uri uri = new Uri(Application.streamingAssetsPath + "/TestData/" + "VideoInfo.csv");

            httpInfo.Set(RequestType.GET, uri.AbsoluteUri, (downloadHandler) => {

                //데이터 셋팅
                J_DataManager.instance.SetVideoInfoList(downloadHandler.text);

                //UI 만들자
                for(int i = 0; i < J_DataManager.instance.videoInfoList.Count; i++)
                {
                    GameObject video = Instantiate(videoFactory, trCtOfSV);
                    J_VideoItem item = video.GetComponent<J_VideoItem>();
                    item.SetItem(J_DataManager.instance.videoInfoList[i]);
                }

            }, false);

            HttpManager.Get().SendRequest(httpInfo);

            //for(int i = 0; i < 2; i++)
            //{
            //    //서버랑 통신시 아래 링크 넣기
            //    StartCoroutine(Test("C:/Users/user/Videos/Captures/" + (i + 1) + ".mp4", (i + 1) + ".mp4"));

            //}
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
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
    }

    //int videoNum;
    //IEnumerator Test(string u, string saveName)
    //{
    //    Uri uri = new Uri(u);
    //    //서버랑 통신받을 땐 위에꺼 주석처리하기


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
    //            //byteData 를 file 에 쓰자
    //            file.Write(videoBytes, 0, videoBytes.Length);
    //            file.Close();

    //            //비디오 영상 완료
    //            //GameObject Video = new GameObject("Video" + videoNum); //비디오 프리팹 생성하지말고 넣는다
    //            //GameObject Video = Instantiate(videoFactory, trCtOfSV);
    //            //Video.transform.SetParent(trCtOfSV.transform); //비디오 프리팹 위치 content 하위

    //            videoList = Instantiate(videoFactory, trCtOfSV);

    //            J_VideoItem videoItem = videoList.GetComponent<J_VideoItem>();
    //            //videoItem.SetItem()


    //            //GameObject go = Instantiate(videoFactory, trCtOfSV);
    //            //go.transform.SetParent(Video.transform); //go의 위치 video 하위
    //            VideoPlayer videoPlayer = videoList.GetComponentInChildren<VideoPlayer>();


    //            RenderTexture rt = new RenderTexture(512, 256, 24);
    //            videoPlayer.targetTexture = rt;
    //            videoPlayer.GetComponentInChildren<RawImage>().texture = rt;


    //            videoPlayer.url = Application.dataPath + "/" + saveName;
    //            videoPlayer.Play();

    //            // 여기서 videoBytes를 사용하여 동영상을 처리하거나 저장합니다.
    //            Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");
    //        }
    //    }
    //}
}

