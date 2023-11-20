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

    //    private void OnGUI()
    //    {
    //        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.NOT_START)
    //        {
    //            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Start Capture"))
    //            {
    //                VideoCaptureCtrl.instance.StartCapture();
    //            }
    //        }
    //        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STARTED)
    //        {
    //            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Stop Capture"))
    //            {
    //                VideoCaptureCtrl.instance.StopCapture();
    //            }
    //            if (GUI.Button(new Rect(180, Screen.height - 60, 150, 50), "Pause Capture"))
    //            {
    //                VideoCaptureCtrl.instance.ToggleCapture();
    //            }
    //        }
    //        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.PAUSED)
    //        {
    //            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Stop Capture"))
    //            {
    //                VideoCaptureCtrl.instance.StopCapture();
    //            }
    //            if (GUI.Button(new Rect(180, Screen.height - 60, 150, 50), "Continue Capture"))
    //            {
    //                VideoCaptureCtrl.instance.ToggleCapture();
    //            }
    //        }
    //        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STOPPED)
    //        {
    //            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Processing"))
    //            {
    //                // Waiting processing end.
    //            }
    //        }
    //        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.FINISH)
    //        {
    //            if (!isPlayVideo)
    //            {
    //                if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "View Video"))
    //                {
    //#if UNITY_5_6_OR_NEWER
    //                    // Set root folder.
    //                    isPlayVideo = true;
    //                    VideoPlayer.instance.SetRootFolder();
    //                    // Play capture video.
    //                    VideoPlayer.instance.PlayVideo();
    //                }
    //            }
    //            else
    //            {
    //                if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Next Video"))
    //                {
    //                    // Turn to next video.
    //                    VideoPlayer.instance.NextVideo();
    //                    // Play capture video.
    //                    VideoPlayer.instance.PlayVideo();
    //#else
    //                        // Open video save directory.
    //                        Process.Start(PathConfig.saveFolder);
    //#endif
    //                }
    //            }
    //        }
    //    }

    public void OnCaptureComplete()
    {
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

    string uploadShortformURL = "http://metaverse.ohgiraffers.com:8080/short-form-firebase";
    public void UploadShortformVideo(string title, string description)
    {
        IsUploading = true;

        StartCoroutine(CoUploadShortformVideo(ReadVideoAsBytes(filePath), title, description));

        ////비디오 삭제
        //System.IO.File.Delete(filePathShortform);
    }

    IEnumerator CoUploadShortformVideo(byte[] videoBytes, string title, string description)
    {
        using (UnityWebRequest www = new UnityWebRequest(uploadShortformURL, "POST"))
        {
            // Create a new WWWForm object to package your data.
            WWWForm form = new WWWForm();

            // Add the video bytes to the form data.
            form.AddBinaryData("video", videoBytes, "video.mp4", "video/mp4");

            // Add the title to the form data.
            form.AddField("title", title);
            form.AddField("description", description);

            // Set the form as the request's upload handler.
            www.uploadHandler = new UploadHandlerRaw(form.data);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the correct headers for a multipart/form-data POST request.
            www.SetRequestHeader("Authorization", HttpManager.instance.token);
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

    string uploadInteractiveURL = "http://metaverse.ohgiraffers.com:8080/interactive-movie";
    public void UploadInteractiveVideo(string title, string description, string c1, string c1Path, string c2, string c2Path)
    {
        IsUploading = true;

        List<byte[]> videoBytes = new List<byte[]>();
        List<string> paths = new List<string>();
        paths.Add(filePath);
        paths.Add(c1Path);
        paths.Add(c2Path);
        for (int i = 0; i < paths.Count; i++)
            videoBytes.Add(ReadVideoAsBytes(paths[i]));

        StartCoroutine(CoUploadInteractiveVideo(videoBytes, title, description, c1, c2));
    }

    IEnumerator CoUploadInteractiveVideo(List<byte[]> videoBytesList, string title, string description, string c1, string c2)
    {
        using (UnityWebRequest www = new UnityWebRequest(uploadInteractiveURL, "POST"))
        {
            // Create a new WWWForm object to package your data.
            WWWForm form = new WWWForm();

            // Add the video bytes to the form data.
            for (int i = 0; i < videoBytesList.Count; i++)
            {
                form.AddBinaryData($"video{i + 1}", videoBytesList[i], $"video{i + 1}.mp4", "video/mp4");
            }

            //form.AddBinaryData("video1", videoBytesList[0], "video1.mp4", "video/mp4"); //첫 동영상
            //form.AddBinaryData("video2", videoBytesList[1], "video2.mp4", "video/mp4"); //선택지1
            //form.AddBinaryData("video3", videoBytesList[2], "video3.mp4", "video/mp4"); //선택지2

            // Add the title to the form data.
            form.AddField("title", title);
            form.AddField("description", description);
            form.AddField("choice1", c1);
            form.AddField("choice2", c2);

            // Set the form as the request's upload handler.
            www.uploadHandler = new UploadHandlerRaw(form.data);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the correct headers for a multipart/form-data POST request.
            www.SetRequestHeader("Authorization", HttpManager.instance.token);
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

