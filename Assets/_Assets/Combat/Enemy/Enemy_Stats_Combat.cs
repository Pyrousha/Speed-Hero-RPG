using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats_Combat : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public int dmg;

    public Enemy_Shot_Creator EnemyShotCreator;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void EnemyStartAttack(int attackdir)
    {
        EnemyShotCreator.SpawnAttack(attackdir, dmg);
    }

    public void LoadFromEnemyObject(EnemyObject enemyObject)
    {
        if (enemyObject == null)
            return;

        maxHp = enemyObject.maxHP;
        hp = maxHp;

        spriteRenderer.sprite = enemyObject.enemySprite;
    }
}
