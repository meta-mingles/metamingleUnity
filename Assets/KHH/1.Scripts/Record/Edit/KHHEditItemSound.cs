using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Events;

public class KHHEditItemSound : KHHEditItem
{
    AudioSource audioSource;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (audioSource!=null && screenEditor.IsPlaying)
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

    public override void LoadItemData(KHHScreenEditor editor, string fileName, UnityAction action)
    {
        base.LoadItemData(editor, fileName, action);

        //오디오 로드
        StartCoroutine(CoLoadAudioData(fileName, action));
    }

    IEnumerator CoLoadAudioData(string fileName, UnityAction action)
    {
        yield return StartCoroutine(SaveLoadWav.Load(fileName, audioSource));
        action?.Invoke();

        startX = 0f;
        endX = audioSource.clip.length * lengthScale;
        maxLength = endX;
    }
}
