using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    public int dmg;

    public enum attackType
    {
        Hit,
        Dodge
    }

    public attackType atkType;

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    private void EndOfAnim()
    {
        if (atkType == attackType.Hit) //Attack has reached end, meaning it was not hit by the player
        {
            //Player should take damage
            GameObject heroObj = GameObject.Find("Hero_Combat");

            if (heroObj != null)
                heroObj.GetComponent<Hero_Stats_Combat>().takeDamage(dmg);
        }
        OnDestroy();
    }
}
