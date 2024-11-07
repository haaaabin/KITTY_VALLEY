using System.Collections;
using System.Collections.Generic;
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

    void Awake()
    {
        if (!instance)
            instance = this;

        backpack = new Inventory(backpackSlotCount);
        toolbar = new Inventory(toolbarSlotCount);

        inventoryByName.Add("Backpack", backpack);
        inventoryByName.Add("Toolbar", toolbar);
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

    public void SaveAllInventories()
    {
        
    }
}
