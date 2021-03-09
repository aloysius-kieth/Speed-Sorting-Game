using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public bool IsReady { get; set; }
    public bool useLocalLeaderboard = true;

    float idleTimer;
    float idleDuration = 30f;

    float durationToTransit = 0.25f;

    [Header("Buttons")]
    public Button mainMenuButton;
    public Button nextTermsButton;
    public Button backTermsButton;
    public Button nextInstructionsButton;
    public Button backInstructionsButton;
    public Button nextHandSelectionButton;
    public Button backHandSelectionButton;

    public Toggle rightHandSelectionToggle;
    public Toggle leftHandSelectionToggle;

    [Header("Text")]
    public TextMeshProUGUI resultScoreText;
    public TextMeshProUGUI thankyouPageScoreText;

    [Header("Component References")]
    public Pocket[] pockets;
    public ScreensaverController screensaverController;
    public BackgroundCanvas backgroundCanvas;
    public HUD hud;
    public ScreenBoundary screenBoundary;

    CanvasController canvasController;

    void Start()
    {
        IsReady = false;
        canvasController = GetComponent<CanvasController>();
    }

    // Initialize method 
    public async void Init()
    {
        InitButtonListeners();
        backgroundCanvas.Deactivate(true);

        //await new WaitForSeconds(5);
        IsReady = true;
        Debug.Log("<color=green> UIManager is ready! </color>");

        // Only called on first run
        canvasController.OnStartup(STATE.SCREENSAVER, () => {
            GameEvents.GameResetEvent?.Invoke();
            TrinaxAudioManager.Instance.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE_BGM, true);
            if(!useLocalLeaderboard)
                APICalls.RunPopulateLeaderboard(AppManager.Instance.localLeaderboard.leaderboardDisplay).WrapErrors();
        });
    }

    public void PopulateSettings(GlobalSettings settings)
    {
        idleDuration = settings.idleInterval;
        useLocalLeaderboard = settings.useLocalLeaderboard;
    }

    void InitButtonListeners()
    {
        mainMenuButton.onClick.AddListener(() => { ToTermsConditions(); });
        nextTermsButton.onClick.AddListener(() => { ToInstructions(); });
        backTermsButton.onClick.AddListener(() => { ToScreensaver(); });
        nextInstructionsButton.onClick.AddListener(() => { ToHandsSelection(); });
        backInstructionsButton.onClick.AddListener(() => { ToTermsConditions(); });
        nextHandSelectionButton.onClick.AddListener(() => { ToGame(); });
        backHandSelectionButton.onClick.AddListener(() => { ToInstructions(); });

        rightHandSelectionToggle.onValueChanged.AddListener(delegate {
            OnHandSelectionToggleValueChanged(rightHandSelectionToggle);
        });
    }

    void OnHandSelectionToggleValueChanged(Toggle toggle)
    {
        toggle = rightHandSelectionToggle;
        if (toggle.isOn)
        {
            screenBoundary.SetPosition(true);
            hud.SetPosition(true);
        }
        else
        {
            screenBoundary.SetPosition(false);
            hud.SetPosition(false);
        }
    }

    void Update()
    {
        if (!IsReady) return;

        if (TrinaxGlobal.Instance.state == STATE.INSTRUCTIONS)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > idleDuration)
            {
                idleTimer = 0;
                ReachedIdleDuration();
            }
        }

        if (Input.anyKeyDown) idleTimer = 0;

    }

    void ReachedIdleDuration()
    {
        ToScreensaver(SCREENSAVER_STATE.MAIN_MENU);
    }

    void SetActivePocket(bool isRight)
    {
        if (isRight)
        {
            pockets[0].gameObject.SetActive(true);
            pockets[1].gameObject.SetActive(false);
        }
        else
        {
            pockets[0].gameObject.SetActive(false);
            pockets[1].gameObject.SetActive(true);
        }
    }

    #region UI PAGES
    public void ToScreensaver(SCREENSAVER_STATE _state = SCREENSAVER_STATE.MAIN_MENU)
    {
        GameEvents.GameResetEvent?.Invoke();

        //backgroundCanvas.Deactivate();

        screensaverController.state = _state;
        TrinaxGlobal.Instance.state = STATE.SCREENSAVER;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit);

        if (!useLocalLeaderboard)
            APICalls.RunPopulateLeaderboard(AppManager.Instance.localLeaderboard.leaderboardDisplay).WrapErrors();
        if (!string.IsNullOrEmpty(TrinaxGlobal.Instance.userData.interactionID))
            APICalls.RunEndInteraction().WrapErrors();
        else return;
    }

    void ToTermsConditions()
    {
        backgroundCanvas.Activate(0);

        TrinaxGlobal.Instance.state = STATE.TERMS_CONDITIONS;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit);

        APICalls.RunStartInteraction().WrapErrors();
    }

    void ToInstructions()
    {
        TrinaxGlobal.Instance.state = STATE.INSTRUCTIONS;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit);
    }

    void ToHandsSelection()
    {
        rightHandSelectionToggle.isOn = true;
        leftHandSelectionToggle.isOn = false;

        TrinaxGlobal.Instance.state = STATE.HAND_SELECTION;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit);
    }

    void ToGame()
    {
        TrinaxAudioManager.Instance.StopMusic(0.5f);
        backgroundCanvas.Deactivate(true);
        backgroundCanvas.Activate(1, true);

        TrinaxGlobal.Instance.state = STATE.GAME;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit, () =>
        {
            AppManager.Instance.gameManager.objectManager.Spawn();
            SetActivePocket(rightHandSelectionToggle.isOn);
        });
    }

    public void ToResults(GameManager.RESULT_SCENARIO resultScenario)
    {
        TrinaxGlobal.Instance.state = STATE.RESULT;
        // Set result score text;
        resultScoreText.text = AppManager.Instance.scoreManager.Score.ToString();

        switch (resultScenario)
        {
            case GameManager.RESULT_SCENARIO.NONE:
                TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.LOSE, TrinaxAudioManager.AUDIOPLAYER.SFX, () => {
                    TrinaxAudioManager.Instance.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE_BGM, true);
                });
                ToThankyou(8f);
                break;
            case GameManager.RESULT_SCENARIO.MET_MINIMUM_SCORE:
                canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit, async () =>
                {
                    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.WIN, TrinaxAudioManager.AUDIOPLAYER.SFX, () => {
                        TrinaxAudioManager.Instance.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE_BGM, true);
                    });
                    await new WaitForSeconds(8f);
                    backgroundCanvas.Activate(0, true);
                    APICalls.RunEndInteraction().WrapErrors();
                    ToScreensaver(SCREENSAVER_STATE.LEADERBOARD);
                });

                break;
            case GameManager.RESULT_SCENARIO.ENTER_LEADERBOARD:
                canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit, async () =>
                {
                    TrinaxAudioManager.Instance.PlaySFX(TrinaxAudioManager.AUDIOS.WIN, TrinaxAudioManager.AUDIOPLAYER.SFX, ()=> {
                        TrinaxAudioManager.Instance.PlayMusic(TrinaxAudioManager.AUDIOS.IDLE_BGM, true);
                    });
                    await new WaitForSeconds(8f);
                    ToEnterDetails();
                });
                break;
        }
    }

    void ToEnterDetails()
    {
        TrinaxGlobal.Instance.state = STATE.ENTER_DETAILS;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit);
    }

    async void ToThankyou(float duration)
    {
        thankyouPageScoreText.text = AppManager.Instance.scoreManager.Score.ToString();

        TrinaxGlobal.Instance.state = STATE.THANKYOU;
        canvasController.TransitToCanvas((int)TrinaxGlobal.Instance.state, durationToTransit);

        await new WaitForSeconds(duration);

        backgroundCanvas.Activate(0, true);
        APICalls.RunEndInteraction().WrapErrors();
        ToScreensaver(SCREENSAVER_STATE.LEADERBOARD);
    }
    #endregion


}
