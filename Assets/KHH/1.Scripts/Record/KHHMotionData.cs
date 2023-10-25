using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KHHMotionData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI dataNameText;


    GameObject dragObject;

    public void Set(string file)
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        //복사 생성
        dragObject = Instantiate(this.gameObject);
        dragObject.transform.SetParent(this.transform.parent);
        dragObject.AddComponent<CanvasGroup>();
        dragObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        dragObject.GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        dragObject.transform.position = currentPos;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        //편집 영역에 드랍





        Destroy(dragObject);
    }
}
