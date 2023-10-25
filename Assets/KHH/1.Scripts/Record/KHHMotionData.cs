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
        //���� ����
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
        //��ũ�� ������ ���� �ȿ� ����ߴ��� Ȯ��
        if (eventData.pointerCurrentRaycast.gameObject != null)
        {
            //��� �������� ���� ������ ����Ǿ��� �� ȣ��
            KHHScreenEditor screenEditor = eventData.pointerCurrentRaycast.gameObject.GetComponent<KHHScreenEditor>();
            if (screenEditor != null)
            {
                screenEditor.OnDropItem(this.gameObject);
            }
        }





        Destroy(dragObject);
    }
}
