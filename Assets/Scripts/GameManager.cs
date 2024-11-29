using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ItemManager itemManager;
    public TimeManager timeManager;
    public TileManager tileManager;
    public PlantGrowthManager plantGrowthManager;
    public ItemSellingBox itemBox;
    public SaveData inventorySave;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        itemManager = GetComponent<ItemManager>();
        timeManager = GetComponent<TimeManager>();
        inventorySave = GetComponent<SaveData>();
        plantGrowthManager = GetComponent<PlantGrowthManager>();
        tileManager = GetComponent<TileManager>();
        itemBox = FindObjectOfType<ItemSellingBox>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene")
        {
            // 페이드 인 효과 적용
            SoundManager.Instance.FadeIn(1f, "BGM/InGame"); // 2초 동안 페이드 인

            if (OutGameUI.instance.isNewGame)
            {
                Player.Instance.inventoryManager.ClearInventory();
                plantGrowthManager.ClearPlantSaveData();
                SaveData.instance.DeleteSavedFiles();
                Player.Instance.money = 0;
                timeManager.day = 1;
                timeManager.currentDayIndex = 0;
            }
            else
            {
                Player.Instance.inventoryManager.LoadInventory();
                plantGrowthManager.LoadPlantsData();
                SaveData.instance.LoadPlayerData();
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
