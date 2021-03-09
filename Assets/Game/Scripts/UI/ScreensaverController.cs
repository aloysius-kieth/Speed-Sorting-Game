using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SCREENSAVER_STATE
{
    MAIN_MENU,
    LEADERBOARD,
}

public class ScreensaverController : MonoBehaviour
{
    public SCREENSAVER_STATE state;

    public List<string> screensaverAnimatorParameters = new List<string>();

    [Header("Animators")]
    public Animator screensaverAnim;
    public Animator leaderboardAnim;

    float timer = 0;
    public int timeInterval = 15;

    void Start()
    {
        Init();
    }

    void Init()
    {
        timer = 0;
        for (int i = 0; i < screensaverAnim.parameterCount; i++)
        {
            screensaverAnimatorParameters.Add(screensaverAnim.parameters[i].name);
        }
    }

    void Update()
    {
        if (TrinaxGlobal.Instance.state != STATE.SCREENSAVER) return;

        // Increment interval if animation is not playing
        if (screensaverAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !screensaverAnim.IsInTransition(0))
        {
            timer += Time.deltaTime;
        }

        if (timer > timeInterval)
        {
            timer = 0;
            OnScreenExit();
        }

        if (Input.GetMouseButtonDown(0) && !TrinaxCanvas.Instance.adminPanel.gameObject.activeSelf)
        {
            timer = 0;
            if(state != SCREENSAVER_STATE.MAIN_MENU)
            {
                state = SCREENSAVER_STATE.MAIN_MENU;
                RebindAnim(screensaverAnim);
            }
        }
    }

    void OnScreenEntry()
    {
        Animator anim = screensaverAnim;

        switch (state)
        {
            case SCREENSAVER_STATE.MAIN_MENU:
                //AppManager.Instance.uiManager.backgroundCanvas.Activate(0, true);
                anim.SetTrigger("ToMainMenu");
                break;
            case SCREENSAVER_STATE.LEADERBOARD:
                anim.SetTrigger("ToLeaderboard");
                break;
            default:
                throw new System.Exception("No such state!");
        }
    } 

    void OnScreenExit()
    {
        Animator anim = screensaverAnim;

        switch (state)
        {
            case SCREENSAVER_STATE.MAIN_MENU:
                AppManager.Instance.uiManager.backgroundCanvas.Activate(0, true);
                anim.SetTrigger("OnMainMenuExit");
                break;
            case SCREENSAVER_STATE.LEADERBOARD:
                anim.SetTrigger("OnLeaderboardExit");
                break;
            default:
                throw new System.Exception("No such state!");
        }
    }

    void SetState(SCREENSAVER_STATE _state)
    {
        state = _state;
    }

    void RebindAnim(Animator anim)
    {
        anim.Rebind();
    }

    void OnDisable()
    {
        timer = 0;
    }
}
