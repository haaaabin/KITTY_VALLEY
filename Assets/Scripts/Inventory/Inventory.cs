using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

// Inventory 클래스는 게임 내 슬롯 기반 인벤토리 시스템을 구현합니다.
// 각 슬롯은 아이템 정보를 저장하며, 아이템의 추가, 제거, 이동을 관리합니다.
// 아이템의 최대 수량 제한을 두고, 같은 아이템을 같은 슬롯에 추가하거나 빈 슬롯에 새로 추가할 수 있습니다.
// 인벤토리 간 슬롯 이동 기능도 제공하여 유저가 아이템을 자유롭게 정리할 수 있습니다.

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public Sprite icon;
        public int currentCount;
        public int maxAllowed;
        public int price;
        public bool isSellable;
        public Item item;
        public PlantData plantData;

        public Slot()
        {
            itemName = "";
            currentCount = 0;
            maxAllowed = 99;
            price = 0;
            isSellable = false;
            plantData = null;
            item = null;
        }

        public bool isEmpty
        {
            get
            {
                if (itemName == "" && currentCount == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanAddItem(string itemName)
        {
            if (this.itemName == itemName && currentCount < maxAllowed)
            {
                return true;
            }
            return false;
        }

        public void AddItem(Item item, int count = 1)
        {
            this.itemName = item.itemData.itemName;
            this.icon = item.itemData.icon;
            this.price = item.itemData.price;
            this.maxAllowed = item.itemData.maxAllowed;
            this.plantData = item.plantData;
            this.isSellable = item.itemData.isSellable;
            this.item = item;
            currentCount += count;
        }

        public void RemoveItem()
        {
            if (currentCount > 0)
            {
                currentCount--;

                if (currentCount == 0)
                {
                    icon = null;
                    itemName = "";
                    plantData = null;
                }
            }
        }

        public void RemoveAllItems()
        {
            currentCount = 0;
            icon = null;
            itemName = "";
            plantData = null;
        }
        public void Clear()
        {
            itemName = string.Empty;
            currentCount = 0;
            maxAllowed = 99;
            price = 0;
            isSellable = false;
            plantData = null;
            item = null;
        }
        public int GetRemainingSpace()
        {
            return maxAllowed - currentCount;
        }
    }

    public List<Slot> slots = new List<Slot>();
    public Slot selectedSlot = null;
    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    // 읽기 전용 리스트를 반환하는 속성
    // IReadOnlyList<Slot> 읽을 수는 있지만 수정할 수는 없는 
    public IReadOnlyList<Slot> GetSlots => slots.AsReadOnly();

    public void Add(Item item)
    {
        // 슬롯의 타입이 추가하려는 아이템의 타입과 같고 maxAllowed보다 적으면
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.itemData.itemName && slot.CanAddItem(item.itemData.itemName))
            {
                if(item.isDropped)
                {
                    int itemCount = item.GetDroppedItemCount();
                    slot.AddItem(item,itemCount);
                }
                else
                {
                    slot.AddItem(item);
                }
                return;
            }
        }

        //같은 타입의 슬롯이 없거나 슬롯이 꽉차서 아이템을 추가할 수 없는 경우
        foreach (Slot slot in slots)
        {
            // 빈 슬롯이면
            if (slot.itemName == "")
            {
                 if(item.isDropped)
                {
                    int itemCount = item.GetDroppedItemCount();
                    slot.AddItem(item,itemCount);
                }
                else
                {
                    slot.AddItem(item);
                }
                return;
            }
        }
    }

    public void Remove(int index, bool isDrop = false)
    {
        if(isDrop)
        {
            slots[index].RemoveAllItems();
        }
        else
        {
            slots[index].RemoveItem();
        }
    }

    public void MoveSlot(int fromIndex, int toIndex, Inventory toInventory, int numToMove = 1, string fromInventoryName = "")
    {
        Slot fromSlot = slots[fromIndex];
        Slot toSlot = toInventory.slots[toIndex];

        if (toSlot.isEmpty || toSlot.CanAddItem(fromSlot.itemName))
        {
            for (int i = 0; i < numToMove; i++)
            {
                toSlot.AddItem(fromSlot.item);
                Player.Instance.inventoryManager.AddInventory(fromInventoryName, fromSlot.item);
                fromSlot.RemoveItem();
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (slots != null && slots.Count > 0)
        {
            selectedSlot = slots[index];
        }
    }

    public void Clear()
    {
        foreach (var slot in slots)
        {
            slot.Clear();
        }
    }
}