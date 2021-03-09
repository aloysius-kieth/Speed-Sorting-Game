using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Use this for storing user's data
[System.Serializable]
public class UserData
{
    public string name = "";
    public string mobile = "";
    public string score = "";
    public string interactionID = "";

    public void Clear()
    {
        name = "";
        mobile = "";
        score = "";
    }
}

/// <summary>
/// Global Manager
/// </summary>
public class TrinaxGlobal : MonoBehaviour
{
    #region SINGLETON
    public static TrinaxGlobal Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion

    public bool IsAppPaused { get; set; }
    public bool isReady = false;
    public bool loadNow = false;
    public bool isChangingLevels = false;
    public bool loadedComponentReferences = false;

    public STATE state;
    public SCENE scene;

    public UserData userData = new UserData();

    TrinaxAdminPanel aP;
    public TrinaxSaveManager trinaxSaveManager;
    public TrinaxAudioManager trinaxAudioManager;
    public TrinaxAsyncServerManager trinaxAsyncServerManager;
    public TrinaxCanvas trinaxCanvas;
    public TrinaxKeyboardManager trinaxKeyboardManager;

    public List<KeyValuePair<IManager, int>> IManagers = new List<KeyValuePair<IManager, int>>();

    [Header("Settings")]
    public GlobalSettings globalSettings;
    public GameSettings gameSettings;
    public AudioConfig audioSettings;
    //public KinectSettings kinectSettings;

    public void RefreshSettings()
    {
        Debug.Log("Refresh settings!");
        if (TrinaxAsyncServerManager.Instance != null) TrinaxAsyncServerManager.Instance.PopulateValues(globalSettings);
        else
            Debug.LogWarning("<TrinaxAsyncServerManager> values not populated!");
        if (TrinaxAudioManager.Instance != null)
            TrinaxAudioManager.Instance.PopulateValues();
        else
            Debug.LogWarning("<TrinaxAudioManager> values not populated!");
        if (AppManager.Instance.gameManager != null)
            AppManager.Instance.gameManager.PopulateSettings(gameSettings);
        else
            Debug.LogWarning("<GameManager> values not populated!");
        if (AppManager.Instance.uiManager != null)
            AppManager.Instance.uiManager.PopulateSettings(globalSettings);
        else
            Debug.LogWarning("<UIManager> values not populated!");
        //if (TrinaxArduino.Instance != null)
        //    TrinaxArduino.Instance.Populate(globalSettings);
        //else
        //    Debug.LogWarning("<TrinaxArduino> values not populated!");
    }

    void Start()
    {
#if !UNITY_EDITOR
    Cursor.visible = false;
#endif
        scene = SCENE.MAIN;

        // Assign component references
        SetComponentReferences();
        // Collate all managers that derive from IManager
        AddIManagers();
        // Load our managers in order
        LoadManagers();
        // Assign settings and finally, we are ready to load our game logic
        Init();
    }

    #region Init Managers
    async void Init()
    {
        await new WaitUntil(() => !loadNow);

        // Indicate that everything is ready
        isReady = true;
        Debug.Log("All managers loaded!");

        // *** Here all managers should be fully loaded. Do whatever you want now! *** //

        if (string.IsNullOrEmpty(globalSettings.IP))
        {
            Debug.Log("Mandatory fields in admin panel not filled!" + "\n" + "Opening admin panel...");
            aP.gameObject.SetActive(true);
        }
        else
        {
            aP.gameObject.SetActive(false);
        }

        RefreshSettings();
    }

    void AddIManagers()
    {
        IManagers.Add(new KeyValuePair<IManager, int>(trinaxSaveManager, trinaxSaveManager.ExecutionPriority));
        IManagers.Add(new KeyValuePair<IManager, int>(trinaxAudioManager, trinaxAudioManager.ExecutionPriority));
        IManagers.Add(new KeyValuePair<IManager, int>(trinaxAsyncServerManager, trinaxAsyncServerManager.ExecutionPriority));
        IManagers.Add(new KeyValuePair<IManager, int>(trinaxCanvas.adminPanel, trinaxCanvas.adminPanel.ExecutionPriority));
        IManagers.Add(new KeyValuePair<IManager, int>(trinaxKeyboardManager, trinaxKeyboardManager.ExecutionPriority));

        // Sort by execution order
        IManagers.Sort((value1, value2) => value1.Value.CompareTo(value2.Value));

        //foreach (KeyValuePair<IManager, int> kvp in IManagers)
        //{
        //    Debug.Log(string.Format("Class: {0} | Order: {1}", kvp.Key, kvp.Value));
        //}
    }

    // Use this to await on managers to be loaded before able to call methods from it
    async void LoadManagers()
    {
        Debug.Log("Waiting for managers to be loaded...");
        loadNow = true;


        for (int i = 0; i < IManagers.Count; i++)
        {
            await IManagers[i].Key.Init();
            bool ready = IManagers[i].Key.IsReady;
            await new WaitUntil(() => ready);
        }
        //await new WaitUntil(() => trinaxSaveManager.IsReady);
        //await new WaitUntil(() => trinaxAudioManager.IsReady);
        //await new WaitUntil(() => trinaxAsyncServerManager.IsReady);
        //await new WaitUntil(() => trinaxCanvas.adminPanel.IsReady);
        //await new WaitUntil(() => trinaxKeyboardManager.IsReady);
        loadNow = false;
    }

    #endregion

    void Update()
    {
        if (Application.isPlaying)
        {
            if (Time.frameCount % 30 == 0)
                System.GC.Collect(1, System.GCCollectionMode.Optimized, false, false);
        }
    }

    // Set necessary component references
    void SetComponentReferences()
    {
        loadedComponentReferences = false;
        trinaxSaveManager = GetComponentInChildren<TrinaxSaveManager>();
        trinaxAudioManager = GetComponentInChildren<TrinaxAudioManager>();
        trinaxAsyncServerManager = GetComponentInChildren<TrinaxAsyncServerManager>();
        trinaxCanvas = GetComponentInChildren<TrinaxCanvas>();
        trinaxKeyboardManager = GetComponentInChildren<TrinaxKeyboardManager>();

        aP = trinaxCanvas.adminPanel;

        loadedComponentReferences = true;
    }
}
