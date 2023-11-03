using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHData : Selectable, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum DataType
    {
        None,
        MotionData,
        SoundData,
        BackgroundData,
        CaptionData,
    }
    public DataType type;

    public bool IsSelected { get; set; }
    Outline outline;

    protected KHHDataManager khhDataManager;
    GameObject dragObject;
    Transform dragParent;
    public TextMeshProUGUI dataNameText;

    protected string fileName;
    public string FileName { get { return fileName; } }
    protected string fileExtension;
    public string FileExtension { get { return fileExtension; } }

    protected override void Awake()
    {
        base.Awake();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public virtual void Set(string fileName, string fileExtension, KHHDataManager manager)
    {
        this.fileName = fileName;
        this.fileExtension = fileExtension;
        khhDataManager = manager;
        this.dragParent = manager.transform;
        dataNameText.text = fileName;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        //복사 생성
        dragObject = Instantiate(this.gameObject);
        dragObject.transform.SetParent(dragParent);
        dragObject.GetComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
        dragObject.AddComponent<CanvasGroup>();
        dragObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        dragObject.GetComponent<CanvasGroup>().alpha = 0.5f;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPos = eventData.position;
        dragObject.transform.position = currentPos;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
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
        }

        Destroy(dragObject);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        IsSelected = true;
        outline.enabled = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        IsSelected = false;
        outline.enabled = false;
    }
}
