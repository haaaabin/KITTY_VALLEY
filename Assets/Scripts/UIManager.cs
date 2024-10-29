using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/* 인벤토리 UI 관리
    다양한 인벤토리 UI 관리, 상태 갱신, 토글*/

public class UIManager : MonoBehaviour
{
    public Dictionary<string, InventoryBase> inventoryUIByName = new Dictionary<string, InventoryBase>();
    public RectTransform inventoryPanel;
    public List<InventoryBase> inventoryUIs;
    public static Slot_UI draggedSlot;
    public static Image draggedIcon;
    public static bool dragSingle;

    public TextMeshProUGUI timeText;
    public float moveDistance = 270f;
    public float moveSpeed = 5f;

    public bool isInventoryOpen = false;
    bool isInventoryMoving = false;
    Vector2 closePosition;
    Vector2 openPosition;

    void Awake()
    {
        Initialize();
        closePosition = inventoryPanel.anchoredPosition;
        openPosition = closePosition + new Vector2(0, moveDistance);
    }

    public InventoryBase GetInventoryUI(string inventoryName)
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
        if (Input.GetKeyDown(KeyCode.E) && !isInventoryMoving)
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
        isInventoryOpen = !isInventoryOpen;
        Vector2 targetPosition = isInventoryOpen ? openPosition : closePosition;
        StartCoroutine(MovePanel(targetPosition));
    }

    IEnumerator MovePanel(Vector2 targetPosition)
    {
        isInventoryMoving = true;
        while (Vector2.Distance(inventoryPanel.anchoredPosition, targetPosition) > 0.1f)
        {
            inventoryPanel.anchoredPosition = Vector2.Lerp(inventoryPanel.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }
        inventoryPanel.anchoredPosition = targetPosition;
        isInventoryMoving = false;
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
        foreach (KeyValuePair<string, InventoryBase> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh();
        }
    }

    void Initialize()
    {
        foreach (InventoryBase ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }
}
