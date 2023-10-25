using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHRecordManager : MonoBehaviour
{
    public static KHHRecordManager Instance;

    public GameObject record;
    public GameObject edit;

    public Camera captureCamera;
    public RenderTexture captureRenderTexture;

    public Button recordExitButton;
    public Button recordStartButton;
    public Button recordStopButton;

    //[Header("TDPT")]
    //public MovieSender movieSender;

    public KHHModelRecorder recorder;
    public MicrophoneRecorder microphoneRecorder;
    public KHHMotionDataManager motionDataManager;
    public VideoCapture videoCapture;
    public GameObject barracudaRunner;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        recordExitButton.onClick.AddListener(() =>
        {
            record.SetActive(false);
            edit.SetActive(true);
            captureCamera.targetTexture = captureRenderTexture;
            motionDataManager.Refresh();
            videoCapture.CameraPlayStop();
            barracudaRunner.SetActive(false);
        });

        recordStartButton.onClick.AddListener(() =>
        {
            recorder.StartRecord();
            microphoneRecorder.StartRecordMicrophone();
        });

        recordStopButton.onClick.AddListener(() =>
        {
            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv";
            recorder.StopRecord(fileName);
            microphoneRecorder.StopRecordMicrophone(fileName);
        });

        //movieSender.CameraPlayStart();
    }

    public bool Init()
    {
        if (videoCapture.SetEnd)
            videoCapture.CameraPlayStart();
        return videoCapture.SetEnd;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
