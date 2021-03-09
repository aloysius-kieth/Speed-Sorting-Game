using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// After everything is initialized in global manager, actual game logic is initialized here
public class AppManager : MonoBehaviour
{
    #region SINGLETON
    public static AppManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public UIManager uiManager;
    [HideInInspector]
    public ScoreManager scoreManager;
    [HideInInspector]
    public LocalLeaderboard localLeaderboard;

    async void Start()
    {
        localLeaderboard = GetComponent<LocalLeaderboard>();
        gameManager = GetComponent<GameManager>();
        uiManager = GetComponent<UIManager>();
        scoreManager = GetComponent<ScoreManager>();

        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);

        // App start
        Execute();
    }

    async void Execute()
    {
        Debug.Log("App starting...");
        localLeaderboard.Init();
        await new WaitUntil(() => localLeaderboard.IsReady);
        Debug.Log("Done load localleaderboard");
        gameManager.Init();
        await new WaitUntil(() => gameManager.IsReady);
        Debug.Log("Done load gamemanager");
        uiManager.Init();
        await new WaitUntil(() => uiManager.IsReady);
        Debug.Log("Done load uimanager");
    }
}
