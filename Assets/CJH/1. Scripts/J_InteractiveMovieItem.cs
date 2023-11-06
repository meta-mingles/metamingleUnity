using System;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class J_InteractiveMovieItem : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public Action<int> onClickInteractive;

    public TMP_Text choice1;
    public TMP_Text choice2;
    public ShortVideoInfo videoInfo;

    void Start()
    {

    }

    void Update()
    {

    }

    public void OnClickInteractiveButton(int btnIdx)
    {
        if (onClickInteractive != null)
        {
            onClickInteractive(btnIdx);
        }

        
    }
    //인터렉티브 데이터 셋팅
    public void SetItem(ShortVideoInfo Info)
    {
        videoInfo = Info;

        //choice1.text = videoInfo.interactiveMovieDTOS

    }

}
