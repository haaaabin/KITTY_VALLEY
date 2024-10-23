using System.Collections.Generic;
using UnityEngine;

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
        public int count;
        public int maxAllowed;

        public Slot()
        {
            itemName = "";
            count = 0;
            maxAllowed = 99;
        }

        public bool isEmpty
        {
            get
            {
                if (itemName == "" && count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanAddItem(string itemName)
        {
            if (this.itemName == itemName && count < maxAllowed)
            {
                return true;
            }
            return false;
        }

        public void AddItem(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            count++;
        }

        public void AddItem(string itemName, Sprite icon, int maxAllowed)
        {
            this.itemName = itemName;
            this.icon = icon;
            count++;
            this.maxAllowed = maxAllowed;
        }

        public void RemoveItem()
        {
            if (count > 0)
            {
                count--;

                if (count == 0)
                {
                    icon = null;
                    itemName = "";
                }
            }
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

    public void Add(Item item)
    {
        // 슬롯의 타입이 추가하려는 아이템의 타입과 같고 maxAllowed보다 적으면
        foreach (Slot slot in slots)
        {
            if (slot.itemName == item.data.itemName && slot.CanAddItem(item.data.itemName))
            {
                slot.AddItem(item);
                return;
            }
        }

        //같은 타입의 슬롯이 없거나 슬롯이 꽉차서 아이템을 추가할 수 없는 경우
        foreach (Slot slot in slots)
        {
            // 빈 슬롯이면
            if (slot.itemName == "")
            {
                slot.AddItem(item);
                return;
            }
        }
    }

    public void Remove(int index)
    {
        slots[index].RemoveItem();
    }
    public void Remove(int index, int numToRemove)
    {
        if (slots[index].count >= numToRemove)
        {
            for (int i = 0; i < numToRemove; i++)
            {
                Remove(index);
            }
        }
    }

    public void MoveSlot(int fromIndex, int toIndex, Inventory toInventory, int numToMove = 1)
    {
        Slot fromSlot = slots[fromIndex];
        Slot toSlot = toInventory.slots[toIndex];

        if (toSlot.isEmpty || toSlot.CanAddItem(fromSlot.itemName))
        {
            for (int i = 0; i < numToMove; i++)
            {
                toSlot.AddItem(fromSlot.itemName, fromSlot.icon, fromSlot.maxAllowed);
                fromSlot.RemoveItem();
            }
        }
    }

    public void SelectSlot(int index)
    {
        if(slots != null && slots.Count > 0)
        {
            selectedSlot = slots[index];
        }
    }

}
