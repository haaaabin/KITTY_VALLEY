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
    public bool isDayEnding = false;
    public event Action OnDayEnd;
    public int gameHour = 9;
    public int gameMinute = 0;
    public int currentDayIndex = 0;
    public int day = 1;

    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private float timePerGameMinute = 1f;
    private float currentTime = 0f;
    private ItemBox itemBox;
    private Player player;

    private void Start()
    {
        itemBox = GameManager.instance.itemBox;
        player = GameManager.instance.player;
        UpdateTimeUI();
    }

    private void Update()
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

    private void NextDay()
    {
        gameHour = 9;
        gameMinute = 0;
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        day++;
        isDayEnding = false;

        itemBox.SellItems();
        itemBox.ResetSellingPrice();

        UpdateTimeUI();
        player.SetPosition();

        OnDayEnd?.Invoke();

        Debug.Log("하루가 끝났습니다. 다음 날 시작");
    }

    public IEnumerator EndDay()
    {
        isDayEnding = true;
        player.anim.enabled = false;
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(1f));
        NextDay();
        yield return StartCoroutine(FadeEffect.instance.FadeScreen(0f));
        isDayEnding = false;
    }

    public void UpdateTimeUI()
    {
        dayText.text = $"{daysOfWeek[currentDayIndex]}\n{day}";
        timeText.text = $"{gameHour:D2} : {gameMinute:D2}";
    }
}
