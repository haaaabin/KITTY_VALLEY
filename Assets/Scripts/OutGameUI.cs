using System.Collections;
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
        SoundManager.Instance.Init();
    }

    void Start()
    {
        SoundManager.Instance.Play("BGM/OutGame", SoundType.BGM);

        GameObject inventoryObj = new GameObject("InventorySave");
        SaveData inventorySave = inventoryObj.AddComponent<SaveData>();

        if (!inventorySave.HasSavedInventory())
        {
            isNewGame = true;
            continueGameBtn.gameObject.SetActive(false);
            newGameBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                StartCoroutine(StartGameWithFadeOut());
            });
        }
        else
        {
            isNewGame = false;
            continueGameBtn.gameObject.SetActive(true);
            continueGameBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                StartCoroutine(StartGameWithFadeOut());
            });

            newGameBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);

                // 기존 리스너 제거 (중복 등록 방지)
                yesBtn.onClick.RemoveAllListeners();
                noBtn.onClick.RemoveAllListeners();

                panel.SetActive(true);

                yesBtn.onClick.AddListener(() =>
                {
                    SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                    isNewGame = true;
                    StartCoroutine(StartGameWithFadeOut());
                });
                noBtn.onClick.AddListener(() =>
                {
                    SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
                    panel.SetActive(false);
                });
            });
        }

        exitGameBtn.onClick.AddListener(() =>
        {
            SoundManager.Instance.Play("EFFECT/Click2", SoundType.EFFECT);
            SoundManager.Instance.StopAll();
            Application.Quit();
        });
    }

    // FadeOut과 게임 시작을 동시에 실행
    private IEnumerator StartGameWithFadeOut()
    {
        SoundManager.Instance.FadeOut(3.0f);

        FadeEffect.instance.FadeAndLoadScene("InGameScene");

        yield return new WaitForSeconds(2.0f);
    }
}
