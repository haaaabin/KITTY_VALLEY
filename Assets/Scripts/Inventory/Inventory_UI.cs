using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 개별 인벤토리 UI 담당 */

public class Inventory_UI : InventoryBase
{
    public GameObject inventoryPanel;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        inventory = GameManager.instance.player.inventoryManager.GetInventoryByName(inventoryName);
        if (inventory == null) return;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
