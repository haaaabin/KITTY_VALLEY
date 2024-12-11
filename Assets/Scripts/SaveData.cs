using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

[System.Serializable]
public class PlantSaveData
{
    public PlantData plantData;
    public string plantName;
    public Vector3Int position;
    public int growthStage;
    public int growthDay;
    public string currentState;
    public bool isWatered;

    public PlantSaveData(string plantName, Vector3Int position, int growthStage, int growthDay, string currentState, bool isWatered)
    {
        this.plantName = plantName;
        this.position = position;
        this.growthStage = growthStage;
        this.growthDay = growthDay;
        this.currentState = currentState;
        this.isWatered = isWatered;
    }
}

[System.Serializable]
public class PlayerData
{
    public int money;
    public int currentDay;
    public int currentDayIndex;
    public int sellingPrice;

    public PlayerData(int money, int currentDay, int currentDayIndex, int sellingPrice)
    {
        this.money = money;
        this.currentDay = currentDay;
        this.currentDayIndex = currentDayIndex;
        this.sellingPrice = sellingPrice;
    }
}

[System.Serializable]
public class InventorySlotData
{
    public string itemName;
    public int currentCount;
    public string plantName;
}

[System.Serializable]
public class InventoryData
{
    public List<InventorySlotData> slots = new List<InventorySlotData>();
}

[System.Serializable]
public class GameSaveData
{
    public PlayerData playerData;
    public InventoryData backpackData;
    public InventoryData toolbarData;
    public PlantDataWrapper plantData;
}

[System.Serializable]
public class PlantDataWrapper
{
    public List<PlantSaveData> plants = new List<PlantSaveData>();
}

public class SaveData : MonoBehaviour
{
    public static SaveData instance;
    public Inventory inventoryToSave = null;    // 저장할 인벤토리

    private static Dictionary<int, ItemData> allItem = new Dictionary<int, ItemData>();
    private static Dictionary<int, PlantData> allPlant = new Dictionary<int, PlantData>();
    private const char SPLIT_CHAR = '_';

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        CreateItemDictionary();
        CreatePlantDataDictionary();
    }

    // Resources 폴더에서 ItemData 객체들을 불러와서 allItem 딕셔너리에 해시 값으로 저장
    private void CreateItemDictionary()
    {
        ItemData[] allItems = Resources.FindObjectsOfTypeAll<ItemData>();

        foreach (ItemData i in allItems)
        {
            int key = Animator.StringToHash(i.itemName);  // 아이템의 이름을 해시 값으로 변환해 고유한 키로 사용

            if (!allItem.ContainsKey(key))
                allItem.Add(key, i);
        }
    }

    private void CreatePlantDataDictionary()
    {
        PlantData[] allPlants = Resources.FindObjectsOfTypeAll<PlantData>();

        foreach (PlantData i in allPlants)
        {
            int key = Animator.StringToHash(i.plantName);

            if (!allPlant.ContainsKey(key))
                allPlant.Add(key, i);
        }
    }

    public void DeleteSavedFiles()
    {
        // Get all .json files in the persistent data path
        string[] jsonFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.json");

        // Iterate through each file and delete it
        foreach (string filePath in jsonFilePaths)
        {
            File.Delete(filePath);
        }
    }

    public void SaveGameData(
        Inventory backpack, Inventory toolbar,
        int currentMoney, int currentDay, int currentDayIndex, int sellingPrice,
        List<PlantSaveData> plant)
    {
        string filePath = Application.persistentDataPath + $"/GameData.json";

        GameSaveData gameSaveData = new GameSaveData
        {
            playerData = new PlayerData(currentMoney, currentDay, currentDayIndex, sellingPrice),
            backpackData = new InventoryData(),
            toolbarData = new InventoryData(),
            plantData = new PlantDataWrapper()
        };

        foreach (var slot in backpack.GetSlots)
        {
            if (!slot.isEmpty)
            {
                gameSaveData.backpackData.slots.Add(new InventorySlotData
                {
                    itemName = slot.itemName,
                    currentCount = slot.currentCount,
                    plantName = slot.item?.plantData?.plantName
                });
            }
        }

        foreach (var slot in toolbar.GetSlots)
        {
            if (!slot.isEmpty)
            {
                gameSaveData.toolbarData.slots.Add(new InventorySlotData
                {
                    itemName = slot.itemName,
                    currentCount = slot.currentCount,
                    plantName = slot.item?.plantData?.plantName
                });
            }
        }
        gameSaveData.plantData.plants = plant;
        
        string json = JsonUtility.ToJson(gameSaveData, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/GameData.json";

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameSaveData gameSaveData = JsonUtility.FromJson<GameSaveData>(json);

            if (gameSaveData.playerData != null)
            {
                Player.Instance.money = gameSaveData.playerData.money;
                GameManager.instance.timeManager.day = gameSaveData.playerData.currentDay;
                GameManager.instance.timeManager.currentDayIndex = gameSaveData.playerData.currentDayIndex;
                GameManager.instance.itemBox.sellingPrice = gameSaveData.playerData.sellingPrice;

                if (gameSaveData.playerData.sellingPrice > 0)
                {
                    GameManager.instance.itemBox.SellItems();
                    GameManager.instance.itemBox.ResetSellingPrice();
                }
            }
            LoadInventoryFromData(InventoryManager.instance.backpack, gameSaveData.backpackData);
            LoadInventoryFromData(InventoryManager.instance.toolbar, gameSaveData.toolbarData);
            LoadPlantFromData(gameSaveData.plantData);
        }
    }
    private void LoadPlantFromData(PlantDataWrapper plantsDataWrapper)
    {
        List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>();
        plantSaveDataList.Clear();

        foreach (var plantsData in plantsDataWrapper.plants)
        {
            int plantKey = Animator.StringToHash(plantsData.plantName);
            allPlant.TryGetValue(plantKey, out PlantData plantData);
            plantsData.plantData = plantData;
        }
        plantSaveDataList = plantsDataWrapper.plants;

        GameManager.instance.plantGrowthManager.SetTilePlantSaveData(plantSaveDataList);
    }

    private void LoadInventoryFromData(Inventory inventory, InventoryData inventoryData)
    {
        if (inventory != null)
        {
            inventory.Clear();
            foreach (var slotsData in inventoryData.slots)
            {
                int key = Animator.StringToHash(slotsData.itemName);
                if (allItem.TryGetValue(key, out var itemData))
                {
                    PlantData plantData = null;

                    if (!string.IsNullOrEmpty(slotsData.plantName))
                    {
                        int plantKey = Animator.StringToHash(slotsData.plantName);
                        allPlant.TryGetValue(plantKey, out plantData);
                    }

                    for (int i = 0; i < slotsData.currentCount; i++)
                    {
                        Item item = new GameObject("Item").AddComponent<Item>();
                        item.itemData = itemData;
                        item.plantData = plantData;
                        inventory.Add(item);
                    }
                }
            }
        }
    }

    public void LoadPlants()
    {

    }

    public bool HasSavedData()
    {
        string saveDataPath = Application.persistentDataPath + "/GameData.json";

        return File.Exists(saveDataPath);
    }

    public bool HasSavedPlant()
    {
        string plantPath = Application.persistentDataPath + "/plants.json";

        return File.Exists(plantPath);
    }
}
