using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class J_UIManager : MonoBehaviour
{
    public GameObject canvasGroup;
    private bool isChatVisible = false;

    //UI Button
    public Button enterBT;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Tab키를 누르면 채팅창이 보이도록 한다.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ViewChat();
        }
    }
    void ViewChat()
    {
        isChatVisible = !isChatVisible;
        canvasGroup.SetActive(isChatVisible);
    }
    void SceneChange(int SceneNumber)
    {
        SceneManager.LoadScene(2);
    }


}
