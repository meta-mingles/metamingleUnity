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

    // Start is called before the first frame update
    void Start()
    {
        recordExitButton.onClick.AddListener(() => { });
        recordStartButton.onClick.AddListener(() => { });
        recordStopButton.onClick.AddListener(() => { });

        //movieSender.CameraPlayStart();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
