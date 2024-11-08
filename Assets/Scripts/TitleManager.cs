using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;

    public bool isNewGame = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // "InGameScene" 씬이 로드되었을 때 데이터 로드
        if (scene.name == "InGameScene")
        {
            if(!isNewGame)
                LoadGameData();
        }
    }

    public void LoadGameData()
    {
        DataManager.instance.LoadGameData();
    }
}
