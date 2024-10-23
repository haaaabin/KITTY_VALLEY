using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 시간 흐름 관리*/

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    public Image fadeImg;

    private float timePerGameMinute = 1f;
    private float currentTime = 0f;
    private int gameHour = 9;
    private int gameMinute = 0;
    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private int currentDayIndex = 0;
    private float fadeDuration = 4f;
    private bool isDayEnding = false;
    private int day = 0;

    public event Action OnDayEnd;

    void Start()
    {
        UpdateTimeUI();
    }

    void Update()
    {
        if (isDayEnding) return;

        currentTime += Time.deltaTime;

        if (currentTime >= timePerGameMinute)
        {
            currentTime = 0f;
            gameMinute += 10;

            if (gameMinute >= 60)
            {
                gameMinute = 0;
                gameHour++;
            }

            if (gameHour >= 10)
            {
                StartCoroutine(EndDay());
            }

            UpdateTimeUI();
        }
    }

    IEnumerator EndDay()
    {
        isDayEnding = true;
        yield return StartCoroutine(FadeScreen(1f));

        NextDay();

        yield return StartCoroutine(FadeScreen(0f));
        isDayEnding = false;
    }

    IEnumerator FadeScreen(float targetAlpha)
    {
        float startAlpha = fadeImg.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            fadeImg.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        fadeImg.color = new Color(0, 0, 0, targetAlpha);
        yield return new WaitForSeconds(1f);
    }

    void NextDay()
    {
        gameHour = 9;
        gameMinute = 0;
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        day++;

        OnDayEnd?.Invoke();
        UpdateTimeUI();

        Debug.Log("하루가 끝났습니다. 다음 날 시작");
    }

    void UpdateTimeUI()
    {
        dayText.text = $"{daysOfWeek[currentDayIndex]} {gameHour:D2}:{gameMinute:D2}";
    }

    public int GetDay()
    {
        return day;
    }
}
