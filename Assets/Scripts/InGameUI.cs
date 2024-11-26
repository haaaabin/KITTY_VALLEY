using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private float moveDistance = 270f;
    private float moveSpeed = 5f;
    private bool isInventoryMoving = false;

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
        closeInventoryPos = inventoryPanel.anchoredPosition;
        openInventoryPos = closeInventoryPos + new Vector2(0, moveDistance);
    }

    private void Start()
    {
        saveText.enabled = false;
        yesBtn.onClick.AddListener(() =>
        {
            GameManager.instance.timeManager.StartCoroutine(GameManager.instance.timeManager.EndDay());
            dayEndPanel.SetActive(false);
        });

        noBtn.onClick.AddListener(() =>
        {
            dayEndPanel.SetActive(false);
        });

        gameExitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        UpdateMoneyUI(GameManager.instance.player.money);

        if (OutGameUI.instance.isNewGame)
        {
            speechBubble.SetActive(true);
        }
        else
        {
            speechBubble.SetActive(false);
        }
    }

    private void Update()
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
        // yield return new WaitForSeconds(0.5f);

        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            int currentMoney = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, elapsedTime / duration));
            UpdateMoneyUI(currentMoney);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameManager.instance.player.money = endValue;
        UpdateMoneyUI(GameManager.instance.player.money);
    }

    public void SaveAllData()
    {
        GameManager.instance.player.inventoryManager.SaveInventory();
        GameManager.instance.plantGrowthManager.SavePlantDataList();
        SaveData.instance.SavePlayerData(GameManager.instance.player.money, GameManager.instance.timeManager.day, GameManager.instance.timeManager.currentDayIndex, GameManager.instance.itemBox.sellingPrice);
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
