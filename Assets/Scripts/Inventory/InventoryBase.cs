using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Pool;

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
        inventory = GameManager.instance.player.inventoryManager.GetInventoryByName(inventoryName);
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
        if (inventory == null || UIManager.draggedSlot == null) return;

        Item itemToDrop = GameManager.instance.itemManager.GetItemByName(inventory.slots[UIManager.draggedSlot.slotID].itemName);
        Debug.Log("inventory_slots " + inventory.slots[UIManager.draggedSlot.slotID].itemName);
        Debug.Log("itemToDrop " + itemToDrop.name);

        if (itemToDrop != null)
        {
            if (UIManager.dragSingle)
            {
                GameManager.instance.player.DropItem(itemToDrop);
                inventory.Remove(UIManager.draggedSlot.slotID);
            }
            else
            {
                GameManager.instance.player.DropItem(itemToDrop, inventory.slots[UIManager.draggedSlot.slotID].count);
                inventory.Remove(UIManager.draggedSlot.slotID, inventory.slots[UIManager.draggedSlot.slotID].count);
            }
        }
        
        Refresh();
        UIManager.draggedSlot = null;
    }

    public void SlotBeginDrag(Slot_UI slot)
    {
        UIManager.draggedSlot = slot;
        UIManager.draggedIcon = Instantiate(slot.itemIcon);
        UIManager.draggedIcon.transform.SetParent(canvas.transform);
        UIManager.draggedIcon.raycastTarget = false;
        UIManager.draggedIcon.rectTransform.sizeDelta = new Vector2(100, 100);

        MoveToMousePosition(UIManager.draggedIcon.gameObject);
    }

    public void SlotDrag()
    {
        MoveToMousePosition(UIManager.draggedIcon.gameObject);
    }

    public void SlotEndDrag()
    {
        Destroy(UIManager.draggedIcon.gameObject);
        UIManager.draggedIcon = null;
    }

    public void SlotDrop(Slot_UI slot)
    {
        if (slot == null) return;

        if (UIManager.dragSingle)
        {
            UIManager.draggedSlot.inventory.MoveSlot(UIManager.draggedSlot.slotID, slot.slotID, slot.inventory);
        }
        else
        {
            UIManager.draggedSlot.inventory.MoveSlot(UIManager.draggedSlot.slotID, slot.slotID, slot.inventory, UIManager.draggedSlot.inventory.slots[UIManager.draggedSlot.slotID].count);
        }
        GameManager.instance.uiManager.RefreshAll();
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
