using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum RESULT_SCENARIO
    {
        NONE, //  did not meet minimum score
        MET_MINIMUM_SCORE, // met minimum score
        ENTER_LEADERBOARD, // met minimum score and entered leaderboard
    }

    public bool IsReady { get; set; }
    public bool IsGameover { get; set; }

    [Header("Component Reference")]
    public ObjectManager objectManager;
    public PlayerInputHandler inputHandler;
    public ComboCounter comboCounter;
    public GameTimer gameTimer;
    public CountdownTimer countdownTimer;

    public int MINIMUM_SCORE = 50;
    public RESULT_SCENARIO result_Scenario = RESULT_SCENARIO.NONE;

    void Start()
    {
        IsReady = false;
    }

    // Initialize method 
    public async void Init()
    {
        IsGameover = true;

        inputHandler.Init();
        objectManager.Init();
        gameTimer.Init();
        countdownTimer.Init();

        GameEvents.OnGameoverEvent += OnGameoverEvent;
        GameEvents.OnGameStartEvent += OnGameStartEvent;
        GameEvents.OnGameTimerEndEvent += OnGameTimerEndEvent;
        GameEvents.OnCountdownStartEvent += OnCountdownStartEvent;
        GameEvents.OnCountdownEndEvent += OnCountdownEndEvent;
        GameEvents.OnScoredEvent += OnScoredEvent;
        GameEvents.GameResetEvent += GameResetEvent;

        Debug.Log("<color=green> GameManager is ready! </color>");

        IsReady = true; 
    }

    public void PopulateSettings(GameSettings settings)
    {
        MINIMUM_SCORE = settings.minmumPoints;
        gameTimer.Populate(settings);
        objectManager.Populate(settings);
        comboCounter.Populate(settings);
    }

    void OnGameoverEvent()
    {
        IsGameover = true;

        GetResultScenario().WrapErrors();
    }

    async void OnGameStartEvent()
    {
        await new WaitForSeconds(1.0f);
        TrinaxAudioManager.Instance.PlayMusic(TrinaxAudioManager.AUDIOS.GAME_BGM, true);
        IsGameover = false;
        gameTimer.StartTimer();
    }

    void OnGameTimerEndEvent()
    {
        TrinaxAudioManager.Instance.ImmediateStopMusic();
        objectManager.ReturnAllToPool();
        for (int i = 0; i < AppManager.Instance.uiManager.pockets.Length; i++)
        {
            AppManager.Instance.uiManager.pockets[i].gameObject.SetActive(false);
        }

        GameEvents.OnGameoverEvent?.Invoke();
    }

    void OnCountdownStartEvent()
    {
        countdownTimer.OnActiveWithAnimator();
    }

    void OnCountdownEndEvent()
    {
        GameEvents.OnGameStartEvent?.Invoke();
    }

    void OnScoredEvent(OBJECT_TYPE type)
    {
        objectManager.UpdateScoreableAmount(type);
    }

    void GameResetEvent()
    {
        // Reset score
        AppManager.Instance.scoreManager.Reset();
        AppManager.Instance.gameManager.gameTimer.Reset();
        TrinaxGlobal.Instance.userData.Clear();
    }

    RESULT_SCENARIO result;
    async Task GetResultScenario()
    {
#if UNITY_EDITOR
        AppManager.Instance.scoreManager.Score = 1006;
#endif
        if (!AppManager.Instance.uiManager.useLocalLeaderboard)
        {
            Debug.Log("using database result");
            string r = await APICalls.RunGetResult(AppManager.Instance.scoreManager.Score);
            result = (RESULT_SCENARIO) System.Enum.Parse(typeof(RESULT_SCENARIO), r);
        }
        else
        {
            Debug.Log("Using local result");
            if (AppManager.Instance.scoreManager.Score >= MINIMUM_SCORE)
            {
                if (AppManager.Instance.scoreManager.Score > AppManager.Instance.localLeaderboard.GetLastEntryScore())
                    result = RESULT_SCENARIO.ENTER_LEADERBOARD;
                else result = RESULT_SCENARIO.MET_MINIMUM_SCORE;
            }
            else
                result = RESULT_SCENARIO.NONE;
        }
        Debug.Log(result);
        AppManager.Instance.uiManager.ToResults(result);
    }
}
