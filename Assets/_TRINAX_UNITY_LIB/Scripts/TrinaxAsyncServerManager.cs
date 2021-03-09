using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

using System.Threading.Tasks;

/// <summary>
/// Async version of server manager
/// </summary>
public class TrinaxAsyncServerManager : MonoBehaviour, IManager
{
    #region SINGLETON
    public static TrinaxAsyncServerManager Instance { get; set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    #endregion

    int executionPriority = 200;
    public int ExecutionPriority { get { return executionPriority; } set { value = executionPriority; } }

    public bool IsReady { get; set; }

    async void Start()
    {

    }

    public async Task Init()
    {
        //await new WaitUntil(() => TrinaxSaveManager.Instance.IsReady);
        Debug.Log("Loading TrinaxAsyncManager...");
        IsReady = false;

        //LOG_DIR = Application.dataPath + LOG_DIR;

        //if (!Directory.Exists(LOG_DIR))
        //    Directory.CreateDirectory(LOG_DIR);

        if (loadingCircle != null)
            loadingCircle.SetActive(false);

        IsReady = true;
        Debug.Log("TrinaxAsyncManager is loaded!");
    }

    public void PopulateValues(GlobalSettings setting)
    {
        ip = setting.IP;
        useServer = setting.useServer;
        useMocky = setting.useMocky;
    }

    public static Action OnConnectionLost;
    public static Action OnMaxRetriesReached;

    public string ip = "127.0.0.1";
    public bool useServer;
    public bool useMocky;
    public GameObject loadingCircle;

    public string userID;

    const string interactionServletUrl = "TCMSpring/Servlet/interactionApiServlet/";
    const string playerServletUrl = "TCMSpring/Servlet/playerServlet/";
    const string port = ":8080/";

    public const string ERROR_CODE_200 = "200";
    public const string ERROR_CODE_201 = "201";

    bool isLoading = false;
    bool isVerifying = false;
    bool isDelayedScanCoroutineRunning = false;

    string LOG_DIR = "/log/";
    string LOG_FILE = "Async_server_logs.log";

    #region API Funcs
    public async Task StartInteraction(StartInteractionSendJsonData json, Action<bool, StartInteractionReceiveJsonData> callback)
    {
        StartInteractionReceiveJsonData data;
        if (!useServer)
        {
            //data = new StartInteractionReceiveJsonData();
            //callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;

        if (!useMocky)
        {
            //Debug.Log("Using actual server url...");
            url = "http://" + ip + port + interactionServletUrl + "startInteraction";
        }
        else
        {
            //Debug.Log("Using mocky url...");
            return;
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.text.Trim();
                data = JsonUtility.FromJson<StartInteractionReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                //WriteError(request, "StartInteraction");
                data = new StartInteractionReceiveJsonData();
                callback(false, data);
            }
        });

        //Debug.Log(r);
    }

    public async Task AddInteractionDetails(InteractionDetailsSendJsonData json, Action<bool, InteractionDetailsReceiveJsonData> callback)
    {
        InteractionDetailsReceiveJsonData data;
        if (!useServer)
        {
            //data = new InteractionDetailsReceiveJsonData();
            //callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;

        if (!useMocky)
        {
            //Debug.Log("Using actual server url...");
            url = "http://" + ip + port + interactionServletUrl + "addInteraction";
        }
        else
        {
            //Debug.Log("Using mocky url...");
            return;
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.text.Trim();
                data = JsonUtility.FromJson<InteractionDetailsReceiveJsonData>(result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                //WriteError(request, "InteractionDetails");
                data = new InteractionDetailsReceiveJsonData();
                callback(false, data);
            }
        });

        //Debug.Log(r);
    }

    public async Task EndInteraction(InteractionEndSendJsonData json, Action<bool, InteractionEndReceiveJsonData> callback)
    {
        InteractionEndReceiveJsonData data;
        if (!useServer)
        {
            //data = new InteractionEndReceiveJsonData();
            //callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;

        if (!useMocky)
        {
            //Debug.Log("Using actual server url...");
            url = "http://" + ip + port + interactionServletUrl + "endInteraction/" + TrinaxGlobal.Instance.userData.interactionID;
        }
        else
        {
            //Debug.Log("Using mocky url...");
            return;
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.text.Trim();
                data = JsonUtility.FromJson<InteractionEndReceiveJsonData>(result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                //WriteError(request, "EndInteraction");
                data = new InteractionEndReceiveJsonData();
                callback(false, data);
            }
        });

        //Debug.Log(r);
    }

