using UnityEngine;
using UnityEngine.UI;

public class OutGameUI : MonoBehaviour
{
    public static OutGameUI instance;

    public GameObject panel;
    public Button newGameBtn;
    public Button continueGameBtn;
    public Button exitGameBtn;
    public Button yesBtn;
    public Button noBtn;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeUI();
        GameManager.Instance.soundManager.Play("BGM/OutGame", SoundType.BGM);
    }

    private void InitializeUI()
    {
        SaveData saveData = new GameObject("InventorySave").AddComponent<SaveData>();
        bool hasSavedData = saveData.HasSavedInventory();

        GameManager.Instance.isNewGame = !hasSavedData;
        continueGameBtn.gameObject.SetActive(GameManager.Instance.isNewGame);

        newGameBtn.onClick.AddListener(() =>
        {
            if (hasSavedData)
            {
                ShowConfirmPanel();
            }
            else
            {
                StartNewGame();
            }
        });

        if (hasSavedData)
        {
            continueGameBtn.onClick.AddListener(LoadGame);
        }

        exitGameBtn.onClick.AddListener(Application.Quit);
    }

    private void ShowConfirmPanel()
    {
        panel.SetActive(true);

        // 기존 리스너 제거 (중복 등록 방지)
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();

        yesBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.isNewGame = true;
            panel.SetActive(false);
            LoadInGameScene();
        });

        noBtn.onClick.AddListener(() =>
        {
            panel.SetActive(false);
        });
    }

    private void StartNewGame()
    {
        GameManager.Instance.isNewGame = true;
        LoadInGameScene();
    }

    private void LoadGame()
    {
        GameManager.Instance.isNewGame = false;
        LoadInGameScene();
    }

    private void LoadInGameScene()
    {
        GameManager.Instance.soundManager.Stop("BGM/OutGame", SoundType.BGM);
        FadeEffect.instance.FadeAndLoadScene("InGameScene");
    }
}
