using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHSoundData : KHHData
{
    AudioSource audioSource;
    public AudioSource AudioSource { get { return audioSource; } }
    AudioClip audioClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Set(string fileName)
    {
        base.Set(fileName);
    }
}
