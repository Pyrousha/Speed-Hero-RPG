using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCanvas : Singleton<StairCanvas>
{
    [SerializeField] private Transform heroTransform;
    [SerializeField] private Animator anim;
    private Vector3 targPosition;

    public void TriggerHit(StairTrigger.Dir direction, Vector3 newTargPos)
    {
        targPosition = newTargPos;

        anim.ResetTrigger("left");
        anim.ResetTrigger("right");

        if(direction == StairTrigger.Dir.left)
        {
            anim.SetTrigger("left");
        }
        else
        {
            anim.SetTrigger("right");
        }
    }

    public void Warp()
    {
        heroTransform.localPosition = targPosition;
    }
}
