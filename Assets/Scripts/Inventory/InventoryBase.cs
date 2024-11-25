using System.Collections.Generic;
using UnityEngine;


public class InventoryBase : MonoBehaviour
{
    [SerializeField] protected List<Slot_UI> slots = new List<Slot_UI>();
    [SerializeField] protected Canvas canvas;
    protected Inventory inventory;
    public string inventoryName;

    protected virtual void Awake()
    {
        canvas = FindObjectOfType<Canvas>();
    }

    protected virtual void Start()
    {
        inventory = Player.Instance.inventoryManager.GetInventoryByName(inventoryName);
        SetUpSlot();
    }

    protected virtual void Update()
    {
        Refresh();
    }

    // 인벤토리 갱신
    public void Refresh()
    {
        if (slots.Count == inventory.slots.Count)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (inventory.slots[i].itemName != "")
                {
                    slots[i].SetItem(inventory.slots[i]);
                }
                else
                {
                    slots[i].EmptyItem();
                }
            }
        }
    }

    public void Remove()
    {
        if (inventory == null || InGameUI.instance.draggedSlot == null) return;

        int slotID = InGameUI.instance.draggedSlot.slotID;
        var slotData = inventory.slots[slotID];

        if (string.IsNullOrEmpty(slotData.itemName)) return;

        Item itemToDrop = GameManager.Instance.itemManager.GetItemByName(slotData.itemName);
        if (itemToDrop != null)
        {
            // 1. 드랍된 아이템에 개수를 저장하고 드랍
            Player.Instance.DropItem(itemToDrop, slotData.currentCount);
            // 2. 인벤토리에서 해당 슬롯의 아이템을 모두 삭제
            inventory.Remove(slotID, true);
        }
        Refresh();
        InGameUI.instance.draggedSlot = null;
    }

    public void SlotBeginDrag(Slot_UI slot)
    {
        InGameUI.instance.draggedSlot = slot;
        InGameUI.instance.draggedIcon = Instantiate(slot.itemIcon);
        InGameUI.instance.draggedIcon.transform.SetParent(canvas.transform);
        InGameUI.instance.draggedIcon.raycastTarget = false;
        InGameUI.instance.draggedIcon.rectTransform.sizeDelta = new Vector2(100, 100);

        MoveToMousePosition(InGameUI.instance.draggedIcon.gameObject);
    }

    public void SlotDrag()
    {
        MoveToMousePosition(InGameUI.instance.draggedIcon.gameObject);
    }

    public void SlotEndDrag()
    {
        Destroy(InGameUI.instance.draggedIcon.gameObject);
        InGameUI.instance.draggedIcon = null;
    }

    public void SlotDrop(Slot_UI slot)
    {
        if (slot == null || InGameUI.instance.draggedSlot == null) return;

        var draggedSlot = InGameUI.instance.draggedSlot;
        if (InGameUI.instance.dragSingle)
        {
            draggedSlot.inventory.MoveSlot(draggedSlot.slotID, slot.slotID, slot.inventory);
        }
        else
        {
            draggedSlot.inventory.MoveSlot(draggedSlot.slotID, slot.slotID, slot.inventory, draggedSlot.inventory.slots[draggedSlot.slotID].currentCount);

        }

        InGameUI.instance.RefreshAllInventory();
    }

    public void MoveToMousePosition(GameObject toMove)
    {
        if (canvas != null)
        {
            Vector2 position;

            // 스크린상의 마우스 위치를 캔버스의 로컬 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);

            // 변환된 로컬 위치를 다시 세계 좌표로 변환하여 게임 오브젝트를 해당 위치로 이동시킴
            toMove.transform.position = canvas.transform.TransformPoint(position);
        }
    }

    protected void SetUpSlot()
    {
        int counter = 0;

        foreach (Slot_UI slot in slots)
        {
            slot.slotID = counter;
            counter++;
            slot.inventory = inventory;
        }
    }
}
