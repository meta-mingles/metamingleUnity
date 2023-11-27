using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class J_InteractiveVideoPlayer : J_VideoPlayerBase
{
    public Action<int> onClickRestart;
    public override void SetItem(ShortVideoInfo info)
    {
        base.SetItem(info);
    }
    //영상이 끝날때 선택다시하기 UI 생성
    protected override void MakeRestartUI(VideoPlayer source)
    {
        restartBt.gameObject.SetActive(true);
    }
}
