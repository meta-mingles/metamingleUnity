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

    public static J_VideoReceiver instance;
    private string videoUrl; //���� url ������ ����
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
    
    public string GetVideoUrl()
    {
        return videoUrl;
    }

    void Start()
    {
        
        return;
        // ���� ����
        // ������ ��û�ϰ� ó��
        HttpInfo videoRequest = new HttpInfo();
        videoRequest.Set(RequestType.GET,string.Empty, (downloadHandler) =>
        {
            Debug.Log("���� �����͸� �޾ҽ��ϴ�.");
            // ���� �����͸� ó���� �� ����

            //���� ��� �����̳� �ٸ� ������� ���� ����
            videoUrl = "http://192.168.0.6:8080/short-form/1";

        });

        SendRequest(videoRequest);
    }

    // �������� REST API ��û (GET, POST, PUT, DELETE)
    public void SendRequest(HttpInfo httpInfo)
    {
        StartCoroutine(CoSendRequest(httpInfo));
    }

    //Get �۽�
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            for(int i = 0; i < 2; i++)
            {
                //������ ��Ž� �Ʒ� ��ũ �ֱ�
                StartCoroutine(Test("C:/Users/user/Videos/Captures/" + (i + 1) + ".mp4", (i + 1) + ".mp4"));

            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            
        }
    }

    VideoPlayer v;
    public GameObject rawImag_VideoFactory;
    public Transform trCanvas;
    IEnumerator Test(string u, string saveName)
    {
        Uri uri = new Uri(u);
        //������ ��Ź��� �� ������ �ּ�ó���ϱ�


        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {                
                byte[] videoBytes = www.downloadHandler.data;

                FileStream file = new FileStream(Application.dataPath + "/" + saveName, FileMode.Create);                
                //byteData �� file �� ����
                file.Write(videoBytes, 0, videoBytes.Length);
                file.Close();

                GameObject go = Instantiate(rawImag_VideoFactory, trCanvas);
                VideoPlayer videoPlayer = go.GetComponent<VideoPlayer>();

                RenderTexture rt = new RenderTexture(512, 256, 24);
                videoPlayer.targetTexture = rt;
                videoPlayer.GetComponent<RawImage>().texture = rt;

                videoPlayer.url = Application.dataPath + "/" + saveName;
                videoPlayer.Play();



                // ���⼭ videoBytes�� ����Ͽ� �������� ó���ϰų� �����մϴ�.
                Debug.Log("Download Successful: " + videoBytes.Length + " bytes received.");
            }
        }
    }
}

