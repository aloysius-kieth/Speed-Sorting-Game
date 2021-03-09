using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TrinaxSaveManager : MonoBehaviour, IManager
{
    #region SINGLETON
    public static TrinaxSaveManager Instance { get; set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }
    #endregion

    int executionPriority = 0;
    public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }
    public bool IsReady { get; set; }

    public TrinaxSaves saveObj;
    const string ADMINSAVEFILE = "adminsave.json";

    async void Start()
    {

    }

    public async Task Init()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.loadNow);
        Debug.Log("Loading TrinaxSaveManager...");
        IsReady = false;
        LoadJson();
        IsReady = true;
        Debug.Log("TrinaxSaveManager is loaded!");
    }

    TrinaxSaves CreateAdminSave()
    {
        GameSettings saveGameSettings = new GameSettings
        {
            gameDuration = TrinaxGlobal.Instance.gameSettings.gameDuration,

            huatkueh_points = TrinaxGlobal.Instance.gameSettings.huatkueh_points,
            angpao_points = TrinaxGlobal.Instance.gameSettings.angpao_points,
            mandarinOrange_points = TrinaxGlobal.Instance.gameSettings.mandarinOrange_points,
            pineappleTart_points = TrinaxGlobal.Instance.gameSettings.pineappleTart_points,
            chunLian_points = TrinaxGlobal.Instance.gameSettings.chunLian_points,
            firecracker_points = TrinaxGlobal.Instance.gameSettings.firecracker_points,

            comboDecreaseRate = TrinaxGlobal.Instance.gameSettings.comboDecreaseRate,
            numberOfHuatKuehToSpawn = TrinaxGlobal.Instance.gameSettings.numberOfHuatKuehToSpawn,
            numberOfAngPaoToSpawn = TrinaxGlobal.Instance.gameSettings.numberOfAngPaoToSpawn,
            minmumPoints = TrinaxGlobal.Instance.gameSettings.minmumPoints,
            objectTotalAmount = TrinaxGlobal.Instance.gameSettings.objectTotalAmount,
            non_scoreable_amt_min = TrinaxGlobal.Instance.gameSettings.non_scoreable_amt_min,
            non_scoreable_amt_max = TrinaxGlobal.Instance.gameSettings.non_scoreable_amt_max,
            spawnDuration = TrinaxGlobal.Instance.gameSettings.spawnDuration,
        };

        GlobalSettings saveGlobalSettings = new GlobalSettings
        {
            IP = TrinaxGlobal.Instance.globalSettings.IP,
            //COMPORT1 = TrinaxGlobal.Instance.globalSettings.COMPORT1,
            //COMPORT2 = TrinaxGlobal.Instance.globalSettings.COMPORT2,
            idleInterval = TrinaxGlobal.Instance.globalSettings.idleInterval,

            useServer = TrinaxGlobal.Instance.globalSettings.useServer,
            useMocky = TrinaxGlobal.Instance.globalSettings.useMocky,
            useKeyboard = TrinaxGlobal.Instance.globalSettings.useKeyboard,
            muteAudioListener = TrinaxAudioManager.Instance._muteAudioListener,
            useLocalLeaderboard = TrinaxGlobal.Instance.globalSettings.useLocalLeaderboard,
        };

        AudioConfig saveAudioSettings = new AudioConfig
        {
            masterVolume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MASTER].slider.value,
            musicVolume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MUSIC].slider.value,
            SFXVolume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX].slider.value,
            SFX2Volume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX2].slider.value,
            SFX3Volume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX3].slider.value,
            SFX4Volume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX4].slider.value,
            UI_SFXVolume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX].slider.value,
            UI_SFX2Volume = TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX2].slider.value,
        };

        //KinectSettings saveKinectSettings = new KinectSettings
        //{
        //    isTrackingBody = TrinaxGlobal.Instance.kinectSettings.isTrackingBody,
        //    isTrackingHead = TrinaxGlobal.Instance.kinectSettings.isTrackingHead,
        //};

        TrinaxSaves save = new TrinaxSaves
        {
            gameSettings = saveGameSettings,
            globalSettings = saveGlobalSettings,
            audioSettings = saveAudioSettings,
            //kinectSettings = saveKinectSettings,
        };    

        return save;
    }

    public void SaveJson()
    {
        saveObj = CreateAdminSave();

        string saveJsonString = JsonUtility.ToJson(saveObj, true);

        JsonFileUtility.WriteJsonToFile(ADMINSAVEFILE, saveJsonString, JSONSTATE.PERSISTENT_DATA_PATH);
        Debug.Log("Saving as JSON " + saveJsonString);
    }

    void PopulateGlobalSettings()
    {
        TrinaxGlobal.Instance.globalSettings.IP = saveObj.globalSettings.IP;
        //TrinaxGlobal.Instance.globalSettings.COMPORT1 = saveObj.globalSettings.COMPORT1;
        //TrinaxGlobal.Instance.globalSettings.COMPORT2 = saveObj.globalSettings.COMPORT2;
        TrinaxGlobal.Instance.globalSettings.idleInterval = saveObj.globalSettings.idleInterval;

        TrinaxGlobal.Instance.globalSettings.useServer = saveObj.globalSettings.useServer;
        TrinaxGlobal.Instance.globalSettings.useMocky = saveObj.globalSettings.useMocky;
        TrinaxGlobal.Instance.globalSettings.useKeyboard = saveObj.globalSettings.useKeyboard;
        TrinaxGlobal.Instance.globalSettings.muteAudioListener = saveObj.globalSettings.muteAudioListener;
        TrinaxGlobal.Instance.globalSettings.useLocalLeaderboard = saveObj.globalSettings.useLocalLeaderboard;
    }

    void PopulateGameSettings()
    {
        TrinaxGlobal.Instance.gameSettings.gameDuration = saveObj.gameSettings.gameDuration;
        TrinaxGlobal.Instance.gameSettings.huatkueh_points = saveObj.gameSettings.huatkueh_points;
        TrinaxGlobal.Instance.gameSettings.angpao_points = saveObj.gameSettings.angpao_points;
        TrinaxGlobal.Instance.gameSettings.mandarinOrange_points = saveObj.gameSettings.mandarinOrange_points;
        TrinaxGlobal.Instance.gameSettings.pineappleTart_points = saveObj.gameSettings.pineappleTart_points;
        TrinaxGlobal.Instance.gameSettings.chunLian_points = saveObj.gameSettings.chunLian_points;
        TrinaxGlobal.Instance.gameSettings.firecracker_points = saveObj.gameSettings.firecracker_points;
        TrinaxGlobal.Instance.gameSettings.comboDecreaseRate = saveObj.gameSettings.comboDecreaseRate;
        TrinaxGlobal.Instance.gameSettings.numberOfHuatKuehToSpawn = saveObj.gameSettings.numberOfHuatKuehToSpawn;
        TrinaxGlobal.Instance.gameSettings.numberOfAngPaoToSpawn = saveObj.gameSettings.numberOfAngPaoToSpawn;
        TrinaxGlobal.Instance.gameSettings.minmumPoints = saveObj.gameSettings.minmumPoints;
        TrinaxGlobal.Instance.gameSettings.objectTotalAmount = saveObj.gameSettings.objectTotalAmount;
        TrinaxGlobal.Instance.gameSettings.non_scoreable_amt_min = saveObj.gameSettings.non_scoreable_amt_min;
        TrinaxGlobal.Instance.gameSettings.non_scoreable_amt_max = saveObj.gameSettings.non_scoreable_amt_max;
        TrinaxGlobal.Instance.gameSettings.spawnDuration = saveObj.gameSettings.spawnDuration;
    }

    void PopulateAudioSettings()
    {
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MASTER].slider.value = saveObj.audioSettings.masterVolume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.MUSIC].slider.value = saveObj.audioSettings.musicVolume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX].slider.value = saveObj.audioSettings.SFXVolume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX2].slider.value = saveObj.audioSettings.SFX2Volume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX3].slider.value = saveObj.audioSettings.SFX3Volume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.SFX4].slider.value = saveObj.audioSettings.SFX4Volume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX].slider.value = saveObj.audioSettings.UI_SFXVolume;
        TrinaxAudioManager.Instance.audioSettings[(int)TrinaxAudioManager.AUDIOPLAYER.UI_SFX2].slider.value = saveObj.audioSettings.UI_SFX2Volume;

        TrinaxGlobal.Instance.audioSettings.musicVolume = saveObj.audioSettings.musicVolume;
    }

    //void PopulateKinectSettings()
    //{
    //    TrinaxGlobal.Instance.kinectSettings.isTrackingBody = saveObj.kinectSettings.isTrackingBody;
    //    TrinaxGlobal.Instance.kinectSettings.isTrackingHead = saveObj.kinectSettings.isTrackingHead;
    //}

    public void LoadJson()
    {
        string loadJsonString = JsonFileUtility.LoadJsonFromFile(ADMINSAVEFILE, JSONSTATE.PERSISTENT_DATA_PATH);
        saveObj = JsonUtility.FromJson<TrinaxSaves>(loadJsonString);

        // Assign our values back!
        if (saveObj != null)
        {
            PopulateGlobalSettings();
            PopulateGameSettings();
            PopulateAudioSettings();
            //PopulateKinectSettings();
        }
        else
        {
            Debug.Log("Json file is empty! Creating a new save file...");
            saveObj = CreateAdminSave();
        }
    }
}
