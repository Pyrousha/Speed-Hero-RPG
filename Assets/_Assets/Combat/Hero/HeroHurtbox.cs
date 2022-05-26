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
            other.transform.parent.parent.parent.parent.GetComponent<Enemy_AI>().StunnedByHero();
            HeroParryManager.Instance.ParryParticleSystem.Play();
        }

        other.gameObject.SetActive(false);
    }
}
