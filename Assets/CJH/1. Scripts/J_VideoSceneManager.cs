using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class J_VideoSceneManager : MonoBehaviour
{
    public Button homeBt; //광장으로 돌아가기 버튼
    string platformSceneName = "Main_Platform";
    private void Awake()
    {
        //// J_SoundManager 스크립트의 runInVideoScene 변수를 false로 설정
        //J_SoundManager soundManager = FindObjectOfType<J_SoundManager>();
        //if (soundManager != null)
        //{
        //    soundManager.isPlaying = false;
        //    soundManager.gameObject.SetActive(false);
        //}
        //브금 끄기
        SoundManager.instance.BGMVolume = -80;

    }
    public void SceneChange(string sceneName)
    {
        GlobalValue.PrevSceneName = SceneManager.GetActiveScene().name;
        GlobalValue.CurSceneName = sceneName;
        KHHPhotonInit.instance.ReJoinRoom(GlobalValue.PrevSceneName, sceneName);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {

        if (homeBt.onClick.GetPersistentEventCount() == 0)
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(homeBt.onClick, SceneChange, platformSceneName);
    }
#endif
}
