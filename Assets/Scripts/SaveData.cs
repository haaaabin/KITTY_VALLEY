using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class PlantSaveData
{
    public PlantData plantData;
    public Vector3Int position;
    public int growthStage;
    public int growthDay;
    public string currentState;
    public bool isWatered;

    public PlantSaveData(PlantData plantData, Vector3Int position, int growthStage, int growthDay, string currentState, bool isWatered)
    {
        this.plantData = plantData;
        this.position = position;
        this.growthStage = growthStage;
        this.growthDay = growthDay;
        this.currentState = currentState;
        this.isWatered = isWatered;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public int money;
    public int currentDay;
    public int currentDayIndex;
    public int sellingPrice;

    public PlayerSaveData(int money, int currentDay, int currentDayIndex, int sellingPrice)
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
public class InventorySaveData
{
    public List<InventorySlotData> slots = new List<InventorySlotData>();
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
        // Get all .txt files in the persistent data path
        string[] txtFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.txt");

        // Get all .json files in the persistent data path
        string[] jsonFilePaths = Directory.GetFiles(Application.persistentDataPath, "*.json");

        // Combine the two file arrays
        string[] allFilePaths = txtFilePaths.Concat(jsonFilePaths).ToArray();

        // Iterate through each file and delete it
        foreach (string filePath in allFilePaths)
        {
            File.Delete(filePath);
        }
    }

    /* ---- Inventory Save & Load ---- */
    public void SaveInventory(string inventoryName, Inventory inventoryToSave)
    {
        string filePath = Application.persistentDataPath + $"/{inventoryName}.json";

        InventorySaveData inventorySaveData = new InventorySaveData();

        foreach (var slot in inventoryToSave.GetSlots)
        {
            if (!slot.isEmpty)
            {
                inventorySaveData.slots.Add(new InventorySlotData
                {
                    itemName = slot.itemName,
                    currentCount = slot.currentCount,
                    plantName = slot.item?.plantData?.plantName
                });
            }
        }

        string json = JsonUtility.ToJson(inventorySaveData, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadInventory(string inventoryName)
    {
        string filePath = Application.persistentDataPath + $"/{inventoryName}.json";
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        InventorySaveData inventorySaveData = JsonUtility.FromJson<InventorySaveData>(json);

        Inventory currentInventory = inventoryName == "Backpack"
            ? InventoryManager.instance.backpack
            : InventoryManager.instance.toolbar;

        if (currentInventory != null)
        {
            currentInventory.Clear();
            foreach (var slotData in inventorySaveData.slots)
            {
                int key = Animator.StringToHash(slotData.itemName);
                if (allItem.TryGetValue(key, out var itemData))
                {
                    PlantData plantData = null;

                    // 해당 item의 plantData 가져오기
                    if (!string.IsNullOrEmpty(slotData.plantName))
                    {
                        int plantKey = Animator.StringToHash(slotData.plantName);
                        allPlant.TryGetValue(plantKey, out plantData);
                    };

                    for (int i = 0; i < slotData.currentCount; i++)
                    {
                        Item item = new GameObject("Item").AddComponent<Item>();
                        item.itemData = itemData;
                        item.plantData = plantData;
                        currentInventory.Add(item);
                    }
                }
            }
        }
    }


    /* ---- Plant Save & Load ----*/
    public void SavePlants(List<PlantSaveData> plantSaveDataList)
    {
        string filePath = Application.persistentDataPath + $"/plants.txt";

        using (StreamWriter sw = new StreamWriter(filePath, false))
        {
            sw.WriteLine($"-- PlantData --");
            foreach (var plant in plantSaveDataList)
            {
                sw.WriteLine($"{plant.plantData.plantName}{SPLIT_CHAR}{plant.position.x}{SPLIT_CHAR}{plant.position.y}{SPLIT_CHAR}{plant.position.z}{SPLIT_CHAR}{plant.growthStage}{SPLIT_CHAR}{plant.growthDay}{SPLIT_CHAR}{plant.currentState}{SPLIT_CHAR}{plant.isWatered}");
            }
        }
    }

    public List<PlantSaveData> LoadPlants()
    {
        string filePath = Application.persistentDataPath + $"/plants.txt";
        List<PlantSaveData> plantSaveDataList = new List<PlantSaveData>();

        if (File.Exists(filePath))
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split(SPLIT_CHAR);
                    if (data.Length == 8)
                    {
                        string plantName = data[0];
                        Vector3Int position = new Vector3Int(int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
                        int growthStage = int.Parse(data[4]);
                        int growthDay = int.Parse(data[5]);
                        string currentState = data[6];
                        bool isWatered = bool.Parse(data[7]);

                        int key = Animator.StringToHash(plantName);
                        PlantData plantData;
                        if (allPlant.TryGetValue(key, out plantData))
                        {
                            PlantSaveData saveData = new PlantSaveData(plantData, position, growthStage, growthDay, currentState, isWatered);
                            plantSaveDataList.Add(saveData);
                        }
                    }
                }
            }
        }
        return plantSaveDataList;
    }

    public void SavePlayerData(int currentMoney, int currentDay, int currentDayIndex, int sellingPrice)
    {
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        PlayerSaveData playerSaveData = new PlayerSaveData(currentMoney, currentDay, currentDayIndex, sellingPrice);

        string json = JsonUtility.ToJson(playerSaveData, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadPlayerData()
    {
        string filePath = Application.persistentDataPath + "/PlayerData.json";
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerSaveData playerSaveData = JsonUtility.FromJson<PlayerSaveData>(json);

            Player.Instance.money = playerSaveData.money;
            GameManager.instance.timeManager.day = playerSaveData.currentDay;
            GameManager.instance.timeManager.currentDayIndex = playerSaveData.currentDayIndex;
            GameManager.instance.itemBox.sellingPrice = playerSaveData.sellingPrice;

            if (playerSaveData.sellingPrice > 0)
            {
                GameManager.instance.itemBox.SellItems();
                GameManager.instance.itemBox.ResetSellingPrice();
            }
        }
    }
    public bool HasSavedInventory()
    {
        string backpackPath = Application.persistentDataPath + "/Backpack.json";
        string toolbarPath = Application.persistentDataPath + "/Toolbar.json";

        return File.Exists(backpackPath) || File.Exists(toolbarPath);
    }

    public bool HasSavedPlant()
    {
        string plantPath = Application.persistentDataPath + "/plants.txt";

        return File.Exists(plantPath);
    }
}
