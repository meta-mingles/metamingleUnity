using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class KHHEditManager : MonoBehaviour
{
    public static KHHEditManager Instance;

    public GameObject record;
    public GameObject edit;

    public Camera captureCamera;
    public KHHScreenEditor screenEditor;
    public GameObject barracudaRunner;

    [Header("Left")]
    public Button recordButton;
    public Button videoButton;
    public Button soundButton;
    public Button backgroundButton;
    public Button interactiveButton;

    public GameObject videoPanel;
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
        if (recordButton != null) recordButton.onClick.AddListener(() =>
        {
            //전환이 가능한 상태
            if (KHHRecordManager.Instance.Init())
            {
                record.SetActive(true);
                edit.SetActive(false);
                captureCamera.targetTexture = null;
                barracudaRunner.SetActive(true);
            }
        });
        if (videoButton != null) videoButton.onClick.AddListener(() => { videoPanel.SetActive(true); soundPanel.SetActive(false); backgroundPanel.SetActive(false); });
        if (soundButton != null) soundButton.onClick.AddListener(() => { videoPanel.SetActive(false); soundPanel.SetActive(true); backgroundPanel.SetActive(false); });
        if (backgroundButton != null) backgroundButton.onClick.AddListener(() => { videoPanel.SetActive(false); soundPanel.SetActive(false); backgroundPanel.SetActive(true); });
        if (interactiveButton != null) interactiveButton.onClick.AddListener(Interactive);

        if (playButton != null) playButton.onClick.AddListener(() => { screenEditor.Play(); });
        if (stopButton != null) stopButton.onClick.AddListener(() => { screenEditor.Stop(); });
        if (exportButton != null) exportButton.onClick.AddListener(() => { if (screenEditor.FileLoaded) GenerateVideo(); });
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void Interactive()
    {
        isInterActive = !isInterActive;
        KHHVideoCapture.instance.IsInteractive = isInterActive;
        interactive.SetActive(isInterActive);
    }

    void GenerateVideo()
    {
        isExporting = true;

        //전송 윈도우 띄우기
        exportWindow.gameObject.SetActive(true);
        exportWindow.StateChange(KHHExportWindow.ExportState.Processing);

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

        yield return StartCoroutine(screenEditor.LoadFile(interactiveButtonLeft.FileName));

        //choice1
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        yield return StartCoroutine(screenEditor.LoadFile(interactiveButtonRight.FileName));

        //choice2
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

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
