using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem
{
    public string name;
    public Vector2Int gridPosition;
    public Sprite image;
    public string uniqueId;

    public InventoryItem(string _name, Vector2Int _gridPosition, Sprite _image, string _uniqueId)
    {
        name = _name;
        gridPosition = _gridPosition;
        image = _image;
        uniqueId = _uniqueId;
    }
}
