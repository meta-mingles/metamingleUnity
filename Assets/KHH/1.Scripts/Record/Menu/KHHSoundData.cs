﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHSoundData : KHHData
{
    AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                KHHEditManager.Instance.StopButtonEvent();
                //파일 삭제
                File.Delete(KHHEditData.FileSoundPath + "/" + fileName + ".wav");
                khhDataManager.Refresh();
                KHHEditManager.Instance.screenEditor.Refresh();
            }
        }
    }
}
