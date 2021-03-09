using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;

public class EnterDetails : MonoBehaviour
{
    [Header("Inputfields")]
    public TMP_InputField nameIF;
    public TMP_InputField contactIF;

    [Header("Buttons")]
    public Button submitButton;
    public Button skipButton;
    public Button noSkipButton;
    public Button yesSkipButton;

    [Header("Toggles")]
    public Toggle eyeToggle;
    public Toggle PDAToggle;
    public Image eyeToggleImg;

    [Header("Eye Toggle Sprites")]
    public Sprite[] toggleSprites;

    [Header("Popup box")]
    public SkipBox box;

    [Header("Keyboard")]
    public GameObject keyboard;
    public GameObject overlay;

    [Header("Text")]
    public TextMeshProUGUI infoHeader;

    DateTime date;
    string currentDate;

    void Start()
    {
        submitButton.onClick.AddListener(OnSubmit);
        skipButton.onClick.AddListener(EnablePopupbox);
        noSkipButton.onClick.AddListener(DisablePopupbox);
        yesSkipButton.onClick.AddListener(()=> {
            TrinaxAudioManager.Instance.PlayUISFX(TrinaxAudioManager.AUDIOS.BUTTON_CLICK, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
            AppManager.Instance.uiManager.ToScreensaver(SCREENSAVER_STATE.LEADERBOARD);
        });

        eyeToggle.onValueChanged.AddListener(delegate { OnToggleVisiblity(eyeToggle); });     
    }

    void OnEnable()
    {
        date = DateTime.Now;
        currentDate = date.ToString("dd/MM/yyyy");
        infoHeader.text = string.Format("As of {0}, you are one of the Top 5 scorers of “Fortune Wins”. If your record remains undefeated by the end of the campaign period, you will receive a bonus prize! All winners will be contacted via the mobile number provided.", currentDate);
        keyboard.transform.localPosition = new Vector3(0, -830f);
        keyboard.SetActive(false);
        overlay.SetActive(false);

        if (!box.gameObject.activeSelf)
            box.gameObject.SetActive(true);

        eyeToggle.isOn = true;
        PDAToggle.isOn = false;
        eyeToggleImg.sprite = toggleSprites[0];
        eyeToggleImg.SetNativeSize();
        contactIF.contentType = TMP_InputField.ContentType.Password;

        nameIF.text = "";
        contactIF.text = "";
    }

    async void OnSubmit()
    {
        bool validInput = true;

        if (!CredentialsValidator.validateName(nameIF.text))
        {
            validInput = false;
            submitButton.interactable = false;

            nameIF.image.DOColor(Color.red, 0.25f);
            nameIF.transform.DOShakePosition(0.25f, new Vector3(50, 0), 5, 90, false, true).OnComplete(() => {
                nameIF.image.DOColor(Color.white, 0.25f);
                submitButton.interactable = true;
            });
        }

        if (!CredentialsValidator.validateMobile(contactIF.text))
        {
            validInput = false;
            submitButton.interactable = false;

            contactIF.image.DOColor(Color.red, 0.25f);
            contactIF.transform.DOShakePosition(0.25f, new Vector3(50, 0), 5, 90, false, true).OnComplete(()=> {
                contactIF.image.DOColor(Color.white, 0.25f);
                submitButton.interactable = true;
            });
        }

        if(!PDAToggle.isOn)
        {
            validInput = false;
            submitButton.interactable = false;
            PDAToggle.image.DOColor(Color.red, 0.25f);
            PDAToggle.transform.DOShakePosition(0.25f, new Vector3(50, 0), 5, 90, false, true).OnComplete(() => {
                PDAToggle.image.DOColor(Color.white, 0.25f);
                submitButton.interactable = true;
            });
        }

        if (validInput)
        {
            TrinaxAudioManager.Instance.PlayUISFX(TrinaxAudioManager.AUDIOS.BUTTON_CLICK, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
            PlayerInfo pInfo = new PlayerInfo
            {
                name = nameIF.text,
                score = AppManager.Instance.scoreManager.Score,
                mobile = contactIF.text,
            };

            AppManager.Instance.localLeaderboard.Save(pInfo);

            TrinaxGlobal.Instance.userData.name = nameIF.text;
            TrinaxGlobal.Instance.userData.score = AppManager.Instance.scoreManager.Score.ToString();
            TrinaxGlobal.Instance.userData.mobile = contactIF.text;


            await APICalls.RunAddGameResult();

            AppManager.Instance.uiManager.backgroundCanvas.Activate(0, true);
            AppManager.Instance.uiManager.ToScreensaver(SCREENSAVER_STATE.LEADERBOARD);
        }
        else
        {
            TrinaxAudioManager.Instance.PlayUISFX(TrinaxAudioManager.AUDIOS.INVALID, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
        }
    }

    void EnablePopupbox()
    {
        TrinaxAudioManager.Instance.PlayUISFX(TrinaxAudioManager.AUDIOS.BUTTON_CLICK, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
        box.Activate();
    }

    void DisablePopupbox()
    {
        TrinaxAudioManager.Instance.PlayUISFX(TrinaxAudioManager.AUDIOS.BUTTON_CLICK, TrinaxAudioManager.AUDIOPLAYER.UI_SFX);
        box.Deactivate();
    }

    void OnToggleVisiblity(Toggle toggle)
    {
        if (toggle.isOn)
        {
            eyeToggleImg.sprite = toggleSprites[0];
            contactIF.contentType = TMP_InputField.ContentType.Password;
        }
        else
        {
            eyeToggleImg.sprite = toggleSprites[1];
            contactIF.contentType = TMP_InputField.ContentType.IntegerNumber;
        }
        contactIF.ForceLabelUpdate();
        eyeToggleImg.SetNativeSize();
    }
}
