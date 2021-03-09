using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public int Score { get; set; }
    public TextMeshProUGUI scoreText;

    int comboCount;
    int comboScore;

    public float combo_const_Value = 1.7f;

    public ComboCounter comboCounter;

    void Start()
    {
        Score = 0;
        comboScore = 0;
        comboCount = 0;
        Init();
    }

    void Init()
    {
        GameEvents.OnComboStartEvent += OnComboStartEvent;
        GameEvents.OnComboUpdateEvent += OnComboUpdateEvent;
        GameEvents.OnComboEndEvent += OnComboEndEvent;
        GameEvents.OnComboCancelledEvent += OnComboCancelledEvent;
        GameEvents.OnGameTimerEndEvent += OnGameTimerEndEvent;
        UpdateHUD();
    }

    float CalculateCombo(int count, float const_value)
    {
        return Mathf.Pow(count, const_value);
    }

    void OnComboStartEvent()
    {
        TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_1, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //ComboCount++;
        comboCount++;
        comboScore = (int)CalculateCombo(comboCount, combo_const_Value);
        comboCounter.comboCountText.text = comboScore.ToString();
    }

    int indexToPlay = (int)TrinaxAudioManager.AUDIOS.COMBO_1;
    void OnComboUpdateEvent()
    {
        comboCount++;
        comboScore = (int)CalculateCombo(comboCount, combo_const_Value);
        comboCounter.comboCountText.text = comboScore.ToString();

        if (indexToPlay == (int)TrinaxAudioManager.AUDIOS.COMBO_8)
            indexToPlay = (int)TrinaxAudioManager.AUDIOS.COMBO_8;
        else
            indexToPlay++;

        TrinaxAudioManager.Instance.PlaySFX((TrinaxAudioManager.AUDIOS)indexToPlay, TrinaxAudioManager.AUDIOPLAYER.SFX2);

        //if (ComboCount == 2)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_2, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}
        //else if (ComboCount == 3)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_3, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}
        //else if (ComboCount == 4)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_4, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}
        //else if (ComboCount == 5)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_5, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}
        //else if (ComboCount == 6)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_6, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}
        //else if (ComboCount == 7)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_7, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}
        //else if (ComboCount >= 8)
        //{
        //    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.COMBO_8, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        //}

    }

    void OnComboEndEvent()
    {
        Score = Score + comboScore;
        UpdateHUD();
        comboScore = 0;
        comboCount = 0;
        indexToPlay = (int)TrinaxAudioManager.AUDIOS.COMBO_1;
    }

    void OnComboCancelledEvent()
    {
        comboScore = 0;
        comboCount = 0;
        comboCounter.comboCountText.text = comboScore.ToString();
        indexToPlay = (int)TrinaxAudioManager.AUDIOS.COMBO_1;
    }

    void OnGameTimerEndEvent()
    {
        OnComboEndEvent();
    }

    void UpdateHUD()
    {
        scoreText.text = Score.ToString();
    }

    public void AddScore(int amt)
    {
        Score += amt;
        UpdateHUD();
    }

    public void MinusScore(int amt)
    {
        TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.SCORE_FAIL, TrinaxAudioManager.AUDIOPLAYER.SFX2);
        Score -= amt;
        if (Score <= 0)
        {
            Score = 0;
        }
        UpdateHUD();
        scoreText.DOColor(new Color(1.54f, 0.18f, 0.255f), 0.25f).OnComplete(()=> {
            scoreText.DOColor(new Color(1f, 0.79f, 0.16f), 0.25f);
        });
        scoreText.transform.DOShakePosition(0.25f, new Vector3(25f, 0), 5, 0, false, true);
    }

    public void Reset()
    {
        Score = 0;
        comboScore = 0;
        comboCount = 0;
        comboCounter.comboCountText.text = comboScore.ToString();
        UpdateHUD();
        scoreText.color = new Color(1f, 0.79f, 0.16f);
    }
}
