using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    public static PlayerPrefsManager instance;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
}
