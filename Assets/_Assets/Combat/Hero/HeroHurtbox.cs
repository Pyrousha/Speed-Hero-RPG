using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroHurtbox : MonoBehaviour
{
    [SerializeField] private Hero_Stats heroStats;


    private void OnTriggerEnter(Collider other)
    {
        if (HeroParryManager.Instance.ParryState != HeroParryManager.ParryStateEnum.parrying)
        {
            heroStats.TakeDamage(0.5f);
        }
        else
        {
            heroStats.BecomeInvincible();

            //Do Parry
            HeroParryManager.Instance.DoParry(other);
        }

        other.gameObject.SetActive(false);
    }
}
