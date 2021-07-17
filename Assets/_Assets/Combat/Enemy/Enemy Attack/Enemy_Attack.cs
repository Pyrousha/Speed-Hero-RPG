using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    public int dmg;

    GameObject heroObj;
    SongLoader songLoader;
    Hero_Stats_Combat heroStats; 

    private void Start()
    {
        heroObj = GameObject.Find("Hero_Combat");
        if (heroObj != null)
            heroStats = heroObj.GetComponent<Hero_Stats_Combat>();

        if ((GameObject.Find("Note Grid/State Controller").GetComponent<SongLoader>().state == SongLoader.gameState.BeatOffset) || (atkType == attackType.Hit))
            GetComponent<Animator>().Play("Flash Blue for offset syncing");
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
        Debug.Log("end of anim");
        if (atkType == attackType.Hit) //Attack has reached end, meaning it was not hit by the player
        {
            Debug.Log("deal dmg??");
            //Player should take damage
            if (heroObj != null)
                heroStats.takeDamage(dmg);
        }
        OnDestroy();
    }
}
