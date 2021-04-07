using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSpawnAttack : MonoBehaviour
{
    public HeroShotCreator shotCreator;
    public Hero_Controller_Combat hero_Controller_Combat;

    // Start is called before the first frame update
    void Start()
    {
        hero_Controller_Combat = GetComponent<Hero_Controller_Combat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void atk(int attackNum)
    {
        //Call funstion to spawn attack projectile
        switch(attackNum)
        {
            case (2):
                {
                    shotCreator.attack_2();
                    break;
                }
            case (3):
                {
                    shotCreator.attack_3();
                    break;
                }
            case (5):
                {
                    shotCreator.attack_5();
                    break;
                }
            case (7):
                {
                    shotCreator.attack_7();
                    break;
                }
            case (9):
                {
                    shotCreator.attack_9();
                    break;
                }
            default:
                {
                    //Debug.Log("invalid integer passed into atk() function. Value: " + attackNum);
                    break;
                }
        }
        //Debug.Log("attack spawned");
    }
}
