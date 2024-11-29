using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    public static FadeEffect instance;
    public Image fadePanel;
    public TextMeshProUGUI loadingText;

    private float fadeDuration = 2f;

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

    public IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = fadePanel.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadePanel.color = new Color(0, 0, 0, targetAlpha);
        yield return new WaitForSeconds(1f);
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeAndSwitchScene(sceneName));
    }

    private IEnumerator FadeAndSwitchScene(string sceneName)
    {
        yield return FadeScreen(1f);

        loadingText.gameObject.SetActive(true);
        loadingText.text = "로딩 중...";

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 씬이 로드되었으나 자동으로 활성화되지 않도록 설정

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(2f);
                loadingText.text = "로딩 완료!";
                yield return new WaitForSeconds(1f);
                loadingText.gameObject.SetActive(false);

                asyncLoad.allowSceneActivation = true;
                yield return FadeScreen(0f);
            }
            yield return null;
        }

    }
}