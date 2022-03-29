using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Attack_Projectile : MonoBehaviour
{
    [SerializeField] private GameObject hitEnemyEffect;
    private int dmgToDeal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDamage(int newDmg)
    {
        dmgToDeal = newDmg;
    }

    public void OnHitEnemy()
    {
        GameObject hitEffectObj = Instantiate(hitEnemyEffect, null);
        FindObjectOfType<Enemy_Stats_Combat>().TakeDamage(dmgToDeal);
        Destroy(gameObject);
    }
}
