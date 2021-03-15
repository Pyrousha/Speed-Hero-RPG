using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Stats_Combat : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public int dmg;

    public Enemy_Shot_Creator EnemyShotCreator;

    private int counter = 1;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 1; i<=9; i++)
        Invoke("TestMethod", i*0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TestMethod()
    {
        if ((counter >= 1) && (counter <= 9))
        {
            EnemyStartAttack(counter);
            counter++;
        }
    }

    public void EnemyStartAttack(int attackdir)
    {
        EnemyShotCreator.SpawnAttack(attackdir, dmg);
    }
}
