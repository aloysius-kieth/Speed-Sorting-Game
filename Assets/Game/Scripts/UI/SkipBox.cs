using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipBox : MonoBehaviour
{
    public Animator anim;

    public void Activate()
    {
        anim.SetTrigger("SkipEntry");
    }

    public void Deactivate()
    {
        anim.SetTrigger("SkipExit");
    }
}
