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

    public void AddStartItem()
    {
        foreach (var item in startItems)
        {
            toolbar.Add(item);
        }
    }

    public void Add(Item item)
    {
        if(toolbar.Add(item))
        {
            return;
        }
        
        if(backpack.Add(item))
        {
            return;
        }
    }

    public void AddInventory(string inventoryName, Item item)
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

    public void ClearInventory()
    {
        foreach (var inventory in inventoryByName.Values)
        {
            inventory.Clear();
        }
    }
}
