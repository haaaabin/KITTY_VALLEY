using System.Collections.Generic;
using UnityEngine;


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
        
        // 슬롯이 비어있는지 확인
        public bool isEmpty
        {
            get
            {
                return string.IsNullOrEmpty(itemName) && currentCount == 0;
            }
        }

        // 해당 아이템을 추가할 수 있는지 확인
        public bool CanAddItem(string itemName)
        {
            return this.itemName == itemName && currentCount < maxAllowed;
        }

        // 아이템을 슬롯에 추가
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
                    ClearAll();
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

        // 슬롯 초기화
        public void ClearAll()
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

    public List<Slot> slots = new List<Slot>();  //슬롯 리스트
    public Slot selectedSlot = null;  // 현재 선택된 슬롯

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

    // 인벤토리에 아이템 추가
    public void Add(Item item)
    {
        // 슬롯의 타입이 추가하려는 아이템의 타입과 같고 maxAllowed보다 적으면
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.itemData.itemName && slot.CanAddItem(item.itemData.itemName))
            {
                if (item.isDropped)
                {
                    int itemCount = item.GetDroppedItemCount();
                    slot.AddItem(item, itemCount);
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
                if (item.isDropped)
                {
                    int itemCount = item.GetDroppedItemCount();
                    slot.AddItem(item, itemCount);
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
        if (isDrop)
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
            slot.ClearAll();
        }
    }
}