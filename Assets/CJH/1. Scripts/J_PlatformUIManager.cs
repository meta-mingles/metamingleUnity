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
        SoundManager.instance.BGMVolume = 1;
        SoundManager.instance.PlayBGM("PlatformBGM");
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
        GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = sceneName;
        SceneManager.LoadScene(sceneName);
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
