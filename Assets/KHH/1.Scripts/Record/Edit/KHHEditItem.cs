using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KHHEditItem : MonoBehaviour
{
    public KHHData.DataType type;

    KHHScreenEditor screenEditor;

    RectTransform rt;
    RectTransform item;

    //Àç»ý
    protected float playTime = 0.0f;
    protected int curIdx = 0;

    protected List<float> timeList;
    public List<float> TimeList { get { return timeList; } }

    public float curlength;
    public float maxLength;
    public float endX;

    float scale = 1;

    EditItemSelect left;
    EditItemSelect middle;
    EditItemSelect right;

    KHHModelRecorder recorder;

    public virtual void Set(KHHModelRecorder recorder)
    {
        rt = GetComponent<RectTransform>();
        item = transform.Find("Item").GetComponent<RectTransform>();
        left = item.Find("Left").GetComponent<EditItemSelect>();
        middle = item.Find("Middle").GetComponent<EditItemSelect>();
        right = item.Find("Right").GetComponent<EditItemSelect>();

        this.recorder = recorder;
        maxLength = recorder.TimeList[TimeList.Count - 1] - recorder.TimeList[0];
        curlength = maxLength;
        rt.sizeDelta = new Vector2(maxLength * 10, 60);
        item.sizeDelta = new Vector2(maxLength * 10, 60);

        endX = recorder.TimeList[TimeList.Count - 1] * 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (recorder == null) return;

        if (left.isDrag)
        {
            if (item.localPosition.x + left.posXDiff < 0)
            {
                item.localPosition = new Vector3(0, item.localPosition.y, item.localPosition.z);
            }
            else if (item.sizeDelta.x - left.posXDiff > maxLength)
            {
                item.sizeDelta = new Vector2(maxLength, item.sizeDelta.y);
            }
            else
            {
                item.localPosition = new Vector3(item.localPosition.x+ left.posXDiff, item.localPosition.y, item.localPosition.z);
                item.sizeDelta = new Vector2(item.sizeDelta.x - left.posXDiff, item.sizeDelta.y);
            }
        }

        if (middle.isDrag)
        {
            item.localPosition =
                new Vector3(item.localPosition.x + middle.posXDiff < 0 ? 0 : item.localPosition.x + middle.posXDiff, item.localPosition.y, item.localPosition.z);
        }

        if (right.isDrag)
        {
            //if (right.posXDiff -  < 0)
            //{

            //}
            //else


                item.sizeDelta = new Vector2(item.sizeDelta.x + right.posXDiff, item.sizeDelta.y);
        }

        curlength = item.sizeDelta.x;
    }

    public void StartPlay()
    {
        playTime = 0.0f;
        curIdx = 0;
    }

    public void StopPlay()
    {
    }

    public virtual void LoadRecordData(KHHScreenEditor screenEditor, string fileName)
    {
        this.screenEditor = screenEditor;
    }

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