    //public async Task GenerateQR(Action<bool, GenerateQRReceiveJsonData> callback)
    //{
    //    GenerateQRReceiveJsonData data;
    //    if (!useServer)
    //    {
    //        //data = new InteractionEndReceiveJsonData();
    //        //callback(true, data);
    //        return;
    //    }

    //    string url;

    //    if (!useMocky)
    //    {
    //        //Debug.Log("Using actual server url...");
    //        url = "http://" + ip + port + servletUrl + "generateQRcode/" + TrinaxGlobal.Instance.userData.interactionID;
    //    }
    //    else
    //    {
    //        //Debug.Log("Using mocky url...");
    //        return;
    //    }

    //    var r = await LoadUrlAsync(url, (request) =>
    //    {
    //        if (string.IsNullOrEmpty(request.error))
    //        {
    //            string result = request.text.Trim();
    //            data = JsonUtility.FromJson<GenerateQRReceiveJsonData>(result);

    //            if (data.error.error_code == ERROR_CODE_200)
    //            {
    //                callback(true, data);
    //            }
    //            else
    //            {
    //                callback(false, data);
    //            }
    //        }
    //        else
    //        {
    //            //WriteError(request, "GenerateQR");
    //            data = new GenerateQRReceiveJsonData();
    //            callback(false, data);
    //        }
    //    });

    //    //Debug.Log(r);
    //}

    //public async Task VerifyQrCodeAsync(VerifyQRSendJsonData json, Action<bool, VerifyQRReceiveJsonData> callback)
    //{
    //    VerifyQRReceiveJsonData data;
    //    if (!useServer)
    //    {
    //        data = new VerifyQRReceiveJsonData();
    //        callback(true, data);
    //        return;
    //    }

    //    // TODO: Check if main manager is at a certain point in the game for this API to be called

    //    if(isVerifying)
    //    {
    //        if (isDelayedScanCoroutineRunning)
    //        {
    //            Debug.Log("Already scanning!");
    //        }
    //        else
    //        {
    //            Debug.Log("Running delay qr scanner task");
    //            DelayQrScanner();
    //        }
    //        return;
    //    }
    //    isVerifying = true;

    //    string sendJson = JsonUtility.ToJson(json);
    //    string url;

    //    if (!useMocky)
    //    {
    //        Debug.Log("Using actual server url...");
    //        url = "http://www.mocky.io/v2/5b7e53fe3000007a0084c0d4";
    //        //url = "http://" + ip + port + frontUrl + "formType=addInteraction&qrID=" + qrCode;
    //    }
    //    else
    //    {
    //        Debug.Log("Using mocky url...");
    //        // returns ID
    //        url = "http://www.mocky.io/v2/5b7e53fe3000007a0084c0d4";
    //    }

    //    var r = await LoadPostUrl(url, sendJson, (request) =>
    //    {
    //        if (string.IsNullOrEmpty(request.error))
    //        {
    //            string result = request.text.Trim();
    //            data = JsonUtility.FromJson<VerifyQRReceiveJsonData>(result);
    //            //Debug.Log("myresult: " + result);

    //            if (data.error.error_code == ERROR_CODE_200)
    //            {
    //                // Get userID from server

    //                callback(true, data);
    //            }
    //            else
    //            {
    //                callback(false, data);
    //            }
    //        }
    //        else
    //        {
    //            WriteError(request, "VerifyQR");
    //            data = new VerifyQRReceiveJsonData();
    //            callback(false, data);
    //        }
    //    });

    //    DelayQrScanner();
    //    //Debug.Log(r);
    //}

