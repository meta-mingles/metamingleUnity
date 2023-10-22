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

    private bool isPlayVideo = false;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        Application.runInBackground = true;
        isPlayVideo = false;
    }

    void Update()
    {
        //비디오 전송
        lock (_lock)
        {
            if (actionQueue.Count > 0)
            {
                OnCaptureComplete(actionQueue.Dequeue(), "title1");
            }
        }
    }

    private void OnGUI()
    {
        if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.NOT_START)
        {
            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Start Capture"))
            {
                VideoCaptureCtrl.instance.StartCapture();
            }
        }
        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STARTED)
        {
            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Stop Capture"))
            {
                VideoCaptureCtrl.instance.StopCapture();
            }
            if (GUI.Button(new Rect(180, Screen.height - 60, 150, 50), "Pause Capture"))
            {
                VideoCaptureCtrl.instance.ToggleCapture();
            }
        }
        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.PAUSED)
        {
            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Stop Capture"))
            {
                VideoCaptureCtrl.instance.StopCapture();
            }
            if (GUI.Button(new Rect(180, Screen.height - 60, 150, 50), "Continue Capture"))
            {
                VideoCaptureCtrl.instance.ToggleCapture();
            }
        }
        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.STOPPED)
        {
            if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Processing"))
            {
                // Waiting processing end.
            }
        }
        else if (VideoCaptureCtrl.instance.status == VideoCaptureCtrl.StatusType.FINISH)
        {
            if (!isPlayVideo)
            {
                if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "View Video"))
                {
#if UNITY_5_6_OR_NEWER
                    // Set root folder.
                    isPlayVideo = true;
                    VideoPlayer.instance.SetRootFolder();
                    // Play capture video.
                    VideoPlayer.instance.PlayVideo();
                }
            }
            else
            {
                if (GUI.Button(new Rect(10, Screen.height - 60, 150, 50), "Next Video"))
                {
                    // Turn to next video.
                    VideoPlayer.instance.NextVideo();
                    // Play capture video.
                    VideoPlayer.instance.PlayVideo();
#else
                        // Open video save directory.
                        Process.Start(PathConfig.saveFolder);
#endif
                }
            }
        }
    }

    public void OnCaptureComplete(string filePath, string title)
    {
        StartCoroutine(UploadVideo(ReadVideoAsBytes(filePath), title));
    }

    byte[] ReadVideoAsBytes(string path)
    {
        if (System.IO.File.Exists(path))
        {
            return System.IO.File.ReadAllBytes(path);
        }
        return null;
    }

    string uploadURL = "http://192.168.0.6:8080/short-form-firebase";

    IEnumerator UploadVideo(byte[] videoBytes, string title)
    {
        using (UnityWebRequest www = new UnityWebRequest(uploadURL, "POST"))
        {
            // Create a new WWWForm object to package your data.
            WWWForm form = new WWWForm();

            // Add the video bytes to the form data.
            form.AddBinaryData("video", videoBytes, "video.mp4", "video/mp4");

            // Add the title to the form data.
            form.AddField("title", title);

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
}
