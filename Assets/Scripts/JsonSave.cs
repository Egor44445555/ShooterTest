using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.Runtime.InteropServices;

public class JsonSave : MonoBehaviour
{
    public static JsonSave main;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        PlayerSaveData playerSaveData = LoadPlayerData();
        List<InventoryItem> newInventoryItems = new List<InventoryItem>();

        if (playerSaveData != null)
        {
            foreach (InventoryItem item in playerSaveData.inventoryItem)
            {
                newInventoryItems.Add(new InventoryItem(item.name, item.gridPosition, item.image, item.uniqueId));
            }
        }

        SavePlayerData(newInventoryItems.ToArray());
    }

    public void ResetFilePlayerSaveData()
    {
        PlayerSaveData playerSaveData = LoadPlayerData();
        List<InventoryItem> newInventoryItems = new List<InventoryItem>();

        SavePlayerData(newInventoryItems.ToArray());
    }

    public void SavePlayerData(InventoryItem[] inventoryItem)
    {
        PlayerSaveData wrapper = new PlayerSaveData { inventoryItem = inventoryItem };
        string json = JsonUtility.ToJson(wrapper);
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");

        File.WriteAllText(path, json);
    }

    public PlayerSaveData LoadPlayerData()
    {
        string path = Path.Combine(Application.persistentDataPath, "playerData.json");

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerSaveData data = JsonUtility.FromJson<PlayerSaveData>(json);

            return data != null ? data : null;
        }
        else  
        {
            return null;
        }   
    }
}