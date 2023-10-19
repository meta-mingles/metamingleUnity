using ExitGames.Client.Photon.StructWrapping;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class J_VideoReceiver : MonoBehaviour
{
    public enum RequestType
    {
        GET, //Get 요청
        POST, //Post 요청
        PUT, //Put 요청
        DELETE, // Delete요청
        TEXTURE //이미지,  비디오 텍스처 요청
    }

    public enum ResponseType
    {
        JSON, //JSON 응답
        TEXTURE //이미지,  비디오 텍스처 응답
    }

    [Serializable]
    public class HttpInfo
    {
        public RequestType requestType; //요청유형
        public string url = ""; //요청 url
        public string body; //요청 본문 데이터(post, put 요청용)
        public Action<DownloadHandler> onReceive; //응답 처리 콜백함수
        //public ResponseType responseType; //응답유형(JSON 또는 TEXTURE)

        // HttpInfo 객체를 설정하는 메서드
        public void Set(RequestType type, string u, Action<DownloadHandler> callback,bool useDefaultUrl = true)
        {
            requestType = type;
            if (useDefaultUrl) url = "http://192.168.0.6:8080/short-form/1";
            url += u; //지정된 URL로 업데이트
            onReceive = callback; // 응답 처리 콜백 함수 설정
        }
    }

    static J_VideoReceiver instance;

    public static J_VideoReceiver Get()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("J_VideoReceiver");
            go.AddComponent<J_VideoReceiver>();
        }

        return instance;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 예제 사용법
        // 비디오를 요청하고 처리
        HttpInfo videoRequest = new HttpInfo();
        videoRequest.Set(RequestType.GET,string.Empty, (downloadHandler) =>
        {
            Debug.Log("비디오 데이터를 받았습니다.");
            // 비디오 데이터를 처리할 수 있음

            //예를 들면 파일이나 다른 방식으로 저장 가능

        });

        SendRequest(videoRequest);
    }

    // 서버에게 REST API 요청 (GET, POST, PUT, DELETE)
    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    //IEnumerator CoSendRequest(HttpInfo httpInfo)
    //{
    //    using (UnityWebRequest req = UnityWebRequest.Get(httpInfo.url))

    //    //서버에 요청을 보내고 응답이 올때까지 양보
    //    yield return req.SendWebRequest();

    //    //만약 응답 성공시
    //    if (req.result == UnityWebRequest.Result.Success)
    //    {
    //        //비디오 응답처리
    //        if (httpInfo.onReceive != null)
    //        {
    //            Debug.Log(www.error);
    //            httpInfo.onReceive(req.downloadHandler);
    //        }
    //    }
    //    //통신 실패
    //    else
    //    {
    //        byte[] videoBytes = www.downloadHandler.data;
    //        Debug.LogError("네트워크 오류: " + req.error);
    //    }

        //형훈이형
        IEnumerator CoSendRequest(HttpInfo httpInfo)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(httpInfo.url))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
                 Debug.Log(httpInfo.url);

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    byte[] videoBytes = www.downloadHandler.data;
                    // 여기서 videoBytes를 사용하여 동영상을 처리하거나 저장합니다.
                    Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");
                }
            }
        }
    }

