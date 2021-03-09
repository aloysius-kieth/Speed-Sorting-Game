using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBoundary : MonoBehaviour
{
    public float VerticalExtent { get; set; }
    public float HorizontalExtent { get; set; }

    public Collider2D[] spawnAreaBoundary;

    public void Init()
    {
        VerticalExtent = Camera.main.orthographicSize;
        HorizontalExtent = VerticalExtent * Screen.width / Screen.height;

        //Debug.Log(string.Format("Vertical: {0} | Horizontal: {1}", VerticalExtent, HorizontalExtent));
        //Debug.Log("Area to spawn: " + AreaToSpawn);
    }

    public void SetPosition(bool isRight)
    {
        if (isRight)
        {
            spawnAreaBoundary[0].offset = new Vector3(-2.2f, 0);
            spawnAreaBoundary[1].offset = new Vector3(7.08f, 2.05f);
        }
        else
        {
            spawnAreaBoundary[0].offset = new Vector3(2.2f, 0);
            spawnAreaBoundary[1].offset = new Vector3(-7.08f, 2.05f);
        }
    }
}
