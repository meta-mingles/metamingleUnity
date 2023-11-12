using DG.Tweening;
using RockVR.Video;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KHHEditManager : MonoBehaviour
{
    public static KHHEditManager Instance;

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

    public Button settingButton;
    public Button exitButton;

    public GameObject motionPanel;
    public GameObject soundPanel;
    public GameObject backgroundPanel;

    public KHHEditSetting editSetting;

    [Header("Middle")]
    public Button playButton;
    public Button stopButton;
    public Button exportButton;

    public KHHExport export;

    bool init = false;

    private void Awake()
    {
        Instance = this;
        KHHEditData.Open("test video");
    }

    // Start is called before the first frame update
    void Start()
    {
        KHHCanvasShield.Instance.Show();

        if (recordButton != null) recordButton.onClick.AddListener(RecordButtonEvent);
        if (motionButton != null) motionButton.onClick.AddListener(MotionButtonEvent);
        if (soundButton != null) soundButton.onClick.AddListener(SoundButtonEvent);
        if (backgroundButton != null) backgroundButton.onClick.AddListener(BackgroundButtonEvent);

        if (settingButton != null) settingButton.onClick.AddListener(SettingButtonEvent);

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
        motionButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(242, 189, 171, 255);
        motionButton.targetGraphic.color = new Color32(242, 189, 171, 255);
        soundButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        soundButton.targetGraphic.color = Color.white;
        backgroundButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        backgroundButton.targetGraphic.color = Color.white;

        motionDataManager.Refresh();
    }

    public void SoundButtonEvent()
    {
        motionPanel.SetActive(false);
        soundPanel.SetActive(true);
        backgroundPanel.SetActive(false);
        motionButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        motionButton.targetGraphic.color = Color.white;
        soundButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(242, 189, 171, 255);
        soundButton.targetGraphic.color = new Color32(242, 189, 171, 255);
        backgroundButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        backgroundButton.targetGraphic.color = Color.white;

        soundDataManager.Refresh();
    }

    public void BackgroundButtonEvent()
    {
        motionPanel.SetActive(false);
        soundPanel.SetActive(false);
        backgroundPanel.SetActive(true);
        motionButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        motionButton.targetGraphic.color = Color.white;
        soundButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        soundButton.targetGraphic.color = Color.white;
        backgroundButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color32(242, 189, 171, 255);
        backgroundButton.targetGraphic.color = new Color32(242, 189, 171, 255);

        backgroundDataMaanger.Refresh();
    }

    void SettingButtonEvent()
    {
        editSetting.Open();
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
        export.Open();
    }
}
