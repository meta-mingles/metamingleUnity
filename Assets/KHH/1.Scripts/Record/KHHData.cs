using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class KHHData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    GameObject dragObject;

    public enum DataType
    {
        None,
        MotionData,
        SoundData,
        BackgroundData,
        CaptionData,
    }
    public DataType type;

    protected string fileName;
    public string FileName { get { return fileName; } }

    public virtual void Set(string fileName)
    {
        this.fileName = fileName;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //복사 생성
        dragObject = Instantiate(this.gameObject);
        dragObject.transform.SetParent(this.transform.parent);
        dragObject.AddComponent<CanvasGroup>();
        dragObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        dragObject.GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        dragObject.transform.position = currentPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //스크린 에디터 영역 안에 드롭했는지 확인
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //드롭 아이템이 편집 영역에 드랍되었을 때 호출
            KHHScreenEditor screenEditor = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<KHHScreenEditor>();
            if (screenEditor != null)
            {
                screenEditor.OnDropItem(this.gameObject);
            }

            //드롭 아이템이 Interactive버튼 위에 드랍되었을 때 호출
            KHHInteractiveButton interactiveButton = eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<KHHInteractiveButton>();
            if (interactiveButton != null)
            {
                interactiveButton.OnDropItem(this.gameObject);
            }
        }

        Destroy(dragObject);
    }
}
