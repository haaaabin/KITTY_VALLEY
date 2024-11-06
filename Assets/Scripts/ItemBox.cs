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
    bool isPlayerInRange = false;
    int itemPrice = 0;
    int itemCount = 0;
    public int sellingPrice = 0;


    Inventory.Slot selectedSlot;

    void Start()
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

    void Update()
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

    void OpenItemBox()
    {
        Debug.Log("아이템 박스 오픈");
        isBoxOpen = true;
        anim.SetBool("isOpen", isBoxOpen);
        sellingPanel.SetActive(true);

        InitializePanel();

        if (UIManager.instance != null && !UIManager.instance.isInventoryOpen)
        {
            UIManager.instance.ToggleInventoryUI();
        }
    }

    void CloseItemBox()
    {
        Debug.Log("아이템 박스 클로즈");
        isBoxOpen = false;
        anim.SetBool("isOpen", isBoxOpen);
        sellingPanel.SetActive(false);

        if (UIManager.instance != null && UIManager.instance.isInventoryOpen)
        {
            UIManager.instance.ToggleInventoryUI();
        }
    }

    void UpdatePanel()
    {
        if (selectedSlot != null)
        {
            sellingIcon.sprite = selectedSlot.count > 0 && selectedSlot.isSellable ? selectedSlot.icon : null;
            sellingIcon.color = selectedSlot.count > 0 && selectedSlot.isSellable ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
            priceText.text = itemPrice.ToString();
            countText.text = itemCount.ToString();
        }
    }

    void InitializePanel()
    {
        sellingIcon.sprite = null;
        itemPrice = 0;
        itemCount = 0;
    }

    void OnPlusButtonClick()
    {
        if (selectedSlot != null && selectedSlot.count > 0 && itemCount < selectedSlot.count)
        {
            if (selectedSlot.isSellable)
            {
                itemCount++;
                itemPrice = itemCount * selectedSlot.price;
                Debug.Log("selectedSlot" + selectedSlot.itemName + " : " + itemPrice);
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

    void OnMinusButtonClick()
    {
        if (selectedSlot != null && selectedSlot.count > 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                itemCount--;
                itemPrice = itemCount * selectedSlot.price;
                Debug.Log("selectedSlot" + selectedSlot.itemName + " : " + itemPrice);
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

    void OnCheckButtonClick()
    {
        if (selectedSlot != null && selectedSlot.count >= 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                sellingPrice += itemPrice;
                Debug.Log("sellingPrice" + selectedSlot.itemName + " : " + sellingPrice);
                selectedSlot.count -= itemCount;

                PlayerPrefsManager.instance.AddMoney(sellingPrice);
                UIManager.instance.UpdateMoneyUI();

                UIManager.instance.RefreshInventoryUI("Toolbar");
                InitializePanel();

                sellingPrice = 0;
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


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
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
}
