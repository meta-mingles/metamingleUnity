using RockVR.Video;
using System;
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

    private string filePathShortform;
    private List<string> filePathInteractive = new List<string>();

    //private bool isPlayVideo = false;
    public bool IsInteractive { get; set; } = false;

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
                if (IsInteractive)
                {
                    filePathInteractive.Add(actionQueue.Dequeue());
                    if (filePathInteractive.Count == 3)
                    {
                        OnCaptureComplete();
                    }
                }
                else
                {
                    filePathShortform = actionQueue.Dequeue();
                    OnCaptureComplete();
                }
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

    string uploadShortformURL = "http://" + "192.168.165.74:8080/short-form-firebase";
    public void UploadShortformVideo(string title)
    {
        StartCoroutine(CoUploadShortformVideo(ReadVideoAsBytes(filePathShortform), title));

        ////비디오 삭제
        //System.IO.File.Delete(filePathShortform);
    }

    IEnumerator CoUploadShortformVideo(byte[] videoBytes, string title)
    {
        using (UnityWebRequest www = new UnityWebRequest(uploadShortformURL, "POST"))
        {
            // Create a new WWWForm object to package your data.
            WWWForm form = new WWWForm();

            // Add the video bytes to the form data.
            form.AddBinaryData("video", videoBytes, "video.mp4", "video/mp4");

            // Add the title to the form data.
            form.AddField("title", title);
            form.AddField("description", "test description");

            // Set the form as the request's upload handler.
            www.uploadHandler = new UploadHandlerRaw(form.data);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the correct headers for a multipart/form-data POST request.
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
        }
    }

    string uploadInteractiveURL = "http://" + "192.168.165.74:8080/interactive-movie";
    public void UploadInteractiveVideo(string title, string choice1, string choice2)
    {
        IsUploading = true;

        List<byte[]> videoBytes = new List<byte[]>();
        for (int i = 0; i < filePathInteractive.Count; i++)
            videoBytes.Add(ReadVideoAsBytes(filePathInteractive[i]));

        StartCoroutine(CoUploadInteractiveVideo(videoBytes, title, choice1, choice2));

        ////비디오 삭제
        //for (int i = 0; i < filePathInteractive.Count; i++)
        //    System.IO.File.Delete(filePathInteractive[i]);
        //filePathInteractive.Clear();
    }

    IEnumerator CoUploadInteractiveVideo(List<byte[]> videoBytesList, string title, string choice1, string choice2)
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
            form.AddField("description", "test description");
            form.AddField("choice1", choice1);
            form.AddField("choice2", choice2);

            // Set the form as the request's upload handler.
            www.uploadHandler = new UploadHandlerRaw(form.data);
            www.downloadHandler = new DownloadHandlerBuffer();

            // Set the correct headers for a multipart/form-data POST request.
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

