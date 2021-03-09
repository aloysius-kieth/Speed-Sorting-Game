using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public int duration = 30;

    int currentDuration;

    float spawnDuration = 5f;

    public bool timerStarted;
    public bool increaseDifficulty = false;

    bool alertStarted = false;

    public void Init()
    {
        timerText.text = duration.ToString();
    }

    public void Populate(GameSettings settings)
    {
        spawnDuration = settings.spawnDuration;
        duration = settings.gameDuration;
        currentDuration = duration;
    }

    public void StartTimer()
    {
        timerStarted = true;
        TimeToSpawn(spawnDuration);
        RunTimer();
    }

    public void StopTimer()
    {
        timerStarted = false;
        alertStarted = false;
    }

    void Update()
    {
        if (!timerStarted) return;

        if (currentDuration <= 0 && timerStarted)
        {
            currentDuration = 0;
            timerStarted = false;
            alertStarted = false;
            GameEvents.OnGameTimerEndEvent?.Invoke();
        }

        if (currentDuration <= 5 && !alertStarted && timerStarted)
        {
            alertStarted = true;
            RunAlert();
        }
    }

    async void TimeToSpawn(float delay)
    {
        float count = delay;
        while (timerStarted)
        {
            count--;
            if(count == 0)
            {
                count = delay;
                GameEvents.SpawnObjectEvent?.Invoke();
            }
            await new WaitForSeconds(1);
        }
    }

    async void RunTimer()
    {
        while (timerStarted)
        {
            currentDuration--;
            timerText.text = currentDuration.ToString();
            await new WaitForSeconds(1);
        }
    }

    async void RunAlert()
    {
        while(alertStarted)
        {
            timerText.DOColor(new Color(1.54f, 0.18f, 0.255f), 0.25f);
            timerText.transform.DOShakePosition(0.15f, new Vector3(30f, 0), 10, 0, false, true).OnComplete(()=> {
                timerText.DOColor(new Color(0.561f, 0.39f, 0.19f), 0.25f); });
            await new WaitForSeconds(1);
        }
    }

    public void Reset()
    {
        StopTimer();
        currentDuration = duration;
        timerText.text = currentDuration.ToString();
    }
}