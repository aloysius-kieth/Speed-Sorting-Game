using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    int leftPostionX;
    [SerializeField]
    int rightPositionX;

    public VerticalLayoutGroup layout;

    public void SetPosition(bool isRight)
    {
        if (isRight)
            layout.padding.left = rightPositionX;
        else layout.padding.left = leftPostionX;
    }
}
