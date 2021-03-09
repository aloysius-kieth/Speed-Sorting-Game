using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public static class APICalls
{
    #region APICALLS

    const string DEVICEID = "001";
    const string PROJECTID = "961202001021704311941311432835943508b7ed2ba2-9546-459a-9b0b-d94dd7418d34";
    public static async Task RunStartInteraction()
    {
        StartInteractionSendJsonData sJson = new StartInteractionSendJsonData
        {
            deviceID = DEVICEID,
            projectID = PROJECTID,
        };

        await TrinaxAsyncServerManager.Instance.StartInteraction(sJson, (bool success, StartInteractionReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (rJson.data != null)
                {
                    TrinaxGlobal.Instance.userData.interactionID = rJson.data;
                    Debug.Log("Started interaction!");
                }
            }
            else
            {
                Debug.Log("Error in <StartInteraction> API!");
            }
        });
    }

    public static async Task RunAddInteraction()
    {
        InteractionDetailsSendJsonData sJson = new InteractionDetailsSendJsonData
        {
            interactionID = TrinaxGlobal.Instance.userData.interactionID,
            location = "Game Page",
        };

        await TrinaxAsyncServerManager.Instance.AddInteractionDetails(sJson, (bool success, InteractionDetailsReceiveJsonData rJson) =>
        {
            if (success)
            {
                Debug.Log("Added interaction!");
            }
            else
            {
                Debug.Log("Error in <AddInteraction> API!");
            }
        });
    }

    public static async Task RunEndInteraction()
    {
        InteractionEndSendJsonData sJson = new InteractionEndSendJsonData();

        await TrinaxAsyncServerManager.Instance.EndInteraction(sJson, (bool success, InteractionEndReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (rJson.data)
                {
                    Debug.Log("End interaction!");
                    TrinaxGlobal.Instance.userData.interactionID = "";
                }
            }
            else
            {
                Debug.Log("Error in <EndInteraction> API!");
            }
        });
    }

    //public static async Task GetInventoryCount()
    //{
    //    // disable start buttonn first
    //    await TrinaxAsyncServerManager.Instance.GetInventoryCount((bool success, InventoryCountReceiveJsonData rJson) =>
    //    {
    //        if (success)
    //        {
    //            if (rJson.data > 0)
    //            {
    //                Debug.Log(string.Format("Inventory Count: {0}", rJson.data));
    //                GameManager.Instance.startGameBtn.interactable = true;
    //            }
    //            else
    //            {
    //                GameManager.Instance.startGameBtn.interactable = false;
    //            }
    //        }
    //    });
    //}

    //public static async Task AddPlayer()
    //{
    //    AddPlayerSendJsonData sJson = new AddPlayerSendJsonData();
    //    await TrinaxAsyncServerManager.Instance.AddPlayer(sJson, (bool success, AddPlayerReceiveJsonData rJson) =>
    //    {
    //        if (success && !string.IsNullOrEmpty(rJson.data))
    //        {
    //            GameManager.Instance.PLAYER_ID = rJson.data;
    //            Debug.Log(string.Format("Send: {0}, {1}, {2}", GameManager.Instance.firstName, GameManager.Instance.lastName, GameManager.Instance.scoreManager.ScoredPairs));

    //            APICalls.GetResult().WrapErrors();
    //        }
    //        else
    //        {
    //            Debug.LogWarning("ERROR: Did not add player to table!");
    //        }
    //    });
    //}

    //public static async Task GetResult()
    //{
    //    await TrinaxAsyncServerManager.Instance.GetResult((bool success, GetResultReceiveJsonData rJson) =>
    //    {
    //        if (success)
    //        {
    //            if (rJson.data)
    //            {
    //                Debug.Log("Player only did not get gift twice! Dispensing gift...");
    //                GameManager.Instance.doDispense = true;
    //            }
    //            else
    //            {
    //                Debug.Log("Player already received gift twice! Do not dispense gift");
    //                GameManager.Instance.doDispense = false;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Player already received gift twice! Do not dispense gift");
    //            GameManager.Instance.doDispense = false;
    //        }
    //    });

    //    //GameManager.Instance.CheckIfCanDispense();
    //}

    //public static async Task DispenseGift()
    //{
    //    await TrinaxAsyncServerManager.Instance.DispenseGift((bool success, dispenseGiftReceiveJsonData rJson) =>
    //    {
    //        if (success)
    //        {
    //            if (rJson.data)
    //            {
    //                Debug.Log("Dispensed out gift ");
    //            }
    //            else { Debug.LogWarning("ERROR: Did not dispense gift!"); }
    //        }
    //        else
    //        {
    //            Debug.LogWarning("ERROR: Did not dispense gift!");
    //        }
    //    });
    //}

    public static async Task RunPopulateLeaderboard(LeaderboardDisplay lb)
    {
        List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
        await TrinaxAsyncServerManager.Instance.PopulateLeaderboard((bool success, LeaderboardReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (rJson.data != null)
                {
                    for (int i = 0; i < rJson.data.Count; i++)
                    {
                        PlayerInfo info = new PlayerInfo
                        {
                            rank = int.Parse(rJson.data[i].playerid),
                            name = rJson.data[i].name,
                            mobile = rJson.data[i].mobile,
                            score = rJson.data[i].score,
                        };
                        playerInfoList.Add(info);
                    }
                    lb.PopulateData(playerInfoList);
                }
                else
                {
                    Debug.Log("no data");
                }
            }
            else
            {
                Debug.Log("Error in <getLeaderBoard> API!");
                lb.PopulateDefault();
            }
        });
    }

    public static async Task RunAddGameResult()
    {
        AddResultSendJsonData sJson = new AddResultSendJsonData
        {
            name = TrinaxGlobal.Instance.userData.name,
            score = TrinaxGlobal.Instance.userData.score,
            mobile = TrinaxGlobal.Instance.userData.mobile,
        };

        await TrinaxAsyncServerManager.Instance.AddResult(sJson, (bool success, AddResultReceiveJsonData rJson) =>
        {
            if (success)
            {
                if (!string.IsNullOrEmpty(rJson.data))
                {
                    Debug.Log("Send " + sJson.name + " | " + sJson.score);
                }
            }
            else
            {
                Debug.Log("Error in <AddResult> API!");
            }
        });
    }

    public static async Task<string> RunGetResult(int score)
    {
        string result = "";
        await TrinaxAsyncServerManager.Instance.GetResult(score, (bool success, GetResultReceiveJsonData rJson) => {
            if (success)
            {
                if (!string.IsNullOrEmpty(rJson.data))
                {
                    result = rJson.data;
                }
                else
                {
                    Debug.Log("No result from server!");
                }
            }
            else
            {
                Debug.Log("Error in <AddResult> API!");
            }
        });
        return result;
    }
    #endregion
}
