using ExitGames.Demos.DemoPunVoice;
using System.Collections;
using System.Collections.Generic;
using UniHumanoid;
using UnityEngine;
using UnityEngine.Events;

public class KHHEditItem : MonoBehaviour
{
    public KHHData.DataType type;

    protected KHHScreenEditor screenEditor;

    RectTransform rt;
    RectTransform item;

    //재생
    protected float itemCorrectTime = 0.0f;
    protected float delayTime = 0.0f;

    public float EndTime { get { return (endX + changeRightX) / lengthScale; } }
    protected int curIdx = 0;
    protected int startIdx = 0;
    protected int endIdx = 0;

    protected List<float> timeList;
    public List<float> TimeList { get { return timeList; } }

    public float curLength;
    public float maxLength;

    public float startX;
    public float endX;

    float changePosX;
    float changeLeftX;
    float changeRightX;

    protected float lengthScale = 10f;

    EditItemSelect left;
    EditItemSelect middle;
    EditItemSelect right;

    //KHHModelRecorder recorder;

    public void Set()
    {
        startX = timeList[0] * lengthScale;
        endX = timeList[timeList.Count - 1] * lengthScale;
        maxLength = endX - startX;
        curIdx = 0;

        rt = GetComponent<RectTransform>();
        item = transform.Find("Item").GetComponent<RectTransform>();
        left = item.Find("Left").GetComponent<EditItemSelect>();
        middle = item.Find("Middle").GetComponent<EditItemSelect>();
        right = item.Find("Right").GetComponent<EditItemSelect>();

        curLength = maxLength;

        changePosX = 0;
        changeLeftX = 0;
        changeRightX = 0;

        rt.sizeDelta = new Vector2(maxLength, 60);
        item.sizeDelta = new Vector2(maxLength, 60);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //if (recorder == null) return;

        //크기조절
        if (left.isDrag)
        {
            changeLeftX += left.posXDiff;
            if (changeLeftX < 0)
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
            if (changeRightX > 0)
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
        delayTime = changePosX / lengthScale;

        //시작 시간 찾기
        float itemStartTime = (startX + changeLeftX) / lengthScale;
        float itemEndTime = (endX + changeRightX) / lengthScale;

        startIdx = 0;
        endIdx = timeList.Count - 1;

        for (int i = 0; i < timeList.Count; i++)
        {
            if (timeList[i] < itemStartTime)
                startIdx = i;
            if (timeList[i] < itemEndTime)
                endIdx = i;
        }

        curIdx = startIdx;
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
