using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ItemManager itemManager;
    public Player player;
    public TimeManager timeManager;
    public TileManager tileManager;
    public PlantGrowthManager plantGrowthManager;
    public ItemBox itemBox;
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
        itemBox = FindObjectOfType<ItemBox>();
        player = FindObjectOfType<Player>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene")
        {
            if (OutGameUI.instance.isNewGame)
            {
                player.inventoryManager.ClearInventory();
                plantGrowthManager.ClearPlantSaveData();
                SaveData.instance.DeleteSavedFiles();
                player.money = 0;
                timeManager.day = 1;
                timeManager.currentDayIndex = 0;
            }
            else
            {
                player.inventoryManager.LoadInventory();
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
