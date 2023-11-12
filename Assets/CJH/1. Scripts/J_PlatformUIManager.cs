using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.Events;

public class J_PlatformUIManager : MonoBehaviour
{
    public static J_PlatformUIManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    [Header("Platform")]
    public Button videoBt; //영상보러가기 버튼
    public Button customizeBt; //아바타 커스터마이징 가기 버튼
    public Button chatBt; //채팅 버튼
    public Button settingBt; //설정 버튼

    string videoSceneName = "VideoScene";
    string customizationSceneName = "Customization";

    public void Update()
    {

    }

    public void SceneChange(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    private void OnValidate()
    {
        if (videoBt.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddStringPersistentListener(videoBt.onClick, SceneChange, videoSceneName);
        if (customizeBt.onClick.GetPersistentEventCount() == 0)
            UnityEventTools.AddStringPersistentListener(customizeBt.onClick, SceneChange, customizationSceneName);
      
    }
}
