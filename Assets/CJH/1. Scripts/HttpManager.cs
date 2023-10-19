using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

//https://jsonplaceholder.typicode.com

[Serializable]
public class JsonList<T>
{
    public List<T> data;
}

[Serializable]
public struct SignUpInfo
{
    public string nickname;
}

[Serializable]
public struct NetFishInfo
{
    public int status;
    public string message;
    public data data;
}

[Serializable]
public struct data
{
    public userDTO user;
    public fishDTO fish;
    public int price;
}

[Serializable]
public struct userDTO
{
    public string nickname;
    public int money;
}
[Serializable]
public struct fishDTO
{
    public string name;
    public int price;
}


[Serializable]
public struct NetActivityInfo
{
    public int id;
    public string image;
    public string village;
    public string villageEn;
    public string name;
    public string price;
    public string period;
    public string people;

}

[Serializable]
public struct NetVillageInfo
{
    public int id;
    public string name;
    public string image;
    public string address;
    public string fishNameEn;
}


[Serializable]
public struct UserInfo
{
    public int id;
    public string userId;
    public string password;
    public string nickname;
    public int money;
}

[Serializable]
public struct ReceiveUserInfo
{
    public UserInfo data;
}




public enum RequestType
{
    GET,
    POST,
    PUT,
    DELETE,
    TEXTURE
}

//�� ����ϱ� ���� ����
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
        if (useDefaultUrl) url = "http://192.168.0.6:8080/short-form/1";
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
            //���� ������Ʈ �����
            GameObject go = new GameObject("HttpStudy");
            //������� ���� ������Ʈ�� HttpManager ������Ʈ ������
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

    //�������� REST API ��û (GET, POST, PUT, DELETE)
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
                //Get������� req �� ���� ����
                req = UnityWebRequest.Get(httpInfo.url);
                break;
            case RequestType.POST:

                string str = JsonUtility.ToJson(httpInfo.body);
                req = UnityWebRequest.Post(httpInfo.url, str);

                byte[] byteBody = Encoding.UTF8.GetBytes(httpInfo.body);
                req.uploadHandler = new UploadHandlerRaw(byteBody);
                //��� �߰�
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

        //������ ��û�� ������ ������ �ö����� �纸�Ѵ�.
        yield return req.SendWebRequest();

        //���࿡ ������ �����ߴٸ�
        if (req.result == UnityWebRequest.Result.Success)
        {
            //print("��Ʈ��ũ ���� : " + req.downloadHandler.text);


            if (httpInfo.requestType == RequestType.TEXTURE)
            {
                if (httpInfo.onReceiveImage != null)
                {
                    httpInfo.onReceiveImage(req.downloadHandler, httpInfo.imageId);
                }
            }
            else
            {
                if (httpInfo.onReceive != null)
                {
                    httpInfo.onReceive(req.downloadHandler);
                }
            }

        }
        //��� ����
        else
        {
            print("네트워크 에러 : " + req.error);
        }

        req.Dispose();
    }
}