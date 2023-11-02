using DG.Tweening;
using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHEditManager : MonoBehaviour
{
    public static KHHEditManager Instance;
    public string videoName = "test video";

    public GameObject record;
    public GameObject edit;

    public KHHMotionDataManager motionDataManager;
    public KHHSoundDataManager soundDataManager;
    public KHHBackgroundDataManager backgroundDataMaanger;

    public Camera captureCamera;
    public RenderTexture captureRenderTexture;
    public KHHScreenEditor screenEditor;
    public GameObject barracudaRunner;


    [Header("Left")]
    public Button recordButton;
    public Button motionButton;
    public Button soundButton;
    public Button backgroundButton;
    public Button captionButton;
    public Button interactiveButton;

    public CanvasGroup recordWaitCG;
    public GameObject motionPanel;
    public GameObject soundPanel;
    public GameObject backgroundPanel;
    public GameObject interactive;
    public KHHInteractiveButton interactiveButtonLeft;
    public KHHInteractiveButton interactiveButtonRight;

    [Header("Middle")]
    public Button playButton;
    public Button stopButton;
    public Button exportButton;

    public KHHExportWindow exportWindow;

    bool isInterActive = false;
    bool isExporting = false;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (recordButton != null) recordButton.onClick.AddListener(RecordButtonEvent);
        if (motionButton != null) motionButton.onClick.AddListener(MotionButtonEvent);
        if (soundButton != null) soundButton.onClick.AddListener(SoundButtonEvent);
        if (backgroundButton != null) backgroundButton.onClick.AddListener(BackgroundButtonEvent);
        if (interactiveButton != null) interactiveButton.onClick.AddListener(InteractiveButtonEvent);

        if (playButton != null) playButton.onClick.AddListener(PlayButtonEvent);
        if (stopButton != null) stopButton.onClick.AddListener(StopButtonEvent);
        if (exportButton != null) exportButton.onClick.AddListener(ExportButtonEvent);

        MotionButtonEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (KHHRecordManager.Instance.videoCapture.SetEnd && recordWaitCG.alpha == 1)
        {
            recordWaitCG.DOFade(0, 0.5f).OnComplete(() => { recordWaitCG.gameObject.SetActive(false); });
            recordWaitCG.blocksRaycasts = false;
        }
    }

    void RecordButtonEvent()
    {
        //전환이 가능한 상태
        if (KHHRecordManager.Instance.Init())
        {
            record.SetActive(true);
            edit.SetActive(false);
            captureCamera.targetTexture = null;
            barracudaRunner.SetActive(true);
        }
    }

    void MotionButtonEvent()
    {
        motionPanel.SetActive(true);
        soundPanel.SetActive(false);
        backgroundPanel.SetActive(false);
        motionButton.image.color = new Color32(200, 200, 200, 255);
        soundButton.image.color = new Color32(255, 255, 255, 255);
        backgroundButton.image.color = new Color32(255, 255, 255, 255);

        motionDataManager.Refresh();
    }

    public void SoundButtonEvent()
    {
        motionPanel.SetActive(false);
        soundPanel.SetActive(true);
        backgroundPanel.SetActive(false);
        motionButton.image.color = new Color32(255, 255, 255, 255);
        soundButton.image.color = new Color32(200, 200, 200, 255);
        backgroundButton.image.color = new Color32(255, 255, 255, 255);

        soundDataManager.Refresh();
    }

    public void BackgroundButtonEvent()
    {
        motionPanel.SetActive(false);
        soundPanel.SetActive(false);
        backgroundPanel.SetActive(true);
        motionButton.image.color = new Color32(255, 255, 255, 255);
        soundButton.image.color = new Color32(255, 255, 255, 255);
        backgroundButton.image.color = new Color32(200, 200, 200, 255);

        backgroundDataMaanger.Refresh();
    }

    void InteractiveButtonEvent()
    {
        isInterActive = !isInterActive;
        KHHVideoCapture.instance.IsInteractive = isInterActive;
        interactive.SetActive(isInterActive);
        interactiveButton.image.color = isInterActive ? new Color32(200, 200, 200, 255) : new Color32(255, 255, 255, 255);
    }

    void PlayButtonEvent()
    {
        playButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
        screenEditor.Play();
    }

    public void StopButtonEvent()
    {
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        screenEditor.Stop();
    }

    void ExportButtonEvent()
    {
        if (screenEditor.FileLoaded)
            GenerateVideo();
    }

    void GenerateVideo()
    {
        isExporting = true;

        //전송 윈도우 띄우기
        exportWindow.gameObject.SetActive(true);
        exportWindow.StateChange(KHHExportWindow.ExportState.Processing);

        captureCamera.targetTexture = null;

        if (isInterActive)
            StartCoroutine(CoGenerateInteractiveVideo());
        else
            StartCoroutine(CoGenerateShortformVideo());
    }

    public void ExportVideo()
    {
        if (isInterActive)
            StartCoroutine(CoExportInteractiveVideo());
        else
            StartCoroutine(CoExportShortformVideo());
    }

    IEnumerator CoGenerateInteractiveVideo()
    {
        //first
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        //yield return StartCoroutine(screenEditor.LoadFileMotion(interactiveButtonLeft.FileName));
        screenEditor.LoadFileMotion(interactiveButtonLeft.FileName);
        while (screenEditor.FileLoaded)
            yield return null;

        //choice1
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        //yield return StartCoroutine(screenEditor.LoadFileMotion(interactiveButtonRight.FileName));
        screenEditor.LoadFileMotion(interactiveButtonLeft.FileName);
        while (screenEditor.FileLoaded)
            yield return null;

        //choice2
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        captureCamera.targetTexture = captureRenderTexture;
        exportWindow.StateChange(KHHExportWindow.ExportState.Generate);
    }

    IEnumerator CoExportInteractiveVideo()
    {
        exportWindow.StateChange(KHHExportWindow.ExportState.Exporting);

        //upload
        KHHVideoCapture.instance.UploadInteractiveVideo("test Interactive", interactiveButtonLeft.Title, interactiveButtonRight.Title);
        while (KHHVideoCapture.instance.IsUploading)
            yield return null;

        exportWindow.StateChange(KHHExportWindow.ExportState.Complete);
        //complete
        isExporting = false;
        Debug.Log("Interactive Finish");
    }

    IEnumerator CoGenerateShortformVideo()
    {
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        captureCamera.targetTexture = captureRenderTexture;
        exportWindow.StateChange(KHHExportWindow.ExportState.Generate);
    }

    IEnumerator CoExportShortformVideo()
    {
        exportWindow.StateChange(KHHExportWindow.ExportState.Exporting);
        //upload
        KHHVideoCapture.instance.UploadShortformVideo("test video");
        while (KHHVideoCapture.instance.IsUploading)
            yield return null;

        exportWindow.StateChange(KHHExportWindow.ExportState.Complete);
        //complete
        isExporting = false;
        Debug.Log("Finish");
    }
}
