﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditItemSelect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDrag = false;

    protected float recentPosX;
    public float posXDiff;

    private void Update()
    {
        if (isDrag)
        {
            posXDiff = Input.mousePosition.x - recentPosX;
            recentPosX =Input.mousePosition.x;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        recentPosX = eventData.position.x;
        Debug.Log("BeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        Debug.Log("EndDrag");
    }
}
