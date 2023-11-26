using DG.Tweening;
using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;

public class KHHEditManager : MonoBehaviour
{
    public static KHHEditManager Instance;

    public GameObject record;
    public GameObject edit;

    public KHHMotionDataManager motionDataManager;
    public KHHSoundDataManager soundDataManager;
    public KHHBackgroundDataManager backgroundDataMaanger;
    public RockVR.Video.VideoCapture videoCapture;

    public Camera captureCamera;
    public KHHScreenEditor screenEditor;
    public RawImage screen;
    public GameObject barracudaRunner;

    [Header("Left")]
    public Button recordButton;
    public Button motionButton;
    public Button soundButton;
    public Button backgroundButton;

    public Button settingButton;
    public Button homeButton;

    public GameObject motionPanel;
    public GameObject soundPanel;
    public GameObject backgroundPanel;

    public KHHEditSetting editSetting;

    [Header("Middle")]
    public TMP_InputField titleInputField;
    public Button playButton;
    public Button stopButton;
    public Button exportButton;

    public KHHExport export;

    bool init = false;

    private void Awake()
    {
        Instance = this;
        titleInputField.text = KHHEditData.VideoTitle;
    }

    // Start is called before the first frame update
    void Start()
    {
        VideoCaptureCtrl.instance.Set();
        KHHCanvasShield.Instance.Show();

        if (recordButton != null) recordButton.onClick.AddListener(RecordButtonEvent);
        if (motionButton != null) motionButton.onClick.AddListener(MotionButtonEvent);
        if (soundButton != null) soundButton.onClick.AddListener(SoundButtonEvent);
        if (backgroundButton != null) backgroundButton.onClick.AddListener(BackgroundButtonEvent);

        if (settingButton != null) settingButton.onClick.AddListener(SettingButtonEvent);
        if (homeButton != null) homeButton.onClick.AddListener(HomeButtonEvent);

        if (titleInputField != null) titleInputField.onEndEdit.AddListener(TitleInputFieldEvent);
        if (playButton != null) playButton.onClick.AddListener(PlayButtonEvent);
        if (stopButton != null) stopButton.onClick.AddListener(StopButtonEvent);
        if (exportButton != null) exportButton.onClick.AddListener(ExportButtonEvent);

        MotionButtonEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if (!init && KHHRecordManager.Instance.videoCapture.SetEnd)
        {
            init = true;
            KHHCanvasShield.Instance.Close();
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
        foreach (var image in motionButton.GetComponentsInChildren<Image>())
            image.color = new Color32(64, 205, 182, 255);
        foreach (var image in soundButton.GetComponentsInChildren<Image>())
            image.color = Color.white;
        foreach (var image in backgroundButton.GetComponentsInChildren<Image>())
            image.color = Color.white;

        motionDataManager.Refresh();
    }

    public void SoundButtonEvent()
    {
        motionPanel.SetActive(false);
        soundPanel.SetActive(true);
        backgroundPanel.SetActive(false);
        foreach (var image in motionButton.GetComponentsInChildren<Image>())
            image.color = Color.white;
        foreach (var image in soundButton.GetComponentsInChildren<Image>())
            image.color = new Color32(64, 205, 182, 255);
        foreach (var image in backgroundButton.GetComponentsInChildren<Image>())
            image.color = Color.white;

        soundDataManager.Refresh();
    }

    public void BackgroundButtonEvent()
    {
        motionPanel.SetActive(false);
        soundPanel.SetActive(false);
        backgroundPanel.SetActive(true);
        foreach (var image in motionButton.GetComponentsInChildren<Image>())
            image.color = Color.white;
        foreach (var image in soundButton.GetComponentsInChildren<Image>())
            image.color = Color.white;
        foreach (var image in backgroundButton.GetComponentsInChildren<Image>())
            image.color = new Color32(64, 205, 182, 255);

        backgroundDataMaanger.Refresh();
    }

    void SettingButtonEvent()
    {
        editSetting.Open();
    }

    void HomeButtonEvent()
    {
        StopButtonEvent();
        KHHEditData.Close();
        GlobalValue.PrevSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = "ToolSelect";
        UnityEngine.SceneManagement.SceneManager.LoadScene("ToolSelect");
    }

    void TitleInputFieldEvent(string t)
    {
        if (!KHHEditData.ChangeVideoTitle(t))
        {
            Debug.Log("중복");
            titleInputField.text = KHHEditData.VideoTitle;
        }
    }

    void PlayButtonEvent()
    {
        if (!screenEditor.Play()) return;

        screen.color = Color.white;
        screen.texture = videoCapture.FrameRenderTexture;
        playButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(true);
    }

    public void StopButtonEvent()
    {
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(false);
        screenEditor.Stop();
        screen.texture = null;
        screen.color = new Color32(56, 60, 62, 255);
    }

    void ExportButtonEvent()
    {
        if (!screenEditor.FileLoaded)
            return;

        StopButtonEvent();
        export.Open();
    }
}
