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
        GET, //Get ��û
        POST, //Post ��û
        PUT, //Put ��û
        DELETE, // Delete��û
        TEXTURE //�̹���,  ���� �ؽ�ó ��û
    }

    public enum ResponseType
    {
        JSON, //JSON ����
        TEXTURE //�̹���,  ���� �ؽ�ó ����
    }

    [Serializable]
    public class HttpInfo
    {
        public RequestType requestType; //��û����
        public string url = ""; //��û url
        public string body; //��û ���� ������(post, put ��û��)
        public Action<DownloadHandler> onReceive; //���� ó�� �ݹ��Լ�
        //public ResponseType responseType; //��������(JSON �Ǵ� TEXTURE)

        // HttpInfo ��ü�� �����ϴ� �޼���
        public void Set(RequestType type, string u, Action<DownloadHandler> callback,bool useDefaultUrl = true)
        {
            requestType = type;
            if (useDefaultUrl) url = "http://192.168.0.6:8080/short-form/1";
            url += u; //������ URL�� ������Ʈ
            onReceive = callback; // ���� ó�� �ݹ� �Լ� ����
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
        // ���� ����
        // ������ ��û�ϰ� ó��
        HttpInfo videoRequest = new HttpInfo();
        videoRequest.Set(RequestType.GET,string.Empty, (downloadHandler) =>
        {
            Debug.Log("���� �����͸� �޾ҽ��ϴ�.");
            // ���� �����͸� ó���� �� ����

            //���� ��� �����̳� �ٸ� ������� ���� ����

        });

        SendRequest(videoRequest);
    }

    // �������� REST API ��û (GET, POST, PUT, DELETE)
    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    //IEnumerator CoSendRequest(HttpInfo httpInfo)
    //{
    //    using (UnityWebRequest req = UnityWebRequest.Get(httpInfo.url))

    //    //������ ��û�� ������ ������ �ö����� �纸
    //    yield return req.SendWebRequest();

    //    //���� ���� ������
    //    if (req.result == UnityWebRequest.Result.Success)
    //    {
    //        //���� ����ó��
    //        if (httpInfo.onReceive != null)
    //        {
    //            Debug.Log(www.error);
    //            httpInfo.onReceive(req.downloadHandler);
    //        }
    //    }
    //    //��� ����
    //    else
    //    {
    //        byte[] videoBytes = www.downloadHandler.data;
    //        Debug.LogError("��Ʈ��ũ ����: " + req.error);
    //    }

        //��������
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
                    // ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
                    Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");
                }
            }
        }
    }

