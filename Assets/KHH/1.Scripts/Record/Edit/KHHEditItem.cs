using ExitGames.Demos.DemoPunVoice;
using System.Collections;
using System.Collections.Generic;
using UniHumanoid;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KHHEditItem : MonoBehaviour
{
    public bool isLoad = false;
    public KHHData.DataType type;

    protected KHHScreenEditor screenEditor;

    RectTransform rt;
    RectTransform item;

    //재생
    protected float itemCorrectTime = 0.0f;
    protected float delayTime = 0.0f;

    public float EndTime { get { return (endX + changeRightX) / lengthScale; } }

    public float curLength;
    public float maxLength;

    public float startX;
    public float endX;

    protected float changePosX;
    protected float changeLeftX;
    protected float changeRightX;

    protected float lengthScale = 10f;

    EditItemSelect left;
    EditItemSelect middle;
    EditItemSelect right;

    //KHHModelRecorder recorder;

    public void Set()
    {
        rt = GetComponent<RectTransform>();
        item = transform.Find("Item").GetComponent<RectTransform>();
        left = item.Find("Left").GetComponent<EditItemSelect>();
        middle = item.Find("Middle").GetComponent<EditItemSelect>();
        right = item.Find("Right").GetComponent<EditItemSelect>();

        curLength = endX - startX;

        changePosX = 0;
        changeLeftX = 0;
        changeRightX = 0;

        rt.sizeDelta = new Vector2(curLength, 60);
        item.sizeDelta = new Vector2(curLength, 60);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        isLoad = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isLoad) return;

        //크기조절
        if (left.isDrag)
        {
            changeLeftX += left.posXDiff;
            if (maxLength > 0 && changeLeftX < 0)
            {
                changeLeftX = 0;
            }
            else if (startX + changeLeftX > endX + changeRightX)
            {
                changeLeftX = (endX + changeRightX) - startX;
            }

            curLength = (endX + changeRightX) - (startX + changeLeftX);
            item.sizeDelta = new Vector2(curLength, item.sizeDelta.y);
            item.anchoredPosition = new Vector2(startX + changePosX + changeLeftX, item.anchoredPosition.y);
        }

        if (middle.isDrag)
        {
            changePosX += middle.posXDiff;
            if (startX + changePosX + changeLeftX < 0)
            {
                changePosX = (startX + changeLeftX) * -1;
            }

            item.anchoredPosition = new Vector2(startX + changePosX + changeLeftX, item.anchoredPosition.y);
        }

        if (right.isDrag)
        {
            changeRightX += right.posXDiff;
            if (maxLength > 0 && changeRightX > 0)
            {
                changeRightX = 0;
            }
            else if (endX + changeRightX < startX + changeLeftX)
            {
                changeRightX = (startX + changeLeftX) - endX;
            }

            curLength = (endX + changeRightX) - (startX + changeLeftX);
            item.sizeDelta = new Vector2(curLength, item.sizeDelta.y);
        }
    }

    public virtual void PlayStart()
    {
        itemCorrectTime = changeLeftX / lengthScale;
        delayTime = (changePosX + changeLeftX) / lengthScale;
    }

    public virtual void PlayStop()
    {

    }

    public virtual void PlayEnd()
    {

    }

    public virtual void LoadItemData(KHHScreenEditor editor, string fileName, UnityAction action)
    {
        screenEditor = editor;
    }

    //public virtual void LoadRecordData(KHHScreenEditor editor, string fileName, UnityAction action)
    //{
    //    screenEditor = editor;
    //}

    protected virtual void EditTimeLeft()
    {

    }

    protected virtual void EditTimeRight()
    {

    }

    protected virtual void EditTimeMove()
    {

    }
}
