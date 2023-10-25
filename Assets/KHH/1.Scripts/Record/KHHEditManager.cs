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
    public KHHModelRecorder recorder;
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

    [Header("Middle")]
    public Button playButton;
    public Button stopButton;
    public Button captureButton;

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
            if(KHHRecordManager.Instance.Init())
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
        if (interactiveButton != null) interactiveButton.onClick.AddListener(() => { });

        if (playButton != null) playButton.onClick.AddListener(() => { recorder.StartPlay(); });
        if (stopButton != null) stopButton.onClick.AddListener(() => { recorder.StopPlay(); });
        if (captureButton != null) captureButton.onClick.AddListener(() => { });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
