using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    public int dmg;
    public int attackNum;

    //GameObject heroObj;
    SongLoader songLoader;
    Hero_Stats_Combat heroStats;
    BeatOffsetTracker offsetTracker;
    [SerializeField] private Animator animator;

    float animSpeed;

    public enum attackType
    {
        Hit,
        Dodge
    }

    public attackType atkType;

    private void Start()
    {
        //heroObj = GameObject.Find("Hero_Combat");
        //if (heroObj != null)

        heroStats = FindObjectOfType<Hero_Stats_Combat>();
        //Debug.Log(heroStats);

        animator = GetComponent<Animator>();

        offsetTracker = GameObject.Find("Note Grid/State Controller").GetComponent<BeatOffsetTracker>();

        //if ((GameObject.Find("Note Grid/State Controller").GetComponent<SongLoader>().state == SongLoader.CombatState.BeatOffset) || (atkType == attackType.Hit))
        //animator.Play("Flash Blue for offset syncing");

        SetAnimSpeedAndPlay(animSpeed);
    }

    public void SetAnimSpeed(float atkSpeed, float dodgeSpeed)
    {
        if (atkType == attackType.Dodge)
            animSpeed = dodgeSpeed;
        else
            animSpeed = atkSpeed;
    }

    public void SetAnimSpeedAndPlay(float animSpeed)
    {
        animator.speed = animSpeed;

        if (atkType == attackType.Dodge)
            animator.Play("Enemy_Projectile_dodge", 0, 0);
        else
            animator.Play("Flash Blue for offset syncing", 0, 0);
    }

    public void TryDestroy(int bladeShotAttackNum)
    {
        //Debug.Log("Secs ahead of center: " + (animator.GetCurrentAnimatorStateInfo(0).normalizedTime - (51f/60f)));
        if (attackNum == bladeShotAttackNum)
        {
            float playSpeed = animator.GetCurrentAnimatorStateInfo(0).speed;
            float currTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            float secsAhead = ((75*currTime - 51)/(playSpeed*60));
            
            float maxOffset = offsetTracker.GetMaxOffset();

            //Debug.Log("Min, ideal, max times: " + minTime + ", " + (51f / 60f) + ", " + maxTime);

            if (secsAhead < ((-maxOffset)/2))
            {
                //Debug.Log("Too Early! "+ secsAhead);
                return;
            }

            if (secsAhead > ((maxOffset) / 2))
            {
                //Debug.Log("Too Late! " + secsAhead);
                return;
            }

            //Debug.Log("Recent time hit: " + secsAhead);

            offsetTracker.AddOffsetNote(secsAhead);

            heroStats.DestroyEnemyAttack();
            OnDestroy();
        }
    }

    public float GetAnimPercent()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public void PrintTime()
    {
        float frameNum = (animator.GetCurrentAnimatorStateInfo(0).normalizedTime * 75);
        float framesAhead = frameNum - 51;
        float secsAhead = framesAhead / (60f);
        Debug.Log("FramesAhead: "+framesAhead);
        Debug.Log("SecsAhead: "+secsAhead);
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void EndOfAnim()
    {
        //Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime * 75);
        if (atkType == attackType.Hit) //Attack has reached end, meaning it was not hit by the player
        {
            heroStats.takeDamage(dmg);
        }
        OnDestroy();
    }
}
