using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Dictionary<string, InventoryBase> inventoryUIByName = new Dictionary<string, InventoryBase>();
    public List<InventoryBase> inventoryUIs;
    public Slot_UI draggedSlot;
    public RectTransform inventoryPanel;
    public Image draggedIcon;
    public bool dragSingle;

    private float moveDistance = 270f;
    private float moveSpeed = 5f;

    public bool isInventoryOpen = false;
    private bool isInventoryMoving = false;
    private Vector2 closePosition;
    private Vector2 openPosition;

    public GameObject dayEndPanel;
    public Button yesBtn;
    public Button noBtn;

    public Button settingBtn;
    public GameObject settingPanel;
    public Button saveBtn;
    public Button gameExitBtn;
    public Button loadBtn;

    public Slider bgmSlider;
    public Slider effectSlider;

    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI saveText;


    void Awake()
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

        Initialize();
        closePosition = inventoryPanel.anchoredPosition;
        openPosition = closePosition + new Vector2(0, moveDistance);
    }

    void Start()
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

        // UpdateMoneyUI();
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

    // public void UpdateMoneyUI()
    // {
    //     int currentMoney = DataManager.instance.LoadPlayerMoney();
    //     moneyText.text = currentMoney.ToString();
    //     Debug.Log(currentMoney);
    // }

    /*** 인벤토리 UI ***/
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

    public InventoryBase GetInventoryUI(string inventoryName)
    {
        if (inventoryUIByName.ContainsKey(inventoryName))
        {
            return inventoryUIByName[inventoryName];
        }

        Debug.LogWarning("There is not inventory ui for" + inventoryName);
        return null;
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

    public void SaveData()
    {
        GameManager.instance.player.inventoryManager.SaveInventory();
        GameManager.instance.plantGrowthManager.SavePlantDataList();
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

}
