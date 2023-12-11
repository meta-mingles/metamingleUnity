using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
public enum RequestType
{
    GET,
    POST,
    PUT,
    DELETE,
    TEXTURE
}
public class HttpInfo
{
    public RequestType requestType;
    public string url = "";
    public string testUrl = "";
    public string body = "{}";
    public Action<DownloadHandler> onReceive;
    public Action onReceiveFail;
    public Action<DownloadHandler, int> onReceiveImage;
    public string loginId;
    public string loginPW;
    public string token;
    public int imageId;
    public void Set(
        RequestType type,
        string u,
        Action<DownloadHandler> callback,
        Action callbackFail = null,
        bool useDefaultUrl = true)
    {
        requestType = type;
        if (useDefaultUrl) url = "http://www.metamingle.store:8081"; //기존 서버 url
        //if (useDefaultUrl) testUrl = "http://192.168.0.25:8080"; //기존 서버 url
        url += u;
        //testUrl += u;
        onReceive = callback;
        onReceiveFail = callbackFail;
    }
}
public class HttpManager : MonoBehaviour
{
    public static HttpManager instance;

    //로그인,회원가입
    public string email = "";
    public string password = "";
    public string nickname = "";

    //퀴즈
    public int rankNo;
    public string english = "";
    public string korean = "";
    public int shortFormNo;
    public string token = "";
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
    public void SendRequest(HttpInfo httpInfo)
    {
        if (httpInfo.requestType == RequestType.POST)
        {
            StartCoroutine(Post(httpInfo));
        }
        else
        {
            StartCoroutine(CoSendRequest(httpInfo));
        }
    }

    IEnumerator CoSendRequest(HttpInfo httpInfo)
    {
        UnityWebRequest req = null;
        //POST, GET, PUT, DELETE  б 
        switch (httpInfo.requestType)
        {
            case RequestType.GET:
                //Debug.Log(httpInfo.url);
                //req = UnityWebRequest.Get(httpInfo.testUrl);
                req = UnityWebRequest.Get(httpInfo.url);
                break;

            case RequestType.PUT:
                req = UnityWebRequest.Put(httpInfo.url, httpInfo.body);
                break;
            case RequestType.DELETE:
                req = UnityWebRequest.Delete(httpInfo.url);
                break;
            case RequestType.TEXTURE:
                req = UnityWebRequestTexture.GetTexture(httpInfo.url);
                break;
        }
        if (token.Length > 0)
        {
            req.SetRequestHeader("Authentication", token);
        }
        yield return req.SendWebRequest();
        SetResult(req, httpInfo);
    }

    IEnumerator Post(HttpInfo httpInfo)
    {
        string str = JsonUtility.ToJson(httpInfo.body);
        using (UnityWebRequest req = UnityWebRequest.Post(httpInfo.url, str))
        {
            byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
            req.uploadHandler.Dispose();
            req.uploadHandler = new UploadHandlerRaw(byteBody);
            //헤더추가
            req.SetRequestHeader("Content-Type", "application/json");

            if (token.Length > 0)
            {
                req.SetRequestHeader("Authentication", token);
            }
            yield return req.SendWebRequest();
            SetResult(req, httpInfo);
        }
    }

    void SetResult(UnityWebRequest req, HttpInfo httpInfo)
    {
        if (req.result == UnityWebRequest.Result.Success)
        {
            //이미지 다운로드
            if (httpInfo.requestType == RequestType.TEXTURE)
            {
                if (httpInfo.onReceiveImage != null)
                {
                    httpInfo.onReceiveImage(req.downloadHandler, httpInfo.imageId);
                }
            }
            else
            {
                if (req.downloadHandler.text.Length > 0)
                {
                    //print(req.downloadHandler.text);
                }
                //영상
                if (httpInfo.onReceive != null)
                {
                    httpInfo.onReceive(req.downloadHandler);
                }
            }
        }
        else
        {
            httpInfo.onReceiveFail?.Invoke();
            //로그인 안될때
            print("네트워크 에러 : " + req.error);
        }
    }
}