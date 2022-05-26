using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroParryManager : Singleton<HeroParryManager>
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem parryParticleSystem;
    public ParticleSystem ParryParticleSystem => parryParticleSystem;

    public enum ParryStateEnum
    {
        idle,
        parrying,
        endlag
    }

    private ParryStateEnum parryState;
    public ParryStateEnum ParryState => parryState;

    [Header("Values")]
    [SerializeField] private float parryDuration;
    [SerializeField] private float parryEndlagDuration;

    private float parryEndTime;
    private float parryRefreshedTime;

    // Update is called once per frame
    void Update()
    {
        switch(parryState)
        {
            case ParryStateEnum.idle:
                {
                    if (InputHandler.Instance.Parry.down)
                    {
                        parryState = ParryStateEnum.parrying;
                        parryEndTime = Time.time + parryDuration;
                        anim.SetTrigger("StartParry");
                    }
                    break;
                }
            case ParryStateEnum.parrying:
                {
                    if(Time.time >= parryEndTime)
                    {
                        parryState = ParryStateEnum.endlag;
                        parryRefreshedTime = Time.time + parryEndlagDuration;
                        anim.SetTrigger("EndParry");
                    }

                    break;
                }
            case ParryStateEnum.endlag:
                {
                    if(Time.time >= parryRefreshedTime)
                    {
                        parryState = ParryStateEnum.idle;
                    }

                    break;
                }
        }
    }
}
