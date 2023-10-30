using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class KHHData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum DataType
    {
        None,
        MotionData,
        SoundData,
        CaptionData,
    }
    public DataType type;

    protected string fileName;
    public string FileName { get { return fileName; } }

    public abstract void OnBeginDrag(PointerEventData eventData);
    public abstract void OnDrag(PointerEventData eventData);
    public abstract void OnEndDrag(PointerEventData eventData);
}
