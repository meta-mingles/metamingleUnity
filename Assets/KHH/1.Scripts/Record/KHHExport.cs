using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHExport : MonoBehaviour
{
    public enum ExportState
    {
        None,
        Exporting,
        Uploading,
        Complete,
    }
    public ExportState exportState = ExportState.None;

    public Button backButton;
    public Button uploadButton;

    [Header("Interactive")]
    public Button interactiveButton;
    public GameObject interactive;
    public KHHInteractiveButton interactiveButtonLeft;
    public KHHInteractiveButton interactiveButtonRight;

    Outline interacitveOutline;

    public KHHScreenEditor screenEditor;
    //complete
    public TMP_InputField titleInputField;
    public TMP_InputField descriptionInputField;

    bool isInterActive = false;

    public KHHVideoDataManager videoDataManager;

    // Start is called before the first frame update
    void Awake()
    {
        if (backButton != null) backButton.onClick.AddListener(() => { gameObject.SetActive(false); });  //에디터로 돌아가기
        if (uploadButton != null) uploadButton.onClick.AddListener(ExportButtonEvent);
        if (interactiveButton != null) interactiveButton.onClick.AddListener(InteractiveButtonEvent);

        interacitveOutline = interactiveButton.GetComponent<Outline>();

        VideoCaptureCtrl.instance.Set();
    }

    //private void Update()
    //{
    //    if (exportState != ExportState.Exporting && uploadWaitCG.alpha == 1)
    //    {
    //        uploadButton.interactable = false;
    //    }
    //}

    public void Open()
    {
        gameObject.SetActive(true);
        videoDataManager.Refresh();
        GenerateVideo();
    }

    void GenerateVideo()
    {
        exportState = ExportState.Exporting;
        uploadButton.interactable = false;

        StartCoroutine(CoGenerateVideo());
    }

    IEnumerator CoGenerateVideo()
    {
        screenEditor.Play();
        VideoCaptureCtrl.instance.StartCapture();

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        //captureCamera.targetTexture = captureRenderTexture;
        exportState = ExportState.None;
        uploadButton.interactable = true;
    }

    void ExportButtonEvent()
    {
        if (exportState != ExportState.None)
            return;

        KHHCanvasShield.Instance.Show();
        if (isInterActive)
            StartCoroutine(CoExportInteractiveVideo());
        else
            StartCoroutine(CoExportShortformVideo());
    }

    void InteractiveButtonEvent()
    {
        isInterActive = !isInterActive;
        interactive.SetActive(isInterActive);
        interacitveOutline.enabled = isInterActive;
        //interactiveButton.image.color = isInterActive ? new Color32(200, 200, 200, 255) : new Color32(255, 255, 255, 255);
    }

    IEnumerator CoExportInteractiveVideo()
    {
        exportState = ExportState.Uploading;

        //upload
        KHHVideoCapture.instance.UploadInteractiveVideo(titleInputField.text, descriptionInputField.text, interactiveButtonLeft.Title, interactiveButtonLeft.FilePath, interactiveButtonRight.Title, interactiveButtonRight.FilePath);
        while (KHHVideoCapture.instance.IsUploading)
            yield return null;

        exportState = ExportState.Complete;
        KHHCanvasShield.Instance.Close();
        Debug.Log("Interactive Finish");
    }

    IEnumerator CoExportShortformVideo()
    {
        exportState = ExportState.Uploading;
        //upload
        KHHVideoCapture.instance.UploadShortformVideo(titleInputField.text, descriptionInputField.text);
        while (KHHVideoCapture.instance.IsUploading)
            yield return null;

        exportState = ExportState.Complete;
        KHHCanvasShield.Instance.Close();
        Debug.Log("Finish");
    }
}
