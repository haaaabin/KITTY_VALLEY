using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine.Tilemaps;

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

public class SaveData : MonoBehaviour
{
    public static SaveData instance;

    public Inventory inventoryToSave = null;    // 저장할 인벤토리
    private static Dictionary<int, ItemData> allItem = new Dictionary<int, ItemData>();
    private static int HashItem(ItemData item) => Animator.StringToHash(item.itemName);    // 아이템의 이름을 해시 값으로 변환해 고유한 키로 사용
    private const char SPLIT_CHAR = '_';

    private static Dictionary<int, PlantData> allPlant = new Dictionary<int, PlantData>();
    private static int HashItem(PlantData plant) => Animator.StringToHash(plant.plantName);

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
            int key = HashItem(i);

            if (!allItem.ContainsKey(key))
                allItem.Add(key, i);
        }
    }

    private void CreatePlantDataDictionary()
    {
        PlantData[] allPlants = Resources.FindObjectsOfTypeAll<PlantData>();

        foreach (PlantData i in allPlants)
        {
            int key = HashItem(i);

            if (!allPlant.ContainsKey(key))
                allPlant.Add(key, i);
        }
    }

    public void DeleteSavedFiles()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.txt");
        foreach (string filePath in filePaths)
        {
            File.Delete(filePath);
        }
        Debug.Log("All inventory files deleted at game start.");
    }

    /* ---- Inventory Save & Load ----*/
    public void SaveInventory(string inventoryName, Inventory inventoryToSave)
    {
        string filePath = Application.persistentDataPath + $"/{inventoryName}.txt";

        // StreamWriter : 파일에 텍스트 데이터를 쓰기 위한 클래스
        using (StreamWriter sw = new StreamWriter(filePath, false))
        {
            sw.WriteLine($"-- {inventoryName} --");
            foreach (var slot in inventoryToSave.GetSlots)
            {
                if (!slot.isEmpty)
                {
                    sw.WriteLine($"{slot.itemName}{SPLIT_CHAR}{slot.count}");
                }
            }
        }
        Debug.Log($"{inventoryName} inventory saved!");
    }

    public void LoadInventory(string inventoryName)
    {
        string filePath = Application.persistentDataPath + $"/{inventoryName}.txt";

        if (!File.Exists(filePath))
        {
            return;
        }

        Inventory currentInventory = null;
        if (inventoryName == "Backpack")
        {
            currentInventory = InventoryManager.instance.backpack;
        }
        else if (inventoryName == "Toolbar")
        {
            currentInventory = InventoryManager.instance.toolbar;
        }

        if (currentInventory != null)
        {
            currentInventory.Clear();
            Debug.Log("Clear inventory");
        }

        // StreamWriter : 파일에서 텍스트 데이터를 읽기 위한 클래스
        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                // load할 인벤토리 구분
                if (line.StartsWith("--"))
                {
                    continue;
                }
                else if (currentInventory != null)
                {
                    string[] data = line.Split(SPLIT_CHAR);
                    if (data.Length == 2)
                    {
                        string itemName = data[0];
                        int count = int.Parse(data[1]);

                        ItemData itemData;
                        // 아이템 이름을 해시 값으로 변환하고, 딕셔너리에서 해당 아이템 데이터 찾음
                        int key = Animator.StringToHash(itemName);
                        if (allItem.TryGetValue(key, out itemData))
                        {
                            for (int i = 0; i < count; i++)
                            {
                                Item item = new Item { itemData = itemData };
                                currentInventory.Add(item);
                            }
                        }
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
        Debug.Log($"PlantData saved!");
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
        else
        {
            Debug.LogWarning("No saved plant data found.");
        }
        return plantSaveDataList;
    }


    public bool HasSavedInventory()
    {
        string backpackPath = Application.persistentDataPath + "/Backpack.txt";
        string toolbarPath = Application.persistentDataPath + "/Toolbar.txt";

        return File.Exists(backpackPath) || File.Exists(toolbarPath);
    }

    public bool HasSavedPlant()
    {
        string plantPath = Application.persistentDataPath + "/plants.txt";

        return File.Exists(plantPath);
    }
}


