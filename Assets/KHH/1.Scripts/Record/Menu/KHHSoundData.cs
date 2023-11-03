using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHSoundData : KHHData
{
    AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }
    AudioClip audioClip;

    protected override void Awake()
    {
        base.Awake();
        audioClip = GetComponent<AudioClip>();
    }

    public void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                //파일 삭제
                File.Delete(KHHVideoData.FileSoundPath + "/" + fileName + ".wav");
                khhDataManager.Refresh();
            }
        }
    }
}
