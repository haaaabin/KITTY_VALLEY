using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using JetBrains.Annotations;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button riceBuyBtn;
    public Button tomatoBuyBtn;
    public List<Item> sellItems;

    private bool isPlayerInRange = false;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        riceBuyBtn.onClick.AddListener(() => BuyItem(sellItems[0]));
        tomatoBuyBtn.onClick.AddListener(() => BuyItem(sellItems[1]));
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetMouseButtonDown(1))
        {
            shopPanel.SetActive(true);
        }
    }

    public void BuyItem(Item item)
    {
        if (sellItems.Contains(item))
        {
            if (player.money >= item.itemData.price)
            {
                player.inventoryManager.Add("Toolbar", item);
                StartCoroutine(InGameUI.instance.UpdateMoneyEffect(player.money, player.money - item.itemData.price));
                Debug.Log(item.plantData.plantName);
            }
            else
            {
                InGameUI.instance.ShakingText();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = false;
            shopPanel.SetActive(false);
        }
    }
}