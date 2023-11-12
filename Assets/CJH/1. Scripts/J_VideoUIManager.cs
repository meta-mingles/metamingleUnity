using System.Collections;
using System.Collections.Generic;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_VideoUIManager : MonoBehaviour
{
    public Button homeBt; //광장으로 돌아가기 버튼
    string platformSceneName = "Main_Platform";
    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private void OnValidate()
    {
      
        if (homeBt.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddStringPersistentListener(homeBt.onClick, SceneChange, platformSceneName);
    }
}
