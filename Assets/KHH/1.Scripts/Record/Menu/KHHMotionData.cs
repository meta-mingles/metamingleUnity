using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KHHMotionData : KHHData
{
    public void Update()
    {
        if (IsSelected)
        {
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                //파일 삭제
                File.Delete(KHHVideoData.FileMotionPath + "/" + fileName + ".csv");
                File.Delete(KHHVideoData.FileMotionPath + "/" + fileName + ".wav");
                khhDataManager.Refresh();
            }
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        //드롭 아이템이 Interactive버튼 위에 드랍되었을 때 호출
        KHHInteractiveButton interactiveButton = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<KHHInteractiveButton>();
        if (interactiveButton != null)
        {
            interactiveButton.OnDropItem(this.gameObject);
        }

        base.OnEndDrag(eventData);
    }
}
