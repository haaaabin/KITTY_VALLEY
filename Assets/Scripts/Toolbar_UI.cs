using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    [SerializeField] 
    private List<Slot_UI> toolbarSlots = new List<Slot_UI>();
    private Slot_UI selectedSlot;
  

    void Start()
    {
        SelectSlot(0);
    }

    void Update()
    {
        Refresh();
        CheckAlphaNumericKeys();
    }

    public void Refresh()
    {
        if (toolbarSlots.Count == GameManager.instance.player.inventoryManager.toolbarSlotCount)
        {
            for (int i = 0; i < toolbarSlots.Count; i++)
            {
                if (GameManager.instance.player.inventoryManager.toolbar.slots[i].itemName != "")
                {
                    toolbarSlots[i].SetItem(GameManager.instance.player.inventoryManager.toolbar.slots[i]);
                }
                else
                {
                    toolbarSlots[i].EmptyItem();
                }
            }
        }
    }

    public void SelectSlot(int index)
    {
        if (toolbarSlots.Count == 9)
        {
            if (selectedSlot != null)
            {
                selectedSlot.SetHighlight(false);
            }
            selectedSlot = toolbarSlots[index];
            selectedSlot.SetHighlight(true);
            
            GameManager.instance.player.inventoryManager.toolbar.SelectSlot(index);
        }
    }

    private void CheckAlphaNumericKeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSlot(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSlot(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectSlot(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectSlot(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectSlot(6);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            SelectSlot(7);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            SelectSlot(8);
        }
    }

}
