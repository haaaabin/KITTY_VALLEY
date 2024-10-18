using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/* 인벤토리 UI 관리
    다양한 인벤토리 UI 관리, 상태 갱신, 토글*/

public class UIManager : MonoBehaviour
{
    public Dictionary<string, Inventory_UI> inventoryUIByName = new Dictionary<string, Inventory_UI>();
    public GameObject inventoryPanel;
    public List<Inventory_UI> inventoryUIs;
    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    void Awake()
    {
        Initialize();
    }

    public Inventory_UI GetInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            return inventoryUIByName[inventoryName];
        }

        Debug.LogWarning("There is not inventory ui for" + inventoryName);
        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventoryUI();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            dragSingle = true;
        }
        else
        {
            dragSingle = false;
        }
    }

    public void ToggleInventoryUI()
    {
        if (inventoryPanel != null)
        {
            if (!inventoryPanel.activeSelf)
            {
                inventoryPanel.SetActive(true);
                RefreshInventoryUI("Backpack");
            }
            else
            {
                inventoryPanel.SetActive(false);
            }
        }
    }

    // inventoryName에 해당하는 인벤토리를 찾아 그 인벤토리 ui만 갱신
    public void RefreshInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            inventoryUIByName[inventoryName].Refresh();
        }
    }

    // inventoryUIByName 딕셔너리에 저장된 모든 인벤토리 ui 갱신
    public void RefreshAll()
    {
        foreach (KeyValuePair<string, Inventory_UI> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh();
        }
    }

    void Initialize()
    {
        foreach (Inventory_UI ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }
}
