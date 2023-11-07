using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class KHHEditItemBackground : KHHEditItem
{
    Texture2D texture;

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
                Remove();
            }
        }
    }

    public override void PlayStart()
    {
        base.PlayStart();
        screenEditor.backgroundImage.texture = texture;
    }

    public override void PlayStop()
    {
        base.PlayStop();
        screenEditor.backgroundImage.texture = null;
    }

    public override void PlayEnd()
    {
        base.PlayEnd();
        screenEditor.backgroundImage.texture = null;
    }

    public override void Remove()
    {
        PlayerPrefs.DeleteKey($"{KHHVideoData.VideoName}I");
        PlayerPrefs.DeleteKey($"{KHHVideoData.VideoName}ICX");
        PlayerPrefs.DeleteKey($"{KHHVideoData.VideoName}ICLX");
        PlayerPrefs.DeleteKey($"{KHHVideoData.VideoName}ICRX");
        screenEditor.EditItemList.Remove(screenEditor.EditItemList.Find(x => x == this));
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
