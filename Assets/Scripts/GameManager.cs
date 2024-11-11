using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public ItemManager itemManager;
    public TileManager tileManager;
    public Player player;
    public TimeManager timeManager;
    public PlantGrowthManager plantGrowthManager;
    public ItemBox itemBox;
    public InventorySave inventorySave;

    public bool isSave = false;

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
        tileManager = GetComponent<TileManager>();
        timeManager = GetComponent<TimeManager>();
        inventorySave = GetComponent<InventorySave>();
        plantGrowthManager = GetComponent<PlantGrowthManager>();
        itemBox = FindObjectOfType<ItemBox>();
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        InventorySave.instance.LoadInventory("Backpack");
        InventorySave.instance.LoadInventory("Toolbar");
    }
}
