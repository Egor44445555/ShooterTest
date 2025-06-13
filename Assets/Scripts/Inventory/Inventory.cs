using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory main;
    public float cellSize = 100f;
    public GameObject slotPrefab;
    public GameObject wrapInventory;
    public GameObject inventoryItemsPrefab;
    public GameObject removeInventoryItemsPopup;

    [HideInInspector] public Item lastItem;
    [HideInInspector] public Vector2Int removePosition;
    [HideInInspector] public GameObject removeItemInventory;

    int gridWidth = 5;
    int gridHeight = 5;
    InventorySlot[,] slots;
    Camera mainCamera;
    RectTransform inventoryRect;
    Canvas parentCanvas;
    Vector3 originalItemPosition;
    Vector3 originalItemScale;
    Player player;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        mainCamera = Camera.main;
        inventoryRect = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();
        gridWidth = player.inventorySizeWidth;
        gridHeight = player.inventorySizeHeight;

        if (mainCamera.GetComponent<Physics2DRaycaster>() == null)
        {
            mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
        }

        CreateGrid();
        CheckItems();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended)
            {
                HandleTouchEnd(touch);
            }
        }
    }

    void HandleTouchEnd(Touch touch)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryRect,
            touch.position,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out localPoint
        );

        if (lastItem != null && !IsPositionInsideInventory(localPoint))
        {
            lastItem.ClearOccupiedSlots();
            lastItem.GetComponent<RectTransform>().anchoredPosition = lastItem.basePoint;
        }

        lastItem = null;
    }

    bool IsPositionInsideInventory(Vector2 localPosition)
    {
        Rect rect = inventoryRect.rect;
        return localPosition.x >= rect.xMin &&
            localPosition.x <= rect.xMax &&
            localPosition.y >= rect.yMin &&
            localPosition.y <= rect.yMax;
    }

    public bool CanPlaceItem(Item item, Vector2Int position)
    {
        if (item != null)
        {
            Vector2 size = item.GetSize();

            if (position.x < 0 || position.y < 0 || position.x + (int)size.x > gridWidth || position.y + (int)size.y > gridHeight)
            {
                return false;
            }

            for (int x = 0; x < (int)size.x; x++)
            {
                for (int y = 0; y < (int)size.y; y++)
                {
                    int slotX = position.x + x;
                    int slotY = position.y + y;

                    if (slots[slotX, slotY].isOccupied)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void PlaceItem(Item item, Vector2Int position)
    {
        if (item.occupiedSlots != null)
        {
            foreach (InventorySlot slot in item.occupiedSlots)
            {
                slot.ClearSlot();
            }
        }

        Vector2 size = item.GetSize();
        List<InventorySlot> slotsToOccupy = new List<InventorySlot>();

        PositionItemCorrectly(item, position, size);

        slotsToOccupy.Add(slots[position.x, position.y]);
        item.SetOccupiedSlots(slotsToOccupy.ToArray());
        item.lastPoint = position;

        foreach (InventoryItem itemInventory in player.inventoryItems)
        {
            if (itemInventory.name == item.itemName)
            {
                itemInventory.gridPosition = position;
            }
        }
    }

    void PositionItemCorrectly(Item item, Vector2Int position, Vector2 size)
    {
        RectTransform itemRect = item.GetComponent<RectTransform>();

        Vector2 gridPosition = new Vector2(position.x, position.y) * cellSize;
        Vector2 slotCenter = gridPosition + new Vector2(cellSize * 0.5f, cellSize * 0.5f);
        Vector2 itemOffset = new Vector2(
            (size.x * cellSize) * (0.5f - itemRect.pivot.x),
            (size.y * cellSize) * (0.5f - itemRect.pivot.y)
        );
        Vector2 finalPosition = slotCenter + itemOffset;

        if (inventoryRect != null)
        {
            finalPosition -= new Vector2(
                inventoryRect.pivot.x * inventoryRect.rect.width,
                inventoryRect.pivot.y * inventoryRect.rect.height
            );
        }

        itemRect.anchoredPosition = finalPosition;
    }

    void CreateGrid()
    {
        slots = new InventorySlot[gridWidth, gridHeight];
        inventoryRect.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject slotObj = Instantiate(slotPrefab, transform);
                RectTransform slotRect = slotObj.GetComponent<RectTransform>();

                slotRect.anchoredPosition = new Vector2(x * cellSize + cellSize / 2, y * cellSize + cellSize / 2);
                slotRect.sizeDelta = new Vector2(cellSize, cellSize);

                InventorySlot slot = slotObj.GetComponent<InventorySlot>();
                slot.gridPosition = new Vector2Int(x, y);
                slots[x, y] = slot;
            }
        }
    }

    public void CheckItems()
    {
        foreach (InventoryItem item in FindObjectOfType<Player>().inventoryItems)
        {
            foreach (InventorySlot slot in FindObjectsOfType<InventorySlot>())
            {
                if (item.gridPosition == slot.gridPosition)
                {
                    GameObject itemObj = Instantiate(inventoryItemsPrefab, slot.transform.position, slot.transform.rotation, wrapInventory.transform);
                    Item itemInventory = itemObj.GetComponent<Item>();
                    itemInventory.name = item.name;
                    itemInventory.itemName = item.name;
                    itemInventory.itemSprite = item.image;
                    itemInventory.GetComponent<Image>().sprite = item.image;
                    slot.isOccupied = true;
                    slot.currentItem = itemInventory;
                    PlaceItem(itemInventory, slot.gridPosition);
                }
            }
        }
    }

    public void RemoveItem()
    {
        InventoryItem removeItem = Array.Find<InventoryItem>(FindObjectOfType<Player>().inventoryItems.ToArray(), item => item.gridPosition == removePosition);
        FindObjectOfType<Player>().inventoryItems.Remove(removeItem);
        Destroy(removeItemInventory);
    }
}