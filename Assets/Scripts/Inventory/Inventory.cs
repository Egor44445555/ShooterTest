using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using TMPro;

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

    public void PlaceItem(Item item, Vector2Int position, Vector2Int prevPosition)
    {
        if (item.occupiedSlots != null)
        {
            foreach (InventorySlot slot in item.occupiedSlots)
            {
                slot.ClearSlot();
            }
        }

        slots[prevPosition.x, prevPosition.y].isOccupied = false;
        slots[prevPosition.x, prevPosition.y].currentItem = null;

        Vector2 size = item.GetSize();
        List<InventorySlot> slotsToOccupy = new List<InventorySlot>();

        PositionItemCorrectly(item, position, size);

        slotsToOccupy.Add(slots[position.x, position.y]);
        item.SetOccupiedSlots(slotsToOccupy);
        item.lastPoint = position;

        foreach (InventoryItem itemInventory in player.inventoryItems)
        {
            if (itemInventory.name == item.itemName)
            {
                itemInventory.gridPosition = position;
            }
        }

        for (int i = 0; i < player.inventoryItems.Count; i++)
        {
            if (player.inventoryItems[i].uniqueId == item.uniqueId)
            {
                player.inventoryItems[i] = new InventoryItem(
                    item.itemName,
                    position,
                    item.itemSprite,
                    item.uniqueId
                );
                break;
            }
        }

        JsonSave.main.SavePlayerData(player.inventoryItems.ToArray());
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
        PlayerSaveData playerSaveData = JsonSave.main.LoadPlayerData();

        if (playerSaveData != null && playerSaveData.inventoryItem != null)
        {
            foreach (Item item in FindObjectsOfType<Item>())
            {
                Destroy(item.gameObject);
            }

            foreach (InventorySlot slot in FindObjectsOfType<InventorySlot>())
            {
                if (slot.currentItem != null)
                {
                    Destroy(slot.currentItem.gameObject);
                    slot.currentItem = null;
                    slot.isOccupied = false;
                    slot.count.SetActive(false);
                }
            }

            Dictionary<Vector2, List<Item>> itemsAtPosition = new Dictionary<Vector2, List<Item>>();

            foreach (InventoryItem savedItem in playerSaveData.inventoryItem)
            {
                InventorySlot targetSlot = FindObjectsOfType<InventorySlot>()
                    .FirstOrDefault(s => s.gridPosition == savedItem.gridPosition);

                if (targetSlot != null)
                {
                    GameObject newItem = Instantiate(
                        inventoryItemsPrefab,
                        targetSlot.transform.position,
                        targetSlot.transform.rotation,
                        wrapInventory.transform
                    );

                    Item itemComponent = newItem.GetComponent<Item>();
                    itemComponent.uniqueId = savedItem.uniqueId ?? System.Guid.NewGuid().ToString();
                    itemComponent.lastPoint = savedItem.gridPosition;
                    itemComponent.name = savedItem.name;
                    itemComponent.itemName = savedItem.name;
                    itemComponent.itemSprite = savedItem.image;
                    itemComponent.GetComponent<Image>().sprite = savedItem.image;
                    itemComponent.occupiedSlots.Add(targetSlot);

                    if (!itemsAtPosition.ContainsKey(savedItem.gridPosition))
                    {
                        itemsAtPosition[savedItem.gridPosition] = new List<Item>();
                    }

                    itemsAtPosition[savedItem.gridPosition].Add(itemComponent);
                    targetSlot.isOccupied = true;
                    targetSlot.currentItem = itemComponent;

                    int itemCount = itemsAtPosition[savedItem.gridPosition].Count;
                    TextMeshProUGUI countSlot = targetSlot.count.GetComponent<TextMeshProUGUI>();

                    targetSlot.count.SetActive(itemCount > 1);
                    countSlot.text = itemCount.ToString();
                }
            }

            FindObjectOfType<Player>().inventoryItems = playerSaveData.inventoryItem
                .Select(item => new InventoryItem(
                    item.name, 
                    item.gridPosition, 
                    item.image, 
                    item.uniqueId
                )).ToList();
        }
    }

    public void RemoveItem()
    {
        InventoryItem removeItem = Array.Find<InventoryItem>(FindObjectOfType<Player>().inventoryItems.ToArray(), item => item.name == removeItemInventory.GetComponent<Item>().itemName);
        FindObjectOfType<Player>().inventoryItems.Remove(removeItem);

        Item item = removeItemInventory.GetComponent<Item>();
        TextMeshProUGUI countSlot = item.occupiedSlots[0].count.GetComponent<TextMeshProUGUI>();
        int itemCount = int.Parse(countSlot.text) - 1;
        item.occupiedSlots[0].count.SetActive(itemCount > 1);
        countSlot.text = itemCount.ToString();

        Destroy(removeItemInventory);
        JsonSave.main.SavePlayerData(FindObjectOfType<Player>().inventoryItems.ToArray());
    }
}