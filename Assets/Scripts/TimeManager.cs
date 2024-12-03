using System;
using System.Collections;
using TMPro;
using UnityEngine;

/* 시간 흐름 관리*/

public class TimeManager : MonoBehaviour
{
    public event Action OnDayEnd;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;
    public bool isDayEnding = false;
    public int gameHour = 9;
    public int gameMinute = 0;
    public int currentDayIndex = 0;
    public int day = 1;

    private string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private float timePerGameMinute = 10f;
    private float currentTime = 0f;
    private ItemSellingBox itemBox;

    private void Start()
    {
        itemBox = GameManager.instance.itemBox;
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

            if (gameHour >= 24)
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
        Player.Instance.SetPosition();
        Player.Instance.inventoryManager.toolbar.SelectSlot(0);

        OnDayEnd?.Invoke();

        Debug.Log("하루가 끝났습니다. 다음 날 시작");
    }

    public IEnumerator EndDay()
    {
        isDayEnding = true;
        Player.Instance.anim.enabled = false;
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
