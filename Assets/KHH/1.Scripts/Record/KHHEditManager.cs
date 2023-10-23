using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KHHEditManager : MonoBehaviour
{
    public GameObject record;
    public GameObject edit;

    [Header("Left")]
    public Button recordButton;
    public Button videoButton;
    public Button soundButton;
    public Button backgroundButton;
    public Button interactiveButton;

    public GameObject videoPanel;
    public GameObject soundPanel;
    public GameObject backgroundPanel;

    [Space]
    public Button captureButton;

    // Start is called before the first frame update
    void Start()
    {
        if (recordButton != null) recordButton.onClick.AddListener(() => { record.SetActive(true); edit.SetActive(false); });
        if (videoButton != null) videoButton.onClick.AddListener(() => { videoPanel.SetActive(true); soundPanel.SetActive(false); backgroundPanel.SetActive(false); });
        if (soundButton != null) soundButton.onClick.AddListener(() => { videoPanel.SetActive(false); soundPanel.SetActive(true); backgroundPanel.SetActive(false); });
        if (backgroundButton != null) backgroundButton.onClick.AddListener(() => { videoPanel.SetActive(false); soundPanel.SetActive(false); backgroundPanel.SetActive(true); });
        if (interactiveButton != null) interactiveButton.onClick.AddListener(() => { });

        if (captureButton != null) captureButton.onClick.AddListener(() => { });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
