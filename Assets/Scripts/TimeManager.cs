using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 시간 흐름 관리*/

public class TimeManager : MonoBehaviour
{
    public bool isDayEnding = false;
    public event Action OnDayEnd;
    public event Action OnNextDay;
    public event Action OnTimeUpdated;
    public int gameHour = 9;
    public int gameMinute = 0;
    public int currentDayIndex = 0;
    public int day = 1;

    public string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    private float timePerGameMinute = 10f;
    private float currentTime = 0f;

    private void Start()
    {
        OnTimeUpdated?.Invoke();
        OnNextDay?.Invoke();
        OnDayEnd += EndDay;
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
                OnDayEnd?.Invoke();
            }

            OnTimeUpdated?.Invoke();
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

        Player.Instance.SetPosition();

        OnDayEnd?.Invoke();

        Debug.Log("하루가 끝났습니다. 다음 날 시작");
    }

    public void EndDay()
    {
        StartCoroutine(EndDayCoroutine());
    }

    public IEnumerator EndDayCoroutine()
    {
        isDayEnding = true;
        Player.Instance.anim.enabled = false;

        yield return StartCoroutine(FadeEffect.instance.FadeScreen(1f));

        NextDay();

        yield return StartCoroutine(FadeEffect.instance.FadeScreen(0f));
        isDayEnding = false;
    }

}
