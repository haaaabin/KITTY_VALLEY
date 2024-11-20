using System;
using System.Security;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    public Animator anim;
    public GameObject sellingPanel;
    public Image sellingIcon;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI countText;
    public Button minusBtn;
    public Button plusBtn;
    public Button checkBtn;
    public bool isBoxOpen = false;
    public int sellingPrice = 0;

    private bool isPlayerInRange = false;
    private int itemPrice = 0;
    private int itemCount = 0;
    private Inventory.Slot selectedSlot;

    private void Start()
    {
        anim = GetComponent<Animator>();
        if (sellingPanel != null)
        {
            sellingPanel.SetActive(false);
        }

        plusBtn.onClick.AddListener(OnPlusButtonClick);
        minusBtn.onClick.AddListener(OnMinusButtonClick);
        checkBtn.onClick.AddListener(OnCheckButtonClick);
        InitializePanel();

    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetMouseButtonDown(1) && !isBoxOpen)
        {
            OpenItemBox();
        }

        // 슬롯 선택 상태 확인
        var currentSlot = GameManager.instance.player.inventoryManager.toolbar.selectedSlot;
        if (currentSlot != selectedSlot)
        {
            selectedSlot = currentSlot;
            InitializePanel(); // 슬롯이 변경되면 초기화
        }
        UpdatePanel();
    }

    private void OpenItemBox()
    {
        isBoxOpen = true;
        anim.SetBool("isOpen", isBoxOpen);
        sellingPanel.SetActive(true);

        InitializePanel();

        if (UIManager.instance != null && !UIManager.instance.isInventoryOpen)
        {
            UIManager.instance.ToggleInventoryUI();
        }
    }

    private void CloseItemBox()
    {
        isBoxOpen = false;
        anim.SetBool("isOpen", isBoxOpen);
        sellingPanel.SetActive(false);

        if (UIManager.instance != null && UIManager.instance.isInventoryOpen)
        {
            UIManager.instance.ToggleInventoryUI();
        }
    }

    private void InitializePanel()
    {
        sellingIcon.sprite = null;
        itemPrice = 0;
        itemCount = 0;
    }

    private void UpdatePanel()
    {
        if (selectedSlot != null)
        {
            sellingIcon.sprite = selectedSlot.count > 0 && selectedSlot.isSellable ? selectedSlot.icon : null;
            sellingIcon.color = selectedSlot.count > 0 && selectedSlot.isSellable ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
            priceText.text = itemPrice.ToString();
            countText.text = itemCount.ToString();
        }
    }

    private void OnPlusButtonClick()
    {
        if (selectedSlot != null && selectedSlot.count > 0 && itemCount < selectedSlot.count)
        {
            if (selectedSlot.isSellable)
            {
                itemCount++;
                itemPrice = itemCount * selectedSlot.price;
                UpdatePanel();
            }
            else
            {
                Debug.Log("판매 불가능한 상품");
            }
        }
        else
        {
            Debug.Log("보유 아이템 없음");
        }
    }

    private void OnMinusButtonClick()
    {
        if (selectedSlot != null && selectedSlot.count > 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                itemCount--;
                itemPrice = itemCount * selectedSlot.price;
                UpdatePanel();
            }
            else
            {
                Debug.Log("판매 불가능한 상품");
            }
        }
        else
        {
            Debug.Log("보유 아이템 없음");
        }
    }

    private void OnCheckButtonClick()
    {
        if (selectedSlot != null && selectedSlot.count >= 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                sellingPrice += itemPrice;
                selectedSlot.count -= itemCount;

                UIManager.instance.RefreshInventoryUI("Toolbar");
                InitializePanel();
            }
            else
            {
                Debug.Log("판매 불가능한 상품");
            }
        }
        else
        {
            Debug.Log("보유 아이템 없음");
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
            if (isBoxOpen)
            {
                CloseItemBox();
            }
        }
    }

    public void SellItems()
    {
        int dailyEarnings = GetSellingPrice();
        int newTotalMoney = GameManager.instance.player.money + dailyEarnings;

        StartCoroutine(UIManager.instance.UpdateMoneyEffect(GameManager.instance.player.money, newTotalMoney));
        PlayerPrefs.SetInt("Money", GameManager.instance.player.money);
        PlayerPrefs.SetInt("Selling", dailyEarnings);
    }

    public int GetSellingPrice()
    {
        return sellingPrice;
    }

    public void ResetSellingPrice()
    {
        sellingPrice = 0;
        PlayerPrefs.SetInt("SellingPrice", 0);
    }
}
