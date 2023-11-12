using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KHHTitleManager : MonoBehaviour
{
    public Button loginButton;

    // Start is called before the first frame update
    void Start()
    {
        if (loginButton != null)
            loginButton.onClick.AddListener(() =>
            {
                GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
                GlobalValue.CurSceneName = "ToolSelect";
                SceneManager.LoadScene("ToolSelect");
            });
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
