using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/* 게임 내 다양한 인벤토리 관리
    Backpack, Toolbar */

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public Dictionary<string, Inventory> inventoryByName = new Dictionary<string, Inventory>();

    [Header("BackPack")]
    public Inventory backpack;
    public int backpackSlotCount;

    [Header("Toolbar")]
    public Inventory toolbar;
    public int toolbarSlotCount;
    public List<Item> startItems;

    void Awake()
    {
        if (!instance)
            instance = this;

        backpack = new Inventory(backpackSlotCount);
        toolbar = new Inventory(toolbarSlotCount);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Toolbar", toolbar);
        
    }

    public void startItemAdd(string inventoryName)
    {
        foreach (var item in startItems)
        {
            inventoryByName[inventoryName].Add(item);
        }
    }

    public void Add(string inventoryName, Item item)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            inventoryByName[inventoryName].Add(item);
        }
    }

    public Inventory GetInventoryByName(string inventoryName)
    {
        if (inventoryByName.ContainsKey(inventoryName))
        {
            return inventoryByName[inventoryName];
        }
        return null;
    }

    public string GetInventoryName(Inventory inventory)
    {
        foreach (var entry in inventoryByName)
        {
            if (entry.Value == inventory)
            {
                return entry.Key;
            }
        }
        return null;
    }

    public void SaveInventory()
    {
        SaveData.instance.SaveInventory("Backpack", backpack);
        SaveData.instance.SaveInventory("Toolbar", toolbar);
    }

    public void LoadInventory()
    {
        SaveData.instance.LoadInventory("Backpack");
        SaveData.instance.LoadInventory("Toolbar");
    }

    public void ClearInventory()
    {
        foreach (var inventory in inventoryByName.Values)
        {
            inventory.Clear();
        }
    }
}
