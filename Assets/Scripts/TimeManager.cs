using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 시간 흐름 관리*/

public class TimeManager : MonoBehaviour
{
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public Image fadePanel;

    private float timePerGameMinute = 1f;
    private float currentTime = 0f;
    private int gameHour = 9;
    private int gameMinute = 0;
    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private int currentDayIndex = 0;

    public bool isDayEnding = false;
    private int day = 1;

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

            if (gameHour >= 11)
            {
                StartCoroutine(EndDay());
            }

            UpdateTimeUI();
        }
    }

    public IEnumerator EndDay()
    {
        isDayEnding = true;
        GameManager.instance.player.anim.enabled = false;
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(fadePanel, 1f));

        NextDay();

        yield return StartCoroutine(FadeEffect.instance.FadeScreen(fadePanel, 0f));
        isDayEnding = false;
    }


    void NextDay()
    {
        gameHour = 9;
        gameMinute = 0;
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        day++;
        isDayEnding = false;

        OnDayEnd?.Invoke();
        UpdateTimeUI();
        // UIManager.instance.UpdateMoneyUI();
        GameManager.instance.player.SetPosition();

        Debug.Log("하루가 끝났습니다. 다음 날 시작");
    }

    void UpdateTimeUI()
    {
        dayText.text = $"{daysOfWeek[currentDayIndex]}\n{day}";
        timeText.text = $"{gameHour:D2} : {gameMinute:D2}";
    }

}
