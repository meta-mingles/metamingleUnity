using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class KHHEditItemSound : KHHEditItem
{
    bool isVoice = false;
    public bool IsVoice { get { return isVoice; } set { isVoice = value; } }
    AudioSource audioSource;

    KHHEditItemMotion pairMotion;
    public KHHEditItemMotion PairMotion { set { pairMotion = value; } }

    public string FileName { get { return nameText.text; } }
    public float Volume
    {
        get { return audioSource.volume; }
        set
        {
            if (isVoice) KHHEditVideoState.MotionVVolume = value;
            else KHHEditVideoState.SoundVolume = value;
            audioSource.volume = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
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

        if (isSelected && !isVoice)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                KHHEditManager.Instance.StopButtonEvent();
                Remove();
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

    public override void Remove()
    {
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}S");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}SCX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}SCLX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}SCRX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}SV");
        screenEditor.RemoveItem(this);
        Destroy(gameObject);
    }

    public override void LoadItemData(KHHScreenEditor editor, string filePath, string fileName, UnityAction action)
    {
        base.LoadItemData(editor, filePath, fileName, action);

        //오디오 로드
        StartCoroutine(CoLoadAudioData(filePath, action));
    }

    IEnumerator CoLoadAudioData(string filePath, UnityAction action)
    {
        yield return StartCoroutine(SaveLoadWav.Load(filePath, audioSource));

        startX = 0f;
        endX = audioSource.clip.length * lengthScale;
        maxLength = endX;

        if (isVoice) audioSource.volume = KHHEditVideoState.MotionVVolume;
        else audioSource.volume = KHHEditVideoState.SoundVolume;

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

    public override void OnSelect(BaseEventData eventData)
    {
        KHHEditManager.Instance.SoundButtonEvent();
        KHHEditManager.Instance.soundDataManager.SetSelectedData(this);
        isSelected = true;
        outline.enabled = true;
        if (pairMotion != null) pairMotion.IsSelected = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        outline.enabled = false;
        if (pairMotion != null) pairMotion.IsSelected = false;
    }
}
