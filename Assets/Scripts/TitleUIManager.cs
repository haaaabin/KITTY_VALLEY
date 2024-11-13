using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    public static TitleUIManager instance;

    public GameObject panel;
    public Button newGameBtn;
    public Button continueGameBtn;
    public Button exitGameBtn;
    public Button yesBtn;
    public Button noBtn;

    public bool isNewGame;

    void Awake()
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

    void Start()
    {
        GameObject inventoryObj = new GameObject("InventorySave");
        SaveData inventorySave = inventoryObj.AddComponent<SaveData>();

        if (!inventorySave.HasSavedInventory())
        {
            isNewGame = true;
            continueGameBtn.gameObject.SetActive(false);
            newGameBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("InGameScene");
            });
        }
        else
        {
            isNewGame = false;
            continueGameBtn.gameObject.SetActive(true);
            continueGameBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("InGameScene");
            });

            newGameBtn.onClick.AddListener(() =>
            {
                // 기존 리스너 제거 (중복 등록 방지)
                yesBtn.onClick.RemoveAllListeners();
                noBtn.onClick.RemoveAllListeners();

                panel.SetActive(true);

                yesBtn.onClick.AddListener(() => { isNewGame = true; SceneManager.LoadScene("InGameScene"); });
                noBtn.onClick.AddListener(() => { panel.SetActive(false); });
            });
        }

        exitGameBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
