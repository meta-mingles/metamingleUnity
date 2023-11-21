using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;
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
    public Action<DownloadHandler, int> onReceiveImage;

    public string loginId;
    public string loginPW;
    public string token;

    public int imageId;

    public void Set(
        RequestType type,
        string u,
        Action<DownloadHandler> callback,
        bool useDefaultUrl = true)
    {
        requestType = type;
        if (useDefaultUrl) url = "http://metaverse.ohgiraffers.com:8080"; //기존 서버 url
        url += u;
        onReceive = callback;
    }
}


public class HttpManager : MonoBehaviour
{
    public static HttpManager instance;

    public string email= "";
    public string password = "";
    public string nickname = "";

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
        if(httpInfo.requestType == RequestType.POST)
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

        if(token.Length > 0)
        {
            req.SetRequestHeader("Authorization", token);
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
                req.SetRequestHeader("Authorization", token);
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
            print("네트워크 에러 : " + req.error);
        }
    }
}