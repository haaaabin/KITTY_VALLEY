using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleUIManager : MonoBehaviour
{
    public static TitleUIManager instance;

    public GameObject panel;
    public Image fadePanel;
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
            DontDestroyOnLoad(fadePanel.gameObject);
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
                StartCoroutine(LoadSceneWithFade(fadePanel, "InGameScene"));
            });
        }
        else
        {
            isNewGame = false;
            continueGameBtn.gameObject.SetActive(true);
            continueGameBtn.onClick.AddListener(() =>
            {
                StartCoroutine(LoadSceneWithFade(fadePanel, "InGameScene"));
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

    public IEnumerator LoadSceneWithFade(Image fadePanel, string sceneName)
    {
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(fadePanel, 1f));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 씬이 로드되었으나 자동으로 활성화되지 않도록 설정

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        yield return StartCoroutine(FadeEffect.instance.FadeScreen(fadePanel, 0f));
    }
}
