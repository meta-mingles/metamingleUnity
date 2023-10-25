using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHRecordManager : MonoBehaviour
{
    public Button recordExitButton;
    public Button recordStartButton;
    public Button recordStopButton;

    //[Header("TDPT")]
    //public MovieSender movieSender;

    public KHHModelRecorder recorder;

    //Test
    public Button loadDataButton;
    public Button testPlayButton;
    public Button testStopButton;

    // Start is called before the first frame update
    void Start()
    {
        recordExitButton.onClick.AddListener(() => { });
        recordStartButton.onClick.AddListener(() => { recorder.StartRecord(); });
        recordStopButton.onClick.AddListener(() => { recorder.StopRecord(); });

        //movieSender.CameraPlayStart();

        //Test
        loadDataButton.onClick.AddListener(() => { recorder.LoadRecordData(); });
        testPlayButton.onClick.AddListener(() => { recorder.StartPlay(); });
        testStopButton.onClick.AddListener(() => { recorder.StopPlay(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
