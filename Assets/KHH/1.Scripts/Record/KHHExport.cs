using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
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
    public Button shortformButton;
    public GameObject shortformOff;
    public GameObject shortformOn;
    public Button interactiveButton;
    public GameObject interactiveOff;
    public GameObject interactiveOn;
    public GameObject interactive;
    public KHHInteractiveButton interactiveButtonLeft;
    public KHHInteractiveButton interactiveButtonRight;

    public KHHScreenEditor screenEditor;
    public RawImage screen;
    public Camera captureCamera;
    //complete
    public TMP_InputField titleInputField;
    public TMP_InputField descriptionInputField;

    bool isInterActive = false;

    public KHHVideoDataManager videoDataManager;
    public RockVR.Video.VideoCapture videoCapture;

    [Header("Upload")]
    [SerializeField] KHHExportUpload upload;

    // Start is called before the first frame update
    void Awake()
    {
        if (backButton != null) backButton.onClick.AddListener(() => { gameObject.SetActive(false); });  //에디터로 돌아가기
        if (uploadButton != null) uploadButton.onClick.AddListener(UploadButtonEvent);
        if (shortformButton != null) shortformButton.onClick.AddListener(ShortformButtonEvent);
        if (interactiveButton != null) interactiveButton.onClick.AddListener(InteractiveButtonEvent);
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
        if (!screenEditor.Play()) return;

        exportState = ExportState.Exporting;
        uploadButton.interactable = false;

        StartCoroutine(CoGenerateVideo());
    }

    IEnumerator CoGenerateVideo()
    {
        screen.color = Color.white;
        screen.texture = videoCapture.FrameRenderTexture;
        VideoCaptureCtrl.instance.StartCapture();   //녹화 시작
        yield return null;

        //썸네일 생성
        RenderTexture rt = captureCamera.targetTexture;
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();
        string path = KHHEditData.FilePath + "/thumbnail.png";
        System.IO.File.WriteAllBytes(path, bytes);

        while (screenEditor.IsPlaying)
            yield return null;

        screenEditor.Stop();
        VideoCaptureCtrl.instance.StopCapture();

        while (VideoCaptureCtrl.instance.status != VideoCaptureCtrl.StatusType.FINISH)
            yield return null;

        screen.color = new Color32(56, 60, 62, 255);
        screen.texture = null;
        exportState = ExportState.None;
        uploadButton.interactable = true;
    }

    void UploadButtonEvent()
    {
        if (exportState != ExportState.None)
            return;

        upload.Open(isInterActive ? UploadInteractiveVideo : UploadShortformVideo);
    }

    void ShortformButtonEvent()
    {
        isInterActive = false;
        shortformOff.SetActive(false);
        shortformOn.SetActive(true);
        interactiveOff.SetActive(true);
        interactiveOn.SetActive(false);
        interactive.SetActive(false);
    }

    void InteractiveButtonEvent()
    {
        isInterActive = true;
        shortformOff.SetActive(true);
        shortformOn.SetActive(false);
        interactiveOff.SetActive(false);
        interactiveOn.SetActive(true);
        interactive.SetActive(true);
    }


    void UploadInteractiveVideo()
    {
        StartCoroutine(CoUploadInteractiveVideo());
    }

    IEnumerator CoUploadInteractiveVideo()
    {
        exportState = ExportState.Uploading;

        //upload
        KHHVideoCapture.instance.UploadInteractiveVideo(titleInputField.text, descriptionInputField.text, interactiveButtonLeft.Title, interactiveButtonLeft.FilePath, interactiveButtonRight.Title, interactiveButtonRight.FilePath);
        while (KHHVideoCapture.instance.IsUploading)
            yield return null;

        exportState = ExportState.Complete;
        upload.Complete();
        Debug.Log("Interactive Finish");
    }

    void UploadShortformVideo()
    {
        StartCoroutine(CoUploadShortformVideo());
    }

    IEnumerator CoUploadShortformVideo()
    {
        exportState = ExportState.Uploading;
        //upload
        KHHVideoCapture.instance.UploadShortformVideo(titleInputField.text, descriptionInputField.text);
        while (KHHVideoCapture.instance.IsUploading)
            yield return null;

        exportState = ExportState.Complete;
        upload.Complete();
        Debug.Log("Finish");
    }
}
