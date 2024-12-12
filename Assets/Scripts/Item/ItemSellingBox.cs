using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSellingBox : MonoBehaviour
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
    public bool isOpenItemSellingBox = false;

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
        if (isPlayerInRange && Input.GetMouseButtonDown(0) && !isBoxOpen)
        {
            OpenItemBox();
        }

        // 슬롯 선택 상태 확인
        var currentSlot = Player.Instance.inventoryManager.toolbar.selectedSlot;
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
        isOpenItemSellingBox = true;

        InitializePanel();

        if (InGameUI.instance != null && !InGameUI.instance.isInventoryOpen)
        {
            InGameUI.instance.ToggleInventoryUI();
        }
    }

    private void CloseItemBox()
    {
        isBoxOpen = false;
        anim.SetBool("isOpen", isBoxOpen);
        sellingPanel.SetActive(false);
        isOpenItemSellingBox = false;

        if (InGameUI.instance != null && InGameUI.instance.isInventoryOpen)
        {
            InGameUI.instance.ToggleInventoryUI();
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
            sellingIcon.sprite = selectedSlot.currentCount > 0 && selectedSlot.isSellable ? selectedSlot.icon : null;
            sellingIcon.color = selectedSlot.currentCount > 0 && selectedSlot.isSellable ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
            priceText.text = itemPrice.ToString();
            countText.text = itemCount.ToString();
        }
    }

    private void OnPlusButtonClick()
    {
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
        if (selectedSlot != null && selectedSlot.currentCount > 0 && itemCount < selectedSlot.currentCount)
        {
            if (selectedSlot.isSellable)
            {
                itemCount++;
                itemPrice = itemCount * selectedSlot.price;
                UpdatePanel();
            }
        }
    }

    private void OnMinusButtonClick()
    {
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
        if (selectedSlot != null && selectedSlot.currentCount > 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                itemCount--;
                itemPrice = itemCount * selectedSlot.price;
                UpdatePanel();
            }
        }
    }

    private void OnCheckButtonClick()
    {
        SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
        if (selectedSlot != null && selectedSlot.currentCount >= 0 && itemCount > 0)
        {
            if (selectedSlot.isSellable)
            {
                sellingPrice += itemPrice;
                selectedSlot.currentCount -= itemCount;

                // 선택된 슬롯 아이템의 개수가 0개면 슬롯 비우기
                if (selectedSlot.currentCount == 0)
                {
                    selectedSlot.ClearAll();
                }
                InGameUI.instance.RefreshInventoryUI("Toolbar");
                InitializePanel();
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
            if (isBoxOpen)
            {
                CloseItemBox();
            }
        }
    }

    public void SellItems()
    {
        int dailyEarnings = GetSellingPrice();
        int newTotalMoney = Player.Instance.money + dailyEarnings;

        StartCoroutine(InGameUI.instance.UpdateMoneyEffect(Player.Instance.money, newTotalMoney));
    }

    public int GetSellingPrice()
    {
        return sellingPrice;
    }

    public void ResetSellingPrice()
    {
        sellingPrice = 0;
    }
}
