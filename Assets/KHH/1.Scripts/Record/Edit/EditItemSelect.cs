using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditItemSelect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool isDrag = false;

    protected float recentPosX;
    public float posXDiff;

    public delegate void OnDragEndDelegate();
    public event OnDragEndDelegate OnDragEndEvent = null;

    private void Update()
    {
        if (isDrag)
        {
            posXDiff = Input.mousePosition.x - recentPosX;
            recentPosX = Input.mousePosition.x;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        recentPosX = eventData.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        OnDragEndEvent?.Invoke();
    }
}
