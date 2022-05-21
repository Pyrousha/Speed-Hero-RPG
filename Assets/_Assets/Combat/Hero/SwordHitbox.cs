using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [SerializeField] private PlayerSwordHandler swordHandler;

    public void OnTriggerEnter(Collider other)
    {
        Enemy_Stats enemy = other.GetComponent<Enemy_Stats>();
        if (enemy != null)
        {
            swordHandler.EnemyHit(enemy);
            return;
        }

        EnemyHurtbox enemyHurtbox = other.GetComponent<EnemyHurtbox>();
        if(enemyHurtbox != null)
            swordHandler.EnemyHit(enemyHurtbox.EnemyStats);
    }
}
