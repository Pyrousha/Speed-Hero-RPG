using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroHurtbox : MonoBehaviour
{
    [SerializeField] private Hero_Stats heroStats;


    private void OnTriggerEnter(Collider other)
    {
        heroStats.TakeDamage(0.5f);

        //other.enabled = false;
    }
}
