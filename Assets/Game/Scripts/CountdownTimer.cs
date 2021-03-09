using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public Sprite[] images;

    public Image display;

    public bool useImages = true;

    const string PRESTART = "READY";
    const string END = "GO!";

    public int duration = 3;
    int count = 0;

    CanvasGroup canvasGrp;
    Animator animator;

    void Awake()
    {

    }

    public void Init()
    {
        canvasGrp = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        canvasGrp.alpha = 0;
        animator.enabled = false;
        gameObject.SetActive(false);
    }

    public void StartCountdown(bool useAnimator = false)
    {
        count = duration/* + 1*/;

        if (useAnimator)
        {
            animator.enabled = true;
            DoCountdownByAnimator();
        }
        else
            DoCountdown();

    }

    // count down by await (text)
    async void DoCountdown()
    {
        SetCountText(PRESTART);

        await new WaitForSeconds(1);
        while (count > 0)
        {
            countText.text = count.ToString();
            count--;
            await new WaitForSeconds(1);
        }

        SetCountText(END);
        Debug.Log("countdown finished!");

        await new WaitForSeconds(1);

        OnInActive();
    }

    void DoCountdownByAnimator()
    {
        if (useImages)
        {
            if (count == 3)
                display.sprite = images[0];
            else if(count == 2)
                display.sprite = images[1];
            else if (count == 1)
                display.sprite = images[2];
        }
        //Debug.Log(count);
        countText.text = count.ToString();
        count--;


        if (animator == null) { Debug.LogWarning("No Animator in countdown!"); return; }

        if(count < duration)
        {
            //DoShakeOnCount();
            TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.TICK, TrinaxAudioManager.AUDIOPLAYER.SFX);
        }

        animator.SetTrigger(count == 0 ? "Exit" : "Tick");

    }

    void SetCountText(string text)
    {
        if (countText == null) return;
        countText.text = text;
    }

    public void OnActive()
    {
        SetCountText(PRESTART);
        gameObject.SetActive(true);

        canvasGrp.DOFade(1.0f, 0.25f).OnComplete(()=> { StartCountdown(false); });
    }

    public void OnActiveWithAnimator()
    {
        SetCountText(PRESTART);
        gameObject.SetActive(true);

        StartCountdown(true);
    }

    public void OnInActive()
    {
        canvasGrp.DOFade(0f, 0.25f).OnComplete(() => {
            animator.enabled = false;
            DeactivateGameobject();
        });
    }

    void DeactivateGameobject()
    {
        gameObject.SetActive(false);
        GameEvents.OnCountdownEndEvent?.Invoke();
    }

    void DoShakeOnCount()
    {
        transform.DOShakePosition(0.15f, new Vector3(30, 30), 1, 90, false, true);
    }

}
