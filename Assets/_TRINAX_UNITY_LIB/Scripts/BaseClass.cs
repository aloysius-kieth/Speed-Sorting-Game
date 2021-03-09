using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

[RequireComponent(typeof(CanvasController))]
public abstract class BaseClass : MonoBehaviour
{
    // Component References
    protected CanvasController canvasController;

    public virtual async Task Start()
    {
        await new WaitUntil(() => TrinaxGlobal.Instance.isReady);

        // Cache any component references
        canvasController = GetComponent<CanvasController>();

        //Init();
    }

    public virtual void Init()
    {

    }
}
