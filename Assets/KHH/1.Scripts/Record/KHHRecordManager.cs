using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHRecordManager : MonoBehaviour
{
    public static KHHRecordManager Instance;
    bool isActive = false;

    public GameObject record;
    public GameObject edit;

    public Camera captureCamera;
    public GameObject captureScreen;

    public Button recordExitButton;
    public Button recordStartButton;
    public Button recordStopButton;

    //[Header("TDPT")]
    //public MovieSender movieSender;

    public KHHModelRecorder modelRecorder;
    public KHHMotionDataManager motionDataManager;
    public VideoCapture videoCapture;
    public VNectBarracudaRunner barracudaRunner;

    bool isRecording = false;
    bool startRecord = false;
    public bool StartRecord { get { return startRecord; } set { startRecord = value; } }

    RenderTexture captureRenderTexture;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (recordExitButton != null) recordExitButton.onClick.AddListener(RecordExitButtonEvent);
        if (recordStartButton != null) recordStartButton.onClick.AddListener(RecordStartButtonEvent);
        if (recordStopButton != null) recordStopButton.onClick.AddListener(RecordStopButtonEvent);

        captureRenderTexture = captureCamera.targetTexture;
        //movieSender.CameraPlayStart();
    }

    public bool Init()
    {
        if (videoCapture.SetEnd)
        {
            captureScreen.gameObject.SetActive(true);
            videoCapture.CameraPlayStart();
            isActive = true;
        }
        return videoCapture.SetEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (isRecording && startRecord == false)
        {
            if (modelRecorder.IsRecording && MicrophoneRecorder.Instance.IsRecording)
            {
                startRecord = true;
                //대기
                KHHCanvasShield.Instance.Close();
            }
            return;
        }

        if (!barracudaRunner.IsTracking)
            KHHCanvasShield.Instance.Show();
        else
            KHHCanvasShield.Instance.Close();
    }

    void RecordExitButtonEvent()
    {
        if (isRecording) RecordStopButtonEvent();
        isActive = false;
        record.SetActive(false);
        edit.SetActive(true);
        captureScreen.gameObject.SetActive(false);
        captureCamera.targetTexture = captureRenderTexture;
        motionDataManager.Refresh();
        videoCapture.CameraPlayStop();
        barracudaRunner.gameObject.SetActive(false);
        barracudaRunner.IsTracking = false;
    }

    void RecordStartButtonEvent()
    {
        isRecording = true;
        KHHCanvasShield.Instance.Show();
        modelRecorder.StartRecord();
        MicrophoneRecorder.Instance.StartRecordMicrophone();
        recordStartButton.gameObject.SetActive(false);
        recordStopButton.gameObject.SetActive(true);
    }

    void RecordStopButtonEvent()
    {
        isRecording = false;
        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
        modelRecorder.StopRecord(KHHEditData.FileMotionPath + "/" + fileName + ".csv");
        MicrophoneRecorder.Instance.StopRecordMicrophone(fileName);
        recordStartButton.gameObject.SetActive(true);
        recordStopButton.gameObject.SetActive(false);
    }
}
