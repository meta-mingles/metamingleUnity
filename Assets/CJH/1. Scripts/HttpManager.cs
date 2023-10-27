using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

    public string body = "{}";
    public Action<DownloadHandler> onReceive;
    public Action<DownloadHandler, int> onReceiveImage;


    public int imageId;

    public void Set(
        RequestType type,
        string u,
        Action<DownloadHandler> callback,
        bool useDefaultUrl = true)
    {
        requestType = type;
        if (useDefaultUrl) url = "http://192.168.165.74:8080/";
        url += u;
        onReceive = callback;
    }
}


public class HttpManager : MonoBehaviour
{
    static HttpManager instance;

    public static HttpManager Get()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("HttpStudy");
            go.AddComponent<HttpManager>();
        }

        return instance;
    }

    //Texture
    public static HttpManager Texture()
    {
        if(instance == null)
        {
            GameObject go = new GameObject("HttpTexture");
            go.AddComponent<HttpManager>();
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

    }

    void Update()
    {

    }

    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    IEnumerator CoSendRequest(HttpInfo httpInfo)
    {
        UnityWebRequest req = null;

        //POST, GET, PUT, DELETE �б�
        switch (httpInfo.requestType)
        {
            case RequestType.GET:
                req = UnityWebRequest.Get(httpInfo.url);
                break;
            case RequestType.POST:

                string str = JsonUtility.ToJson(httpInfo.body);
                req = UnityWebRequest.Post(httpInfo.url, str);

                byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
                req.uploadHandler = new UploadHandlerRaw(byteBody);
                req.SetRequestHeader("Content-Type", "application/json");

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

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {


            if (httpInfo.requestType == RequestType.TEXTURE)
            {
                if (httpInfo.onReceiveImage != null)
                {
                    httpInfo.onReceiveImage(req.downloadHandler, httpInfo.imageId);
                }
            }
            else
            {          
                if(req.downloadHandler.text.Length > 0)
                {
                    print(req.downloadHandler.text);
                }
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

        req.Dispose();
    }
}