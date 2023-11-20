using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class J_PlatformUIManager : MonoBehaviour
{
    public static J_PlatformUIManager Instance;
    private void Awake()
    {
        Instance = this;

        //사운드 키기
        SoundManager.instance.BGMVolume = -5;
        SoundManager.instance.PlayBGM("PlatformBGM");

    }

    [Header("Platform")]

    public Button videoBt; //영상보러가기 버튼
    public Button customizeBt; //아바타 커스터마이징 가기 버튼
    public Button chatBt; //채팅 버튼

    [Header("Setting")]

    public Button settingBt; //설정 버튼
    public GameObject settingTab; //설정창
    public Button closeBt; //닫기 버튼

    string videoSceneName = "VideoScene";
    string customizationSceneName = "Customization";

    //씬이동 -- 비디오씬, 커스텀씬
    public void SceneChange(string sceneName)
    {
        GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = sceneName;
        SceneManager.LoadScene(sceneName);
    }

    //설정 탭 열기
    public void OpenTab()
    {
        settingTab.SetActive(true);
        Debug.Log("Tab열림");
    }
    //설정 탭 닫기
    public void CloseTab()
    {
        if (closeBt.onClick != null)
            settingTab.SetActive(false);
        Debug.Log("Tab닫힘");
    }





#if UNITY_EDITOR
    private void OnValidate()
    {
        if (videoBt != null && videoBt.onClick.GetPersistentEventCount() == 0)
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(videoBt.onClick, SceneChange, videoSceneName);
        if (customizeBt != null &&customizeBt.onClick.GetPersistentEventCount() == 0)
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(customizeBt.onClick, SceneChange, customizationSceneName);

    }
#endif
}
