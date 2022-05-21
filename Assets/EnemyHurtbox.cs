using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHurtbox : MonoBehaviour
{
    [SerializeField] private Enemy_Stats enemyStats;
    public Enemy_Stats EnemyStats => enemyStats;
}
