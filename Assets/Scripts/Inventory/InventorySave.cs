using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InventorySave : MonoBehaviour
{
    public static InventorySave instance;
    public Inventory inventoryToSave = null;    // 저장할 인벤토리
    private static Dictionary<int, ItemData> allItem = new Dictionary<int, ItemData>();
    private static int HashItem(ItemData item) => Animator.StringToHash(item.itemName); //아이템의 이름을 해시 값으로 변환해 고유한 키로 사용
    const char SPLIT_CHAR = '_';

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        CreateItemDictionary();
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

    public void DeleteSavedFiles()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.txt");
        foreach (string filePath in filePaths)
        {
            File.Delete(filePath);
        }
        Debug.Log("All inventory files deleted at game start.");
    }

    public void ResetInventoryFiles(List<string> inventoryNames)
    {
        foreach (string inventoryName in inventoryNames)
        {
            string filePath = Application.persistentDataPath + $"/{inventoryName}.txt";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"{inventoryName} inventory file deleted.");
            }
        }
    }

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
            Debug.LogWarning("Inventory file not found!");
            return;
        }

        Inventory currentInventory = null;
        if (inventoryName == "Backpack")
        {
            currentInventory = InventoryManager.instance.backpack;
            Debug.Log("Loading Backpack inventory...");
        }
        else if (inventoryName == "Toolbar")
        {
            currentInventory = InventoryManager.instance.toolbar;
            Debug.Log("Loading toolbar inventory...");
        }

        if (currentInventory != null)
        {
            currentInventory.Clear();
            Debug.Log("Clear inventory...");
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

    public bool HasSavedInventory()
    {
        string backpackPath = Application.persistentDataPath + "/Backpack.txt";
        string toolbarPath = Application.persistentDataPath + "/Toolbar.txt";

        return File.Exists(backpackPath) || File.Exists(toolbarPath);
    }
}


