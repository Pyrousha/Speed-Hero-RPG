using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    public int dmg;

    GameObject heroObj;
    Hero_Stats_Combat heroStats; 

    private void Start()
    {
        heroObj = GameObject.Find("Hero_Combat");
        if (heroObj != null)
            heroStats = heroObj.GetComponent<Hero_Stats_Combat>();
    }

    public enum attackType
    {
        Hit,
        Dodge
    }

    public attackType atkType;

    public void TryDestroy()
    {
        if (atkType == attackType.Hit)
        {
            OnDestroy();
            heroStats.DestroyEnemyAttack();
        }
    }

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void EndOfAnim()
    {
        if (atkType == attackType.Hit) //Attack has reached end, meaning it was not hit by the player
        {
            //Player should take damage
            if (heroObj != null)
                heroStats.takeDamage(dmg);
        }
        OnDestroy();
    }
}
