using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ComboCounter : MonoBehaviour
{
    public bool ComboStarted { get; set; }
    public TextMeshProUGUI comboCountText;
    public CanvasGroup canvasGroup;
    public Transform imageBackground;
    public Image counterImage;

    public float decreaseRate = 0.5f;

    bool startFill = false;

    void Start()
    {
        counterImage.fillAmount = 1;
        comboCountText.text = "";
        canvasGroup.alpha = 0;

        GameEvents.OnComboStartEvent += OnComboStartEvent;
        GameEvents.OnComboUpdateEvent += OnComboUpdateEvent;
        GameEvents.OnComboCancelledEvent += OnComboCancelledEvent;
        GameEvents.OnGameoverEvent += OnGameoverEvent;
    }

    public void Populate(GameSettings settings)
    {
        decreaseRate = settings.comboDecreaseRate;
    }

    void OnComboStartEvent()
    {
        canvasGroup.DOFade(1.0f, 0.15f);
        counterImage.fillAmount = 1;
        imageBackground.transform.localScale = Vector3.one;
        imageBackground.transform.DOScale(Vector3.one * 1.4f, 0.25f).OnComplete(()=> {
            startFill = true;
            imageBackground.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBounce);
        });
        //comboCountText.transform.localScale = Vector3.one;
        //comboCountText.transform.DOPunchScale(Vector3.one * 0.95f, 0.50f, 1, 0.1f);
    }

    void OnComboUpdateEvent()
    {
        counterImage.fillAmount = 1;
        imageBackground.transform.localScale = Vector3.one;
        imageBackground.transform.DOScale(Vector3.one * 1.4f, 0.25f).OnComplete(() =>
        {
            imageBackground.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBounce);
        });
        //comboCountText.transform.localScale = Vector3.one;
        //comboCountText.transform.DOPunchScale(Vector3.one * 0.95f, 0.25f, 1, 0.1f);
    }

    void OnComboCancelledEvent()
    {
        Reset();
    }

    void OnGameoverEvent()
    {
        Reset();
    }

    void AnimateFill(System.Action callback)
    {
        counterImage.fillAmount = Mathf.Lerp(counterImage.fillAmount, 1.0f, Time.deltaTime * 10f);
        
        if(counterImage.fillAmount >= 0.9)
        {
            counterImage.fillAmount = 1f;
            callback?.Invoke();
            startFill = false;
        }
    }

    void Update()
    {
        if (startFill)
            AnimateFill(() => { ComboStarted = true; });

        if (ComboStarted)
        {
            if(counterImage.fillAmount <= 0)
            {
                ComboStarted = false;
                canvasGroup.DOFade(0f, 0.15f);
                Debug.Log("Combo ended");
                GameEvents.OnComboEndEvent?.Invoke();
            }
            // update counter
            counterImage.fillAmount -= Time.deltaTime * decreaseRate;
        }
    }

    void Reset()
    {
        ComboStarted = false;
        counterImage.fillAmount = 1;
        //comboCountText.transform.localScale = Vector3.one;
        imageBackground.transform.localScale = Vector3.one;
        canvasGroup.DOFade(0f, 0f);
    }
}
