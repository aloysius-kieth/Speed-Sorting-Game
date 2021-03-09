using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// IDs of all inputFields
/// </summary>
/// 
public enum FIELD_ID
{
    // Adjust as needed
    SERVER_IP,
    IDLE_INTERVAL,
    GAME_DURATION,
    HUAT_KUEH_POINTS,
    ANG_PAO_POINTS,
    MANDARIN_ORANGE_POINTS,
    PINEAPPLE_TART_POINTS,
    CHUN_LIAN_POINTS,
    FIRECRACKER_POINTS,
    COMBO_DECREASE_RATE,
    SPAWN_NUMBER_HUATKUEH,
    SPAWN_NUMBER_ANGPAO,
    MINIMUM_POINTS,
    OBJECT_TOTAL_AMOUNT,
    NON_SCOREABLE_AMT_MIN,
    NON_SCOREABLE_AMT_MAX,
    SPAWN_DURATION,
}

/// <summary>
/// IDs of all toggles
/// </summary>
public enum TOGGLE_ID
{
    // Adjust as needed
    USE_SERVER,
    USE_MOCKY,
    USE_KEYBOARD,
    MUTE_SOUND,
    USE_LOCALLEADERBOARD,
}

/// <summary>
/// IDs of all sliders
/// </summary>
public enum SLIDER_ID
{
    // Adjust as needed
    MASTER,
    MUSIC,
    SFX,
    SFX2,
    SFX3,
    SFX4,
    UI_SFX,
    UI_SFX2,
}

