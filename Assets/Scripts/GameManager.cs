using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ItemManager itemManager;
    // public Player player;
    public TimeManager timeManager;
    public TileManager tileManager;
    public PlantGrowthManager plantGrowthManager;
    public ItemSellingBox itemBox;
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
        itemBox = FindObjectOfType<ItemSellingBox>();
        // player = FindObjectOfType<Player>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene")
        {
            // BGM을 재생하기 전에 볼륨을 0으로 설정
            AudioSource bgmSource = SoundManager.Instance.audioSources[(int)SoundType.BGM];
            bgmSource.volume = 0f;

            // BGM 재생 시작
            SoundManager.Instance.Play("BGM/InGame", SoundType.BGM);

            // 페이드 인 효과 적용
            SoundManager.Instance.FadeIn(2f); // 2초 동안 페이드 인

            if (OutGameUI.instance.isNewGame)
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
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
