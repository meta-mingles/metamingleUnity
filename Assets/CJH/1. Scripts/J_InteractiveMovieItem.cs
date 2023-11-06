using System;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class J_InteractiveMovieItem : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public Action<int> onClickInteractive;

    public TMP_Text choice;
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

        //제목
        //choice.text = videoInfo.interactiveMovieDTOS[index].choice;
    }

}
