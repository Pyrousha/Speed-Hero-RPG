using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void Rise()
    {
        anim.SetTrigger("Rise");
    }

    public void Fall()
    {
        anim.SetTrigger("Fall");
    }

    public void RiseAndReset()
    {
        anim.ResetTrigger("Fall");

        if(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "PlatformRise")
            anim.SetTrigger("Rise");
    }
}
