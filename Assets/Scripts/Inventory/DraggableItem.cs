using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector] public Item item;

    Canvas canvas;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;
    Vector2 pointerOffset;
    Vector2? lastTouchPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        item = GetComponent<Item>();
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Inventory.main.lastItem = item;
        GetComponent<Item>().basePoint = rectTransform.localPosition;

        originalPosition = rectTransform.anchoredPosition;

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPos
        );

        pointerOffset = (Vector2)rectTransform.localPosition - localPointerPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPointerPosition
        );

        rectTransform.localPosition = localPointerPosition + pointerOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        InventorySlot targetSlot = null;

        foreach (var result in eventData.hovered)
        {
            InventorySlot slot = result.gameObject.GetComponent<InventorySlot>();

            if (slot != null)
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot != null)
        {
            if (Inventory.main != null && Inventory.main.CanPlaceItem(item, targetSlot.gridPosition))
            {
                item.lastPoint = targetSlot.gridPosition;
                targetSlot.isOccupied = true;
                targetSlot.currentItem = item;
                originalPosition = rectTransform.anchoredPosition;
                Inventory.main.PlaceItem(item, targetSlot.gridPosition);
            }
            else
            {
                ReturnToOriginalPosition();
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    void ReturnToOriginalPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        lastTouchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (lastTouchPosition.HasValue)
        {
            float maxDistance = 20f;

            if (Vector2.Distance(eventData.position, lastTouchPosition.Value) <= maxDistance)
            {
                Inventory.main.removePosition = GetComponent<Item>().occupiedSlots[0].gridPosition;
                Inventory.main.removeItemInventory = gameObject;                
                Inventory.main.removeInventoryItemsPopup.SetActive(true);
            }
        }
    }
}