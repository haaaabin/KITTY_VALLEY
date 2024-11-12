using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ItemManager itemManager;
    public Player player;
    public TimeManager timeManager;
    public PlantGrowthManager plantGrowthManager;
    public ItemBox itemBox;
    public InventorySave inventorySave;

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
        inventorySave = GetComponent<InventorySave>();
        plantGrowthManager = GetComponent<PlantGrowthManager>();

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
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
