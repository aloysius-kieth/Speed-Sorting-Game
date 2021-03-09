using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrinaxSaves
{
    public GlobalSettings globalSettings;
    public GameSettings gameSettings;
    public AudioConfig audioSettings;
    //public KinectSettings kinectSettings;
}

[System.Serializable]
public struct GlobalSettings
{
    public string IP;
    public float idleInterval;

    public string COMPORT1;
    public string COMPORT2;

    public bool useServer;
    public bool useMocky;
    public bool useKeyboard;
    public bool muteAudioListener;
    public bool useLocalLeaderboard;
}

[System.Serializable]
public struct AudioConfig
{
    public float masterVolume;
    public float musicVolume;
    public float SFXVolume;
    public float SFX2Volume;
    public float SFX3Volume;
    public float SFX4Volume;
    public float UI_SFXVolume;
    public float UI_SFX2Volume;
}

[System.Serializable]
public struct GameSettings
{
    public int gameDuration;
    public int huatkueh_points;
    public int angpao_points;
    public int mandarinOrange_points;
    public int pineappleTart_points;
    public int chunLian_points;
    public int firecracker_points;

    public float comboDecreaseRate;
    public int numberOfHuatKuehToSpawn;
    public int numberOfAngPaoToSpawn;
    public int minmumPoints;
    public int objectTotalAmount;
    public int non_scoreable_amt_min;
    public int non_scoreable_amt_max;

    public float spawnDuration;
}

//[System.Serializable]
//public struct KinectSettings
//{
//    public bool isTrackingBody;
//    public bool isTrackingHead;
//}

#region INTERACTION
[System.Serializable]
public struct StartInteractionSendJsonData
{
    public string deviceID;
    public string projectID;
}

[System.Serializable]
public struct StartInteractionReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public string data;
}

//[System.Serializable]
//public class StartInteractionData
//{
//    //public string interactionID;
//    //public string deviceID;
//    public string projectID;
//    //public string createdDateTime;
//    //public string endDateTime;
//    //public string countryID;
//    //public string state;
//    //public string postalCode;
//    //public string engagement;
//    //public int status;
//}

[System.Serializable]
public struct InteractionDetailsSendJsonData
{
    public string interactionID;
    public string location;
    public string isPrint;
    public string isInfoCollect;
}

[System.Serializable]
public struct InteractionDetailsReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}

[System.Serializable]
public struct InteractionEndSendJsonData
{

}

[System.Serializable]
public struct InteractionEndReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}

#endregion

[System.Serializable]
public struct AddUserSendJsonData
{
    public string birthDate;
    public string location;
    public string language;
    public string interactionID;
}

[System.Serializable]
public struct AddUserReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}

[System.Serializable]
public struct GenerateQRReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public bool data;
}

#region ADD RESULT
[System.Serializable]
public struct AddResultSendJsonData
{
    public string name;
    public string mobile;
    public string score;
    //public string email;
}

[System.Serializable]
public struct AddResultReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public string data;
}
#endregion

#region GET RESULT
[System.Serializable]
public struct GetResultReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public string data;
}
#endregion

#region LEADERBOARD
[System.Serializable]
public struct LeaderboardReceiveJsonData
{
    public requestClass request;
    public errorClass error;
    public List<LeaderboardData> data;
}

[System.Serializable]
public struct LeaderboardData
{
    public string playerid;
    public string name;
    public string mobile;
    public int score;
}
#endregion

#region API DATA
[System.Serializable]
public struct requestClass
{
    public string api;
    public string result;
}

[System.Serializable]
public struct errorClass
{
    public string error_code;
    public string error_message;
}
#endregion

/// <summary>
/// For using playerPrefs with.
/// </summary>
public interface ITrinaxPersistantVars
{
    string ip { get; set; }
    string photoPath { get; set; }
    bool useServer { get; set; }
}

