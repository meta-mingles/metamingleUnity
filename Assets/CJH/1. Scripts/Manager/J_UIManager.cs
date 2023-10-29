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
        //TabŰ�� ������ ä��â�� ���̵��� �Ѵ�.
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
