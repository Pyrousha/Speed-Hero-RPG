using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCanvas : Singleton<StairCanvas>
{
    [SerializeField] private Animator heroSpriteAnim;
    [SerializeField] private Transform heroTransform;

    [SerializeField] private Animator anim;
    [SerializeField] private Animator walkAnim;
    private Vector3 targPosition;

    public void TriggerHit(StairParent.Dir direction, Vector3 newTargPos)
    {
        targPosition = newTargPos;

        anim.ResetTrigger("left");
        anim.ResetTrigger("right");

        if(direction == StairParent.Dir.left)
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

    public void PlayWalkAnim(StairParent.Dir direction)
    {
        if (direction == StairParent.Dir.left)
        {
            walkAnim.Play("stair_walk_left", 0, heroSpriteAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        else
        {
            walkAnim.Play("stair_walk_right", 0, heroSpriteAnim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
    }
}
