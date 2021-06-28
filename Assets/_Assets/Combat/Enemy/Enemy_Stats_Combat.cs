﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats_Combat : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public int dmg;

    public Enemy_Shot_Creator EnemyShotCreator;

    /*
    public void Spawn1()
    {
        EnemyStartAttack(1);
    }
    public void Spawn2()
    {
        EnemyStartAttack(2);
    }
    public void Spawn3()
    {
        EnemyStartAttack(3);
    }
    public void Spawn4()
    {
        EnemyStartAttack(4);
    }
    public void Spawn5()
    {
        EnemyStartAttack(5);
    }
    public void Spawn6()
    {
        EnemyStartAttack(6);
    }
    public void Spawn7()
    {
        EnemyStartAttack(7);
    }
    public void Spawn8()
    {
        EnemyStartAttack(8);
    }
    public void Spawn9()
    {
        EnemyStartAttack(9);
    }
    */

    public void EnemyStartAttack(int attackdir)
    {
        EnemyShotCreator.SpawnAttack(attackdir, dmg);
    }
}
