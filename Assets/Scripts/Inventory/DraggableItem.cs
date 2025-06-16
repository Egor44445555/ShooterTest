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
    Vector2Int prevPoint;

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
        prevPoint = item.lastPoint;

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
            Item itemSlot = result.gameObject.GetComponent<Item>();

            if (slot != null)
            {
                targetSlot = slot;
                break;
            }
        }

        if (targetSlot != null)
        {
            if (Inventory.main != null && Inventory.main.CanPlaceItem(GetComponent<Item>(), targetSlot.gridPosition))
            {
                GetComponent<Item>().lastPoint = targetSlot.gridPosition;
                targetSlot.isOccupied = true;
                targetSlot.currentItem = GetComponent<Item>();
                originalPosition = rectTransform.anchoredPosition;
                Inventory.main.PlaceItem(GetComponent<Item>(), targetSlot.gridPosition, prevPoint);
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
                if (GetComponent<Item>().occupiedSlots.Length > 0)
                {
                    Inventory.main.removePosition = GetComponent<Item>().occupiedSlots[0].gridPosition;
                }
                
                Inventory.main.removeItemInventory = gameObject;                
                Inventory.main.removeInventoryItemsPopup.SetActive(true);
            }
        }
    }
}