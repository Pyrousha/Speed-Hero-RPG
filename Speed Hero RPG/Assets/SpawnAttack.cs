using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAttack : MonoBehaviour
{
    public ShotCreator shotCreator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void atk_2()
    {
        shotCreator.attack_2();
    }
    void atk_3()
    {
        shotCreator.attack_3();
    }
    void atk_5()
    {
        shotCreator.attack_5();
    }
    void atk_7()
    {
        shotCreator.attack_7();
    }
    void atk_9()
    {
        shotCreator.attack_9();
    }
}
