using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class Shop : MonoBehaviour
{
    public GameObject shopPanel;
    public Button riceBuyBtn;
    public Button tomatoBuyBtn;
    public List<Item> sellItems;

    private bool isPlayerInRange = false;
    private bool isBuying = false;
    public bool isOpenShopPanel = false;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();

        riceBuyBtn.onClick.AddListener(() => BuyItem(sellItems[0]));
        tomatoBuyBtn.onClick.AddListener(() => BuyItem(sellItems[1]));
    }

    public void BuyItem(Item item)
    {
        if (isBuying) return;

        isBuying = true;
        if (sellItems.Contains(item))
        {
            if (player.money >= item.itemData.price)
            {
                player.money -= item.itemData.price;
                StartCoroutine(InGameUI.instance.UpdateMoneyEffect(player.money + item.itemData.price, player.money));
                player.inventoryManager.Add(item);
                SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
            }
            else
            {
                InGameUI.instance.ShakingText();
            }
        }
        StartCoroutine(ResetBuyState());
    }

    private IEnumerator ResetBuyState()
    {
        yield return new WaitForSeconds(0.3f);
        isBuying = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = true;
            shopPanel.SetActive(true);
            isOpenShopPanel = true;
            InGameUI.instance.ToggleInventoryUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = false;
            shopPanel.SetActive(false);
            isOpenShopPanel = false;
            InGameUI.instance.ToggleInventoryUI();
        }
    }
}