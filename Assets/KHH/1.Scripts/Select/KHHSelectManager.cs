using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KHHSelectManager : MonoBehaviour
{
    public Button createButton;

    // Start is called before the first frame update
    void Start()
    {
        if (createButton != null)
            createButton.onClick.AddListener(() => { SceneManager.LoadScene("ToolCapture"); });
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
