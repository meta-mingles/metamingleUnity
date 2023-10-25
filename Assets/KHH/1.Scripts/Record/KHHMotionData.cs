using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KHHMotionData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI dataNameText;


    GameObject dragObject;

    string fileName;
    public string FileName { get { return fileName; } }

    public void Set(string fileName)
    {
        this.fileName = fileName;
        dataNameText.text = fileName;
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
        //스크린 에디터 영역 안에 드롭했는지 확인
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //드롭 아이템이 편집 영역에 드랍되었을 때 호출
            KHHScreenEditor screenEditor = eventData.pointerCurrentRaycast.gameObject.GetComponent<KHHScreenEditor>();
            if (screenEditor != null)
            {
                screenEditor.OnDropItem(this.gameObject);
            }
        }





        Destroy(dragObject);
    }
}
