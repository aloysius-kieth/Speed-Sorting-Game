using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class BackgroundCanvas : MonoBehaviour
{
    public CanvasGroup[] canvases;

    public void Activate(int index, bool immediate = false)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            if (index == i)
            {
                if (!immediate)
                    canvases[i].DOFade(1.0f, 0.25f);
                else canvases[i].alpha = 1;
            }
            else
            { 
                canvases[i].alpha = 0;
            }
        }
    }

    public void Deactivate(bool immediate = false)
    {
        for (int i = 0; i < canvases.Length; i++)
        {
            CanvasGroup canvas = canvases[i];
            if (!immediate)
            {
                canvas.DOFade(0f, 0.25f);
            }
            else
            {
                canvas.alpha = 0;
            }
        }
    }

}
