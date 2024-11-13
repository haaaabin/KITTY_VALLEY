using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

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
            if (TitleUIManager.instance.isNewGame)
            {
                player.inventoryManager.ClearInventory();
                inventorySave.DeleteSavedFiles();
            }
            else
            {
                player.inventoryManager.LoadInventory();
                plantGrowthManager.LoadPlantsData();
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