/// <summary>
/// Admin Panel
/// </summary>
public class TrinaxAdminPanel : MonoBehaviour, IManager
{
    int executionPriority = 300;
    public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }

    public bool IsReady { get; set; }
    [Header("Adminpanel Pages")]
    public CanvasGroup[] pages;

    [Header("Display result feedback")]
    public TextMeshProUGUI result;
    public GameObject resultOverlay;

    [Header("InputFields")]
    public TMP_InputField[] inputFields;

    [Header("Toggles")]
    public Toggle[] toggles;

    [Header("Sliders")]
    public Slider[] sliders;
    public TextMeshProUGUI[] sliderValue;

    [Header("Panel Buttons")]
    public Button closeBtn;
    public Button submitBtn;
    public Button pageBtn;
    //public Button clearLocalsavefileBtn;
    //public Button reporterBtn;
    //public Button clearLB;
    //public Button trainingRoomBtn;
    //public Button mainBtn;

    Color red = Color.red;
    Color green = Color.green;

    int pageSelected = 0;

    async void Start()
    {
    }

    public async Task Init()
    {
        Debug.Log("Loading Admin panel...");
        IsReady = false;

        resultOverlay.SetActive(false);

        //toggles[(int)TOGGLE_ID.TRACK_SPINEBASE].onValueChanged.AddListener(delegate { OnSwitchTrackJoint(toggles[(int)TOGGLE_ID.TRACK_SPINEBASE]); });
        //toggles[(int)TOGGLE_ID.TRACK_HEAD].onValueChanged.AddListener(delegate { OnSwitchTrackJoint(toggles[(int)TOGGLE_ID.TRACK_HEAD]); });

        PopulateCurrentValues();
        //CycleThroughInputFields(selected);

        toggles[(int)TOGGLE_ID.MUTE_SOUND].onValueChanged.AddListener(delegate { OnmuteAudioListener(toggles[(int)TOGGLE_ID.MUTE_SOUND]); });

        toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD].onValueChanged.AddListener(delegate { OnEnableLocalLeaderboard(toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD]); });

        InitButtonListeners();

        IsReady = true;
        Debug.Log("Admin panel is loaded!");

        CycleThroughPages(pageSelected);
        gameObject.SetActive(false);
    }

    void InitButtonListeners()
    {
        closeBtn.onClick.AddListener(Close);
        submitBtn.onClick.AddListener(Submit);
        //clearLocalsavefileBtn.onClick.AddListener(MainManager.Instance.ClearLocalsaveFile);
        //mainBtn.onClick.AddListener(ToMain);
        //clearLB.onClick.AddListener(() => { LocalLeaderboardJson.Instance.Clear(); });
        //reporterBtn.onClick.AddListener(TrinaxCanvas.Instance.reporter.doShow);
        pageBtn.onClick.AddListener(() =>
        {
            pageSelected++;
            if (pageSelected >= pages.Length)
            {
                pageSelected = 0;
            }

            CycleThroughPages(pageSelected);
        });
    }

    void CycleThroughPages(int page)
    {
        int num = page;
        for (int i = 0; i < pages.Length; i++)
        {
            CanvasGroup cGrp = pages[i];
            if (i == num)
            {
                cGrp.interactable = true;
                cGrp.blocksRaycasts = true;
                cGrp.DOFade(1.0f, 0.25f);
            }
            else
            {
                cGrp.interactable = false;
                cGrp.blocksRaycasts = false;
                cGrp.DOFade(0.0f, 0.25f);
            }
        }
    }

    void Update()
    {
        UpdateSliderValueText();
        HandleInputs();
    }

    /// <summary>
    /// Updates slider text values.
    /// </summary>
    void UpdateSliderValueText()
    {
        sliderValue[(int)SLIDER_ID.MASTER].text = sliders[(int)SLIDER_ID.MASTER].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.MUSIC].text = sliders[(int)SLIDER_ID.MUSIC].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.SFX].text = sliders[(int)SLIDER_ID.SFX].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.SFX2].text = sliders[(int)SLIDER_ID.SFX2].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.SFX3].text = sliders[(int)SLIDER_ID.SFX3].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.SFX4].text = sliders[(int)SLIDER_ID.SFX4].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.UI_SFX].text = sliders[(int)SLIDER_ID.UI_SFX].value.ToString("0.0");
        sliderValue[(int)SLIDER_ID.UI_SFX2].text = sliders[(int)SLIDER_ID.UI_SFX2].value.ToString("0.0");
    }

    void OnmuteAudioListener(Toggle toggle)
    {
        TrinaxAudioManager.Instance.muteAudioListener(toggle.isOn);
    }

    void OnEnableLocalLeaderboard(Toggle toggle)
    {
        AppManager.Instance.uiManager.useLocalLeaderboard = toggle.isOn;
    }

    /// <summary>
    /// Handles all inputs.
    /// </summary>
    void HandleInputs()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Submit();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            resultOverlay.SetActive(false);
            Close();
        }
    }

    void PopulateGlobalValues()
    {
        inputFields[(int)FIELD_ID.SERVER_IP].text = TrinaxGlobal.Instance.globalSettings.IP.ToString();
        inputFields[(int)FIELD_ID.IDLE_INTERVAL].text = TrinaxGlobal.Instance.globalSettings.idleInterval.ToString();

        toggles[(int)TOGGLE_ID.USE_SERVER].isOn = TrinaxGlobal.Instance.globalSettings.useServer;
        toggles[(int)TOGGLE_ID.USE_MOCKY].isOn = TrinaxGlobal.Instance.globalSettings.useMocky;
        toggles[(int)TOGGLE_ID.USE_KEYBOARD].isOn = TrinaxGlobal.Instance.globalSettings.useKeyboard;
        toggles[(int)TOGGLE_ID.MUTE_SOUND].isOn = TrinaxGlobal.Instance.globalSettings.muteAudioListener;
        toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD].isOn = TrinaxGlobal.Instance.globalSettings.useLocalLeaderboard;
    }

    void PopulateGameValues()
    {
        inputFields[(int)FIELD_ID.GAME_DURATION].text = TrinaxGlobal.Instance.gameSettings.gameDuration.ToString();
        inputFields[(int)FIELD_ID.HUAT_KUEH_POINTS].text = TrinaxGlobal.Instance.gameSettings.huatkueh_points.ToString();
        inputFields[(int)FIELD_ID.ANG_PAO_POINTS].text = TrinaxGlobal.Instance.gameSettings.angpao_points.ToString();
        inputFields[(int)FIELD_ID.MANDARIN_ORANGE_POINTS].text = TrinaxGlobal.Instance.gameSettings.mandarinOrange_points.ToString();
        inputFields[(int)FIELD_ID.PINEAPPLE_TART_POINTS].text = TrinaxGlobal.Instance.gameSettings.pineappleTart_points.ToString();
        inputFields[(int)FIELD_ID.CHUN_LIAN_POINTS].text = TrinaxGlobal.Instance.gameSettings.chunLian_points.ToString();
        inputFields[(int)FIELD_ID.FIRECRACKER_POINTS].text = TrinaxGlobal.Instance.gameSettings.firecracker_points.ToString();
        inputFields[(int)FIELD_ID.COMBO_DECREASE_RATE].text = TrinaxGlobal.Instance.gameSettings.comboDecreaseRate.ToString();
        inputFields[(int)FIELD_ID.SPAWN_NUMBER_HUATKUEH].text = TrinaxGlobal.Instance.gameSettings.numberOfHuatKuehToSpawn.ToString();
        inputFields[(int)FIELD_ID.SPAWN_NUMBER_ANGPAO].text = TrinaxGlobal.Instance.gameSettings.numberOfAngPaoToSpawn.ToString();
        inputFields[(int)FIELD_ID.MINIMUM_POINTS].text = TrinaxGlobal.Instance.gameSettings.minmumPoints.ToString();
        inputFields[(int)FIELD_ID.OBJECT_TOTAL_AMOUNT].text = TrinaxGlobal.Instance.gameSettings.objectTotalAmount.ToString();
        inputFields[(int)FIELD_ID.NON_SCOREABLE_AMT_MIN].text = TrinaxGlobal.Instance.gameSettings.non_scoreable_amt_min.ToString();
        inputFields[(int)FIELD_ID.NON_SCOREABLE_AMT_MAX].text = TrinaxGlobal.Instance.gameSettings.non_scoreable_amt_max.ToString();
        inputFields[(int)FIELD_ID.SPAWN_DURATION].text = TrinaxGlobal.Instance.gameSettings.spawnDuration.ToString();
    }

    /// <summary>
    /// Sets current values to fields.
    /// </summary>
    void PopulateCurrentValues()
    {
        PopulateGlobalValues();
        PopulateGameValues();
    }

    void UpdateSaveValues()
    {
        GlobalSettings globalSettings = new GlobalSettings
        {
            IP = inputFields[(int)FIELD_ID.SERVER_IP].text.Trim(),
            idleInterval = float.Parse(inputFields[(int)FIELD_ID.IDLE_INTERVAL].text.Trim()),

            useServer = toggles[(int)TOGGLE_ID.USE_SERVER].isOn,
            useMocky = toggles[(int)TOGGLE_ID.USE_MOCKY].isOn,
            useKeyboard = toggles[(int)TOGGLE_ID.USE_KEYBOARD].isOn,
            muteAudioListener = toggles[(int)TOGGLE_ID.MUTE_SOUND].isOn,
            useLocalLeaderboard = toggles[(int)TOGGLE_ID.USE_LOCALLEADERBOARD].isOn,
        };

        GameSettings gameSettings = new GameSettings
        {
            gameDuration = int.Parse(inputFields[(int)FIELD_ID.GAME_DURATION].text.Trim()),
            huatkueh_points = int.Parse(inputFields[(int)FIELD_ID.HUAT_KUEH_POINTS].text.Trim()),
            angpao_points = int.Parse(inputFields[(int)FIELD_ID.ANG_PAO_POINTS].text.Trim()),
            mandarinOrange_points = int.Parse(inputFields[(int)FIELD_ID.MANDARIN_ORANGE_POINTS].text.Trim()),
            pineappleTart_points = int.Parse(inputFields[(int)FIELD_ID.PINEAPPLE_TART_POINTS].text.Trim()),
            chunLian_points = int.Parse(inputFields[(int)FIELD_ID.CHUN_LIAN_POINTS].text.Trim()),
            firecracker_points = int.Parse(inputFields[(int)FIELD_ID.FIRECRACKER_POINTS].text.Trim()),
            comboDecreaseRate = float.Parse(inputFields[(int)FIELD_ID.COMBO_DECREASE_RATE].text.Trim()),
            numberOfHuatKuehToSpawn = int.Parse(inputFields[(int)FIELD_ID.SPAWN_NUMBER_HUATKUEH].text.Trim()),
            numberOfAngPaoToSpawn = int.Parse(inputFields[(int)FIELD_ID.SPAWN_NUMBER_ANGPAO].text.Trim()),
            minmumPoints = int.Parse(inputFields[(int)FIELD_ID.MINIMUM_POINTS].text.Trim()),
            objectTotalAmount = int.Parse(inputFields[(int)FIELD_ID.OBJECT_TOTAL_AMOUNT].text.Trim()),
            non_scoreable_amt_min = int.Parse(inputFields[(int)FIELD_ID.NON_SCOREABLE_AMT_MIN].text.Trim()),
            non_scoreable_amt_max = int.Parse(inputFields[(int)FIELD_ID.NON_SCOREABLE_AMT_MAX].text.Trim()),
            spawnDuration = int.Parse(inputFields[(int)FIELD_ID.SPAWN_DURATION].text.Trim()),
        };

        TrinaxGlobal.Instance.globalSettings = globalSettings;
        TrinaxGlobal.Instance.gameSettings = gameSettings;
    }

    /// <summary>
    /// Saves the value to respective fields.
    /// </summary>
    void Submit()
    {
        string resultText = "Empty";
        if (string.IsNullOrEmpty(inputFields[(int)FIELD_ID.SERVER_IP].text.Trim()))
        {
            Debug.Log("Mandatory fields in admin panel is empty!");
            ShowResultOverlay(false);
            result.color = red;
            resultText = "Need to fill mandatory fields!";
        }
        else
        {
            ShowResultOverlay(false);
            result.color = green;
            resultText = "Success!";

            UpdateSaveValues();
            TrinaxSaveManager.Instance.SaveJson();

            TrinaxGlobal.Instance.RefreshSettings();
            //TrinaxArduino.Instance.Restart();
        }

        result.text = resultText;
        result.gameObject.SetActive(true);
    }

    public async void ShowResultOverlay(bool immediate)
    {
        resultOverlay.SetActive(true);

        if (!immediate)
            await new WaitForSeconds(2f);

        resultOverlay.SetActive(false);
    }

    /// <summary>
    /// Closes admin panel.
    /// </summary>
    void Close()
    {
        gameObject.SetActive(false);
    }
}
