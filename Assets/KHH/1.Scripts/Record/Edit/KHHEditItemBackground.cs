using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class KHHEditItemBackground : KHHEditItem
{
    Texture2D texture;

    Coroutine coroutine;
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (screenEditor.backgroundImage != null && screenEditor.IsPlaying)
        {
            float actionTime = screenEditor.playTime - delayTime;
            if (actionTime > EndTime)
            {
                screenEditor.backgroundImage.texture = null;
                return;
            }
        }

        if (isSelected)
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
        if (coroutine != null) StopCoroutine(coroutine);
        screenEditor.backgroundImage.texture = texture;
    }

    public override void PlayStop()
    {
        base.PlayStop();
        coroutine = StartCoroutine(RemoveBackground());
    }

    IEnumerator RemoveBackground()
    {
        yield return new WaitForSeconds(0.05f);
        screenEditor.backgroundImage.texture = null;
        coroutine = null;
    }

    public override void Remove()
    {
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}I");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}ICX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}ICLX");
        PlayerPrefs.DeleteKey($"{KHHEditData.VideoTitle}ICRX");
        screenEditor.RemoveItem(this);
        Destroy(gameObject);
    }

    public override void LoadItemData(KHHScreenEditor editor, string filePath, string fileName, UnityAction action)
    {
        base.LoadItemData(editor, filePath, fileName, action);

        //이미지를 불러온다
        byte[] bytes = File.ReadAllBytes(filePath);
        texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        startX = 0f;
        endX = 100f;
        maxLength = -1f;

        action?.Invoke();
        Set();
    }

    protected override void DragEndLeft()
    {
        KHHEditVideoState.ImageChangeLeftX = changeLeftX;
    }

    protected override void DragEndMiddle()
    {
        KHHEditVideoState.ImageChangeX = changePosX;
    }

    protected override void DragEndRight()
    {
        KHHEditVideoState.ImageChangeRightX = changeRightX;
    }
}
