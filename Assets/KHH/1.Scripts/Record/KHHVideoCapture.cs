using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class KHHVideoCapture : MonoBehaviour
{
    public static KHHVideoCapture instance;

    private readonly object _lock = new object();
    public object Lock => _lock;
    public Queue<string> actionQueue = new Queue<string>();

    private string filePath;
    //public string FilePath { get { return filePath; } }

    //업로드중
    public bool IsUploading { get; set; } = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Application.runInBackground = true;
        //isPlayVideo = false;
    }

    void Update()
    {
        //비디오 전송
        lock (_lock)
        {
            if (actionQueue.Count > 0)
            {
                filePath = actionQueue.Dequeue();
                OnCaptureComplete();
            }
        }
    }

    public void OnCaptureComplete()
    {
        int count = 0;
        string newFilePath = RockVR.Video.PathConfig.SaveFolder + KHHEditData.VideoTitle + "_" + count.ToString() + ".mp4";
        while (System.IO.File.Exists(newFilePath))
        {
            count++;
            newFilePath = RockVR.Video.PathConfig.SaveFolder + KHHEditData.VideoTitle + "_" + count.ToString() + ".mp4";
        }
        System.IO.File.Copy(filePath, newFilePath);
        System.IO.File.Delete(filePath);
        filePath = newFilePath;
        Debug.Log("Capture Finish");
        //완료창 띄우기
    }

    byte[] ReadVideoAsBytes(string path)
    {
        if (System.IO.File.Exists(path))
        {
            return System.IO.File.ReadAllBytes(path);
        }
        return null;
    }

    string uploadShortformURL = "http://www.metamingle.store:8081/short-form-firebase";
    public void UploadShortformVideo(string title, string description)
    {
        IsUploading = true;

        StartCoroutine(CoUploadShortformVideo(ReadVideoAsBytes(filePath), title, description));

        ////비디오 삭제
        //System.IO.File.Delete(filePathShortform);
    }

    IEnumerator CoUploadShortformVideo(byte[] videoBytes, string title, string description)
    {
        string uuid = "none";  // 없을 시 null 반환
        if (PlayerPrefs.HasKey($"{KHHEditData.VideoTitle}uuid"))
            uuid = PlayerPrefs.GetString($"{KHHEditData.VideoTitle}uuid");  // 시나리오 작성 시 저장된 키 값 꺼냄

        using (UnityWebRequest www = new UnityWebRequest(uploadShortformURL, "POST"))
        {
            // Create a new WWWForm object to package your data.
            WWWForm form = new WWWForm();

            // Add the video bytes to the form data.
            form.AddBinaryData("video", videoBytes, "video.mp4", "video/mp4");

            // Add the title to the form data.
            form.AddField("title", title);
            form.AddField("description", description);
            form.AddField("uuid", uuid);  // uuid 함께 전송

            // Set the form as the request's upload handler.
            www.uploadHandler = new UploadHandlerRaw(form.data);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the correct headers for a multipart/form-data POST request.
            www.SetRequestHeader("Authentication", HttpManager.instance.token);
            foreach (KeyValuePair<string, string> header in form.headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                byte[] stringBytes = www.downloadHandler.data;
                string result = Encoding.UTF8.GetString(stringBytes);
                Debug.Log("Upload Successful:" + result);
            }

            IsUploading = false;
        }
    }

    string uploadInteractiveURL = "http://www.metamingle.store:8081/interactive-movie";
    public void UploadInteractiveVideo(string title, string description, string c1, string c1Path, string c2, string c2Path)
    {
        IsUploading = true;

        List<byte[]> videoBytes = new List<byte[]>();
        List<string> paths = new List<string>
        {
            filePath,
            c1Path,
            c2Path
        };
        for (int i = 0; i < paths.Count; i++)
            videoBytes.Add(ReadVideoAsBytes(paths[i]));

        StartCoroutine(CoUploadInteractiveVideo(videoBytes, title, description, c1, c2));
    }

    IEnumerator CoUploadInteractiveVideo(List<byte[]> videoBytesList, string title, string description, string c1, string c2)
    {
        string uuid = "none";  // 없을 시 null 반환
        if (PlayerPrefs.HasKey($"{KHHEditData.VideoTitle}uuid"))
            uuid = PlayerPrefs.GetString($"{KHHEditData.VideoTitle}uuid");  // 시나리오 작성 시 저장된 키 값 꺼냄

        using (UnityWebRequest www = new UnityWebRequest(uploadInteractiveURL, "POST"))
        {
            // Create a new WWWForm object to package your data.
            WWWForm form = new WWWForm();

            // Add the video bytes to the form data.
            for (int i = 0; i < videoBytesList.Count; i++)
            {
                form.AddBinaryData($"video{i + 1}", videoBytesList[i], $"video{i + 1}.mp4", "video/mp4");
            }

            // Add the title to the form data.
            form.AddField("title", title);
            form.AddField("description", description);
            form.AddField("choice1", c1);
            form.AddField("choice2", c2);
            form.AddField("uuid", uuid);  // uuid 함께 전송

            // Set the form as the request's upload handler.
            www.uploadHandler = new UploadHandlerRaw(form.data);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the correct headers for a multipart/form-data POST request.
            www.SetRequestHeader("Authentication", HttpManager.instance.token);
            foreach (KeyValuePair<string, string> header in form.headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                byte[] stringBytes = www.downloadHandler.data;
                string result = Encoding.UTF8.GetString(stringBytes);
                Debug.Log("Upload Successful:" + result);
            }

            IsUploading = false;
        }
    }
}

