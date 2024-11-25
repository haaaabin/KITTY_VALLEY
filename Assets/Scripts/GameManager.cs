using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [HideInInspector] public TimeManager timeManager;
    [HideInInspector] public PlantGrowthManager plantGrowthManager;
    [HideInInspector] public ItemManager itemManager;
    [HideInInspector] public bool isNewGame;

    public SoundManager soundManager = new SoundManager();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        timeManager = GetComponent<TimeManager>();
        plantGrowthManager = GetComponent<PlantGrowthManager>();
        itemManager = GetComponent<ItemManager>();

        soundManager.Init();
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene")
        {
            soundManager.Play("BGM/InGame", SoundType.BGM);
            InitializeInGame();
        }
    }

    private void InitializeInGame()
    {
        if (isNewGame)
        {
            Player.Instance.inventoryManager.ClearInventory();
            plantGrowthManager.ClearPlantSaveData();
            SaveData.instance.DeleteSavedFiles();
            Player.Instance.money = 0;
            timeManager.day = 1;
            timeManager.currentDayIndex = 0;
        }
        else
        {
            Player.Instance.inventoryManager.LoadInventory();
            plantGrowthManager.LoadPlantsData();
            SaveData.instance.LoadPlayerData();
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
