using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Item currentItem;
    public bool isOccupied;
    public Vector2Int gridPosition;
    public GameObject count;
    
    RectTransform rectTransform;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        isOccupied = true;
    }

    public void ClearSlot()
    {
        currentItem = null;
        isOccupied = false;
        
        foreach (Item item in FindObjectsOfType<Item>())
        {
            if (item.occupiedSlots.Count > 0 && item.occupiedSlots[0].gridPosition == gridPosition)
            {
                TextMeshProUGUI countSlot = item.occupiedSlots[0].count.GetComponent<TextMeshProUGUI>();
                int itemCount = int.Parse(countSlot.text) - 1;
                countSlot.text = itemCount.ToString();
                item.occupiedSlots[0].count.SetActive(itemCount > 1);
                break;
            }
        }
    }
    
    public Vector2 GetCenterPosition()
    {
        return rectTransform.anchoredPosition + new Vector2(rectTransform.rect.width * 0.5f, rectTransform.rect.height * 0.5f);
    }
} 