    public async Task AddResult(AddResultSendJsonData json, Action<bool, AddResultReceiveJsonData> callback)
    {
        AddResultReceiveJsonData data;
        if (!useServer)
        {
            data = new AddResultReceiveJsonData();
            callback(true, data);
            return;
        }

        string sendJson = JsonUtility.ToJson(json);
        string url;
        if (!useMocky)
        {
            url = "http://" + ip + port + playerServletUrl + "add";
        }
        else
        {
            url = "http://www.mocky.io/v2/5ba30c362f00005c008d2eed";
        }

        var r = await LoadPostUrl(url, sendJson, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.text.Trim();
                data = JsonUtility.FromJson<AddResultReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "AddResult");
                data = new AddResultReceiveJsonData();
                callback(false, data);
            }
        });
    }
 
    public async Task GetResult(int score, Action<bool, GetResultReceiveJsonData> callback)
    {
        GetResultReceiveJsonData data;
        if (!useServer)
        {
            //data = new GetResultReceiveJsonData();
            //callback(true, data);
            return;
        }

        string url;
        if (!useMocky)
        {
            url = "http://" + ip + port + playerServletUrl + "getResult/" + score.ToString();
        }
        else
        {
            Debug.Log("No url to load!");
            return;
        }

        var r = await LoadUrlAsync(url, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.text.Trim();
                data = JsonUtility.FromJson<GetResultReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "GetResult");
                data = new GetResultReceiveJsonData();
                callback(false, data);
            }
        });
    }

    public async Task PopulateLeaderboard(Action<bool, LeaderboardReceiveJsonData> callback)
    {
        LeaderboardReceiveJsonData data;
        if (!useServer)
        {
            //data = new LeaderboardReceiveJsonData();
            //callback(true, data);
            return;
        }

        string url;
        if (!useMocky)
        {
            Debug.Log("Using actual server url...");
            url = "http://" + ip + port + playerServletUrl + "getLeaderBoard";
        }
        else
        {
            Debug.Log("Using mocky url...");
            url = "http://www.mocky.io/v2/5ba30c4e2f000077008d2eee";
        }

        var r = await LoadUrlAsync(url, (request) =>
        {
            if (string.IsNullOrEmpty(request.error))
            {
                string result = request.text.Trim();
                data = JsonUtility.FromJson<LeaderboardReceiveJsonData>(result);
                //Debug.Log("myresult: " + result);

                if (data.error.error_code == ERROR_CODE_200)
                {
                    callback(true, data);
                }
                else
                {
                    callback(false, data);
                }
            }
            else
            {
                WriteError(request, "PopulateLeaderboard");
                data = new LeaderboardReceiveJsonData();
                callback(false, data);
            }
        });
    }

    #endregion

    async Task<string> LoadUrlAsync(string url, Action<WWW> callback)
    {
        isLoading = true;

        DelayLoadingCircle();

        Debug.Log("Loading url: " + url);
        WWW request = await new WWW(url);
        Debug.Log(url + "\n" + request.text);

        //if (TrinaxGlobal.Instance.state == PAGES.SCREENSAVER)
        //    MainManager.Instance.appStartBtn.interactable = false;

        await ApiCallback(request, url, (WWW r) => { request = r; });
        //await new WaitForSeconds(3.0f); // artifical wait

        callback(request);

        isLoading = false;
        if (loadingCircle != null)
        {
            loadingCircle.SetActive(false);
        }

        //if (TrinaxGlobal.Instance.state == PAGES.SCREENSAVER)
        //    MainManager.Instance.appStartBtn.interactable = true;
        //Debug.Log(request.text);
        return request.text;
    }

    //WWW requestPost, resultGet;
    async Task<string> LoadPostUrl(string url, string jsonString, Action<WWW> callback)
    {
        isLoading = true;
        //then set the headers Dictionary headers=form.headers; headers["Content-Type"]="application/json";

        DelayLoadingCircle();

        WWWForm form = new WWWForm();
        byte[] jsonSenddata = null;
        if (!string.IsNullOrEmpty(jsonString))
        {
            Debug.Log(jsonString);
            jsonSenddata = System.Text.Encoding.UTF8.GetBytes(jsonString);
        }

        form.headers["Content-Type"] = "application/json";
        form.headers["Accept"] = "application/json";
        Dictionary<string, string> headers = form.headers;
        headers["Content-Type"] = "application/json";

        Debug.Log("Loading url: " + url);
        WWW request = await new WWW(url, jsonSenddata, headers);
        Debug.Log(url + "\n" + request.text);

        await ApiCallback(request, url, jsonSenddata, headers, (WWW r) => { request = r; });

        //await new WaitForSeconds(3f); // artifical wait for 150ms

        callback(request);

        isLoading = false;
        if (loadingCircle != null)
        {
            loadingCircle.SetActive(false);
        }
        //Debug.Log(request.text);
        return request.text;
    }

    bool apiDone = false;
    async Task ApiCallback(WWW _request, string _url, byte[] _data, Dictionary<string, string> _headers, Action<WWW> _result)
    {
        await LoopResultPost(_request, _url, _data, _headers, _result);
    }

    async Task ApiCallback(WWW _request, string _url, Action<WWW> _result)
    {
        await LoopResultGet(_request, _url, _result);
    }

    int numOfRetries = 10;
    public GameObject lostConnectionPanel;
    IEnumerator LoopResultPost(WWW _request, string _url, byte[] _data, Dictionary<string, string> _headers, Action<WWW> _result)
    {
        int tries = 0;
        while (true)
        {
            if (string.IsNullOrEmpty(_request.text))
            {
                OnConnectionLost?.Invoke();
                WWW r = new WWW(_url, _data, _headers);
                yield return r;
                if (string.IsNullOrEmpty(r.text))
                {
                    if (!lostConnectionPanel.activeSelf)
                    {
                        lostConnectionPanel.SetActive(true);
                    }
                    tries++;
                    if (tries >= numOfRetries)
                    {
                        Debug.Log("Tried " + numOfRetries + " !" + " Going back to start...");
                        OnMaxRetriesReached?.Invoke();
                        yield return new WaitForSeconds(5f);
                        // GameManager.Instance.ToScreensaver();
                        lostConnectionPanel.SetActive(false);
                        yield break;
                    }
                    Debug.Log("Request from server is empty! Getting a new request...");

                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    //requestPost = r;
                    _result(r);
                    Debug.Log("Result: " + r.text);
                    lostConnectionPanel.SetActive(false);
                    yield break;
                }
            }
            else yield break;
        }
    }

    IEnumerator LoopResultGet(WWW _request, string _url, Action<WWW> _result)
    {
        int tries = 0;
        while (true)
        {
            if (string.IsNullOrEmpty(_request.text))
            {
                OnConnectionLost?.Invoke();
                WWW r = new WWW(_url);
                yield return r;
                if (string.IsNullOrEmpty(r.text))
                {
                    if (!lostConnectionPanel.activeSelf)
                    {
                        lostConnectionPanel.SetActive(true);
                    }
                    tries++;
                    Debug.Log(tries);
                    if (tries >= numOfRetries)
                    {
                        Debug.Log("Tried " + numOfRetries + " !" + " Going back to start...");
                        OnMaxRetriesReached?.Invoke();
                        yield return new WaitForSeconds(5f);
                        //GameManager.Instance.ToScreensaver();
                        lostConnectionPanel.SetActive(false);
                        yield break;
                    }
                    Debug.Log("Request from server is empty! Getting a new request...");

                    yield return new WaitForSeconds(3f);
                }
                else
                {
                    _result(r);
                    //resultGet = r;
                    Debug.Log("Result: " + r.text);
                    lostConnectionPanel.SetActive(false);
                    yield break;
                }
            }
            else yield break;
        }
    }

    async void DelayLoadingCircle()
    {
        await new WaitForSeconds(1.5f);

        if (isLoading && loadingCircle != null)
            loadingCircle.SetActive(true);
    }

    async void DelayQrScanner()
    {
        isDelayedScanCoroutineRunning = true;
        await new WaitForSeconds(3f);

        isVerifying = false;
        isDelayedScanCoroutineRunning = false;
    }

    void WriteError(WWW request, string api)
    {
        string error = "<" + api + "> --- Begin Error Message: " + request.error + " >> Url: " + request.url + System.Environment.NewLine;
        File.AppendAllText(LOG_DIR + LOG_FILE, error);
    }

    void WriteError(string errorStr, string api)
    {
        string error = "<" + api + "> --- Begin Error Message: " + errorStr + System.Environment.NewLine;
        File.AppendAllText(LOG_DIR + LOG_FILE, error);
    }
}
