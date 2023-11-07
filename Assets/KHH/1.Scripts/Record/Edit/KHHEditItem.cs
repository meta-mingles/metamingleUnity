using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KHHEditItem : Selectable
{
    public bool isLoad = false;
    public KHHData.DataType type;

    protected KHHScreenEditor screenEditor;

    protected bool isSelected = false;
    public bool IsSelected { get { return isSelected; } set { isSelected = value; outline.enabled = value; } }
    RectTransform rt;
    RectTransform item;
    protected Outline outline;

    //재생
    protected float itemCorrectTime = 0.0f;
    protected float delayTime = 0.0f;

    public float EndTime { get { return (endX + changeRightX) / lengthScale; } }

    protected float curLength;
    protected float maxLength;

    protected float startX;
    protected float endX;

    protected float changePosX = 0;
    protected float changeLeftX = 0;
    protected float changeRightX = 0;

    protected float lengthScale = 10f;

    EditItemSelect left;
    EditItemSelect middle;
    EditItemSelect right;

    public TextMeshProUGUI nameText;

    //KHHModelRecorder recorder;
    protected override void Awake()
    {
        base.Awake();
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
    }

    public void Init(float cpx = 0f, float clx = 0f, float crx = 0f)
    {
        rt = GetComponent<RectTransform>();
        item = transform.Find("Item").GetComponent<RectTransform>();
        left = item.Find("Left").GetComponent<EditItemSelect>();
        middle = item.Find("Middle").GetComponent<EditItemSelect>();
        right = item.Find("Right").GetComponent<EditItemSelect>();

        left.OnDragEndEvent += DragEndLeft;
        middle.OnDragEndEvent += DragEndMiddle;
        right.OnDragEndEvent += DragEndRight;

        changePosX = cpx;
        changeLeftX = clx;
        changeRightX = crx;
    }

    protected void Set()
    {
        curLength = (endX + changeRightX) - (startX + changeLeftX);

        item.sizeDelta = new Vector2(curLength, 60);
        item.anchoredPosition = new Vector2(startX + changePosX + changeLeftX, item.anchoredPosition.y);
        rt.sizeDelta = new Vector2(startX + changePosX + changeLeftX + curLength, 60);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        isLoad = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isLoad) return;

        //크기조절
        if (left.isDrag)
        {
            changeLeftX += left.posXDiff;
            if (maxLength > 0 && changeLeftX < 0)
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
            rt.sizeDelta = new Vector2(startX + changePosX + changeLeftX + curLength, 60);
        }

        if (middle.isDrag)
        {
            changePosX += middle.posXDiff;
            if (startX + changePosX + changeLeftX < 0)
            {
                changePosX = (startX + changeLeftX) * -1;
            }

            item.anchoredPosition = new Vector2(startX + changePosX + changeLeftX, item.anchoredPosition.y);
            rt.sizeDelta = new Vector2(startX + changePosX + changeLeftX + curLength, 60);
        }

        if (right.isDrag)
        {
            changeRightX += right.posXDiff;
            if (maxLength > 0 && changeRightX > 0)
            {
                changeRightX = 0;
            }
            else if (endX + changeRightX < startX + changeLeftX)
            {
                changeRightX = (startX + changeLeftX) - endX;
            }

            curLength = (endX + changeRightX) - (startX + changeLeftX);
            item.sizeDelta = new Vector2(curLength, item.sizeDelta.y);
            rt.sizeDelta = new Vector2(startX + changePosX + changeLeftX + curLength, 60);
        }
    }

    public virtual void PlayStart()
    {
        itemCorrectTime = changeLeftX / lengthScale;
        delayTime = (changePosX + changeLeftX) / lengthScale;
    }

    public virtual void PlayStop()
    {

    }

    public virtual void PlayEnd()
    {

    }

    public virtual void Remove()
    {

    }

    public virtual void LoadItemData(KHHScreenEditor editor, string filePath, string fileName, UnityAction action)
    {
        screenEditor = editor;
        nameText.text = fileName;
    }

    protected virtual void DragEndLeft()
    {
    }

    protected virtual void DragEndMiddle()
    {
    }

    protected virtual void DragEndRight()
    {
    }

    public override void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        outline.enabled = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        outline.enabled = false;
    }
}
