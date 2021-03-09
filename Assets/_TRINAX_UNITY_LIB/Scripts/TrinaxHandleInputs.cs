using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading.Tasks;

// Inputs relating to debugging, put them here!
public class TrinaxHandleInputs : MonoBehaviour
{
    TrinaxAdminPanel aP;
    bool isReady = false;
    bool hideText = false;

    async void Start()
    {
        isReady = false;
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);

        isReady = true;

        aP = TrinaxCanvas.Instance.adminPanel;
    }

    private void Update()
    {
        if (!isReady) return;

        //foreach (char c in Input.inputString)
        //{
        //    Debug.Log(c);
        //}

        //if (aP != null && aP.gameObject.activeSelf)
        //{
        //if (Input.GetKeyDown(KeyCode.F2) && TrinaxGlobal.Instance.state == PAGES.GAME)
        //{
        //    MainManager.Instance.StopGame();
        //    UIManager.Instance.ToScreensaver();
        //}
        //}

        if (Input.GetKeyDown(KeyCode.F10))
        {
            //hideText = !hideText;
            //TrinaxCanvas.Instance.HideDebugText(hideText);
            Cursor.visible = !Cursor.visible;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            TrinaxGlobal.Instance.IsAppPaused = !TrinaxGlobal.Instance.IsAppPaused;
        }
        if (Input.GetKeyDown(KeyCode.F12) && TrinaxGlobal.Instance.state == STATE.SCREENSAVER)
        {
            //MainManager.Instance.ClearLocalsaveFile();
            //UIManager.Instance.ToScreensaver();
        }

        if (TrinaxGlobal.Instance.IsAppPaused)
        {
            Time.timeScale = 0;
        }
        else Time.timeScale = 1;
    }

}
