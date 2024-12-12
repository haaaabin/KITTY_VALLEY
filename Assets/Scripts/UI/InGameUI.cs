using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    public static InGameUI instance;

    public Dictionary<string, InventoryBase> inventoryUIByName = new Dictionary<string, InventoryBase>();
    public List<InventoryBase> inventoryUIs;
    public Slot_UI draggedSlot;
    public RectTransform inventoryPanel;
    public Image draggedIcon;
    public GameObject dayEndPanel;
    public GameObject settingPanel;
    public Button yesBtn;
    public Button noBtn;
    public Button settingBtn;
    public Button saveBtn;
    public Button gameExitBtn;
    public Button loadBtn;
    public Slider bgmSlider;
    public Slider effectSlider;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI saveText;
    public GameObject speechBubble;
    public GameObject postBoxPanel;
    public bool dragSingle;
    public bool isInventoryOpen = false;

    private Vector2 closeInventoryPos;
    private Vector2 openInventoryPos;
    private float moveSpeed = 6f;
    private bool isInventoryMoving = false;
    private Shop shop;
    private ItemSellingBox itemSellingBox;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeInventory();
        closeInventoryPos = new Vector2(0, 50);
        openInventoryPos = new Vector2(0, 320f);
    }

    private void Start()
    {
        saveText.enabled = false;
        yesBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
            GameManager.instance.timeManager.StartCoroutine(GameManager.instance.timeManager.EndDay());
            dayEndPanel.SetActive(false);
        });

        noBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
            dayEndPanel.SetActive(false);
        });

        gameExitBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
            Application.Quit();
        });

        UpdateMoneyUI(Player.Instance.money);

        if (OutGameUI.instance.isNewGame)
        {
            speechBubble.SetActive(true);
        }
        else
        {
            speechBubble.SetActive(false);
        }

        bgmSlider.value = 0.5f;
        effectSlider.value = 0.5f;
        bgmSlider.onValueChanged.AddListener((value) => SoundManager.Instance.OnVolumeChanged(value, SoundType.BGM));
        effectSlider.onValueChanged.AddListener((value) => SoundManager.Instance.OnVolumeChanged(value, SoundType.EFFECT));

        shop = FindObjectOfType<Shop>();
        itemSellingBox = FindObjectOfType<ItemSellingBox>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInventoryMoving && !shop.isOpenShopPanel && !itemSellingBox.isOpenItemSellingBox)
            {
                SoundManager.Instance.Play("EFFECT/Click", SoundType.EFFECT);
                ToggleInventoryUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel.activeSelf)
                settingPanel.SetActive(false);
        }
    }

    #region 인벤토리 UI 
    private void InitializeInventory()
    {
        foreach (InventoryBase ui in inventoryUIs)
        {
            if (!inventoryUIByName.ContainsKey(ui.inventoryName))
            {
                inventoryUIByName.Add(ui.inventoryName, ui);
            }
        }
    }

    public void ToggleInventoryUI()
    {
        if (isInventoryMoving) return;

        isInventoryOpen = !isInventoryOpen;
        Vector2 targetPosition = isInventoryOpen ? openInventoryPos : closeInventoryPos;
        StartCoroutine(MovePanel(targetPosition));
    }

    private IEnumerator MovePanel(Vector2 targetPosition)
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
    public void RefreshAllInventory()
    {
        foreach (KeyValuePair<string, InventoryBase> keyValuePair in inventoryUIByName)
        {
            keyValuePair.Value.Refresh();
        }
    }
    #endregion

    public void UpdateMoneyUI(int money)
    {
        moneyText.text = money.ToString();
    }

    public IEnumerator UpdateMoneyEffect(int startValue, int endValue)
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            int currentMoney = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, elapsedTime / duration));
            UpdateMoneyUI(currentMoney);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Player.Instance.money = endValue;
        UpdateMoneyUI(Player.Instance.money);
    }

    public void SaveAllData()
    {
        SaveData.instance.SaveGameData(
            Player.Instance.inventoryManager.backpack,
            Player.Instance.inventoryManager.toolbar,
            Player.Instance.money,
            GameManager.instance.timeManager.day,
            GameManager.instance.timeManager.currentDayIndex,
            GameManager.instance.itemBox.sellingPrice,
            GameManager.instance.plantGrowthManager.SavePlantDataList()
        );
        ShowSaveNotification();
    }

    public void ShowSaveNotification()
    {
        StartCoroutine(DisplaySaveText());
    }

    private IEnumerator DisplaySaveText()
    {
        saveText.enabled = true;
        yield return new WaitForSeconds(2);
        saveText.enabled = false;
    }

    public void ShowPostPanel()
    {
        postBoxPanel.SetActive(true);
        speechBubble.SetActive(false);
    }

    public void ShakingText()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float amplitude = 3f;
        float frequency = 20f;
        float duration = 1f;
        Color originalColor = moneyText.color;
        Vector3 orignalPosition = moneyText.rectTransform.anchoredPosition;

        moneyText.color = Color.red;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float offset = Mathf.Sin(elapsedTime * frequency) * amplitude;
            moneyText.rectTransform.anchoredPosition = orignalPosition + new Vector3(offset, 0, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        moneyText.rectTransform.anchoredPosition = orignalPosition;
        moneyText.color = originalColor;
    }
}
