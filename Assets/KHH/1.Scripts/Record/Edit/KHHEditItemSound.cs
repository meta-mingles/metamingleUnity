using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Events;

public class KHHEditItemSound : KHHEditItem
{
    bool isVoice = false;
    public bool IsVoice { set { isVoice = value; } }
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (audioSource != null && screenEditor.IsPlaying)
        {
            float actionTime = screenEditor.playTime - delayTime;
            if (actionTime > EndTime)
            {
                audioSource.Stop();
                return;
            }
        }
    }

    public override void PlayStart()
    {
        base.PlayStart();
        audioSource.time = itemCorrectTime;
        audioSource.PlayDelayed(delayTime);
    }

    public override void PlayStop()
    {
        base.PlayStop();
        audioSource.Stop();
    }

    public override void PlayEnd()
    {
        base.PlayEnd();
        audioSource.Stop();
    }

    public override void LoadItemData(KHHScreenEditor editor, string filePath, UnityAction action)
    {
        base.LoadItemData(editor, filePath, action);

        //오디오 로드
        StartCoroutine(CoLoadAudioData(filePath, action));
    }

    IEnumerator CoLoadAudioData(string filePath, UnityAction action)
    {
        yield return StartCoroutine(SaveLoadWav.Load(filePath, audioSource));

        startX = 0f;
        endX = audioSource.clip.length * lengthScale;
        maxLength = endX;

        action?.Invoke();
        Set();
    }

    protected override void DragEndLeft()
    {
        if (isVoice) KHHEditVideoState.MotionVChangeLeftX = changeLeftX;
        else KHHEditVideoState.SoundChangeLeftX = changeLeftX;
    }

    protected override void DragEndMiddle()
    {
        if (isVoice) KHHEditVideoState.MotionVChangeX = changePosX;
        else KHHEditVideoState.SoundChangeX = changePosX;
    }

    protected override void DragEndRight()
    {
        if (isVoice) KHHEditVideoState.MotionVChangeRightX = changeRightX;
        else KHHEditVideoState.SoundChangeRightX = changeRightX;
    }
}
