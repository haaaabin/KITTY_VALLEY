using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int money;
    public Dictionary<Vector3Int, int> toolbar;
    public Dictionary<Vector3Int, int> inventory;
    public Dictionary<Vector3Int, int> planthGrwothState;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

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
    }

    public void SavePlayerMoney(int money)
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
        PlayerPrefs.Save();
    }

    public int LoadPlayerMoney()
    {
        if (!PlayerPrefs.HasKey("PlayerMoney"))
        {
            SavePlayerMoney(0); // 돈을 0으로 초기 설정
        }
        return PlayerPrefs.GetInt("PlayerMoney");
    }

    public void AddMoney(int amount)
    {
        int currentMoney = LoadPlayerMoney();
        SavePlayerMoney(currentMoney + amount);
    }

    public void SaveInventory(Inventory inventory)
    {
        string jsonData = JsonUtility.ToJson(inventory);
        PlayerPrefs.SetString("InventoryData", jsonData);
        PlayerPrefs.Save();
    }

    public Inventory LoadInventory()
    {
        if(PlayerPrefs.HasKey("InventoryData"))
        {   
            string jsonData = PlayerPrefs.GetString("InventoryData");
            Inventory loadedInventory = JsonUtility.FromJson<Inventory>(jsonData);
            return loadedInventory;
        }
        else
        {
            return new Inventory(27);
        }
    }

    public void SaveToolbar(Inventory inventory)
    {
        string jsonData = JsonUtility.ToJson(inventory);
        PlayerPrefs.SetString("ToolbarData", jsonData);
        PlayerPrefs.Save();
    }

    public Inventory LoadToolbar()
    {
        if(PlayerPrefs.HasKey("ToolbarData"))
        {   
            string jsonData = PlayerPrefs.GetString("ToolbarData");
            Inventory loadedInventory = JsonUtility.FromJson<Inventory>(jsonData);
            return loadedInventory;
        }
        else
        {
            return new Inventory(9);
        }
    }

    public void SavePlantGrowthData()
    {
        Dictionary<Vector3Int, int> growthDays = GameManager.instance.plantGrowthManager.GetGrowthDays();
        Dictionary<Vector3Int, int> growthStages = GameManager.instance.plantGrowthManager.GetGrowthStages();

        foreach (var item in growthDays)
        {
            PlayerPrefs.SetInt("PlantGrowthDays_" + item.Key, item.Value);
            Debug.Log($"Saved PlantGrowthDays_ {item.Key} with value {item.Value}");
        }
        foreach (var item in growthStages)
        {
            PlayerPrefs.SetInt("PlantGrowthStages_" + item.Key, item.Value);
            Debug.Log($"Saved PlantGrowthStages_ {item.Key} with value {item.Value}");
        }
        PlayerPrefs.Save();
    }

    public void LoadPlantGrowthData()
    {
        Dictionary<Vector3Int, int> growthDays = GameManager.instance.plantGrowthManager.GetGrowthDays();
        Dictionary<Vector3Int, int> growthStages = GameManager.instance.plantGrowthManager.GetGrowthStages();

        growthDays.Clear();
        growthStages.Clear();

        foreach (var position in GameManager.instance.tileManager.GetAllTilePositions())
        {
            string plantKey = $"Plant_{position.x}_{position.y}_{position.z}";

            if(PlayerPrefs.HasKey($"{plantKey}_Days") && PlayerPrefs.HasKey($"{plantKey}_Stage"))
            {
                growthDays[position] = PlayerPrefs.GetInt($"{plantKey}_Days");
                growthStages[position] = PlayerPrefs.GetInt($"{plantKey}_Stage");
            }
        }
    }

    public bool HasSavedData()
    {
        return PlayerPrefs.HasKey("PlayerMoney") || PlayerPrefs.HasKey("InventoryData") || PlayerPrefs.HasKey("ToolbarData") 
            || PlayerPrefs.HasKey("PlantGrowthDays_0") || PlayerPrefs.HasKey("PlantGrowthStages_0");
    }

    public void SaveGameData()
    {
        Debug.Log("Game Data Saved:");

        // PlayerMoney 저장
        int playerMoney = GameManager.instance.player.money;
        SavePlayerMoney(playerMoney);
        Debug.Log("Player Money Saved: " + playerMoney);

        // Inventory 저장
        Inventory inventory = InventoryManager.instance.backpack;
        SaveInventory(inventory);
        Debug.Log("Backpack Inventory Saved: " + JsonUtility.ToJson(inventory));
        
        // Toolbar 저장
        Inventory toolbar = InventoryManager.instance.toolbar;
        SaveToolbar(toolbar);
        Debug.Log("Toolbar Inventory Saved: " + JsonUtility.ToJson(toolbar));

        // PlantGrowthData 저장
        SavePlantGrowthData();
    }

    public void LoadGameData()
    {
         // GameManager, InventoryManager, UIManager가 null인지 확인
        if (GameManager.instance == null) 
        {
            Debug.LogError("GameManager.instance is null.");
            return;
        }
        if (InventoryManager.instance == null) 
        {
            Debug.LogError("InventoryManager.instance is null.");
            return;
        }
        if (UIManager.instance == null) 
        {
            Debug.LogError("UIManager.instance is null.");
            return;
        }
        // PlayerMoney 로드
        int playerMoney = LoadPlayerMoney();
        GameManager.instance.player.money = playerMoney;
        Debug.Log($"Player Money Loaded: {playerMoney}");

        // Inventory 로드
        Inventory loadedInventory = LoadInventory();
        InventoryManager.instance.backpack = loadedInventory;
        Debug.Log($"Backpack Inventory Loaded: {JsonUtility.ToJson(loadedInventory)}");

        // Toolbar 로드
        Inventory loadedToolbar = LoadToolbar();
        InventoryManager.instance.toolbar = loadedToolbar;
        Debug.Log($"Toolbar Inventory Loaded: {JsonUtility.ToJson(loadedToolbar)}");
        
        // PlantGrowthData 로드
        LoadPlantGrowthData();
        Debug.Log("Plant Growth Data Loaded");
    }
}
