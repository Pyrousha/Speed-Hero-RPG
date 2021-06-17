using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shot_Creator : MonoBehaviour
{
    public GameObject shotParent;
    public GameObject shotProjectileHit;
    public GameObject shotProjectileDodge;
    GameObject parentObj;

    public Vector3 startingPos;

    public float offset;


    public int[] attackNums;
    public int[] attackTimes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Spawns an attack with the specified direction and damage
    /// </summary>
    /// <param name="attackDir">Direction of the attack to spaws (1-9)</param>
    /// <param name="damage">Damagae that the attack will deal</param>
    public void SpawnAttack(int attackDir, int damage)
    {
        //Create parentObj
        parentObj = Instantiate(shotParent) as GameObject;
        startingPos = parentObj.transform.position;

        switch (attackDir)
        {
            case (1):
                {
                    //Left Attack
                    MoveOffset(-0.5f, -1); //Set location
                    SetRotationZ(90); //Set rotation
                    CreateProjectile(damage, shotProjectileDodge); //Create attack

                    //Spawn Right Attack
                    SpawnAttack(11, damage);
                    break;
                }
            case (2):
                {
                    MoveOffset(-1, 0); //Set location
                    SetRotationZ(90); //Set rotation
                    CreateProjectile(damage, shotProjectileHit); //Create attack
                    break;
                }
            case (3):
                {
                    MoveOffset(0, 0); //Set location
                    SetRotationZ(45); //Set rotation
                    CreateProjectile(damage, shotProjectileHit); //Create attack
                    break;
                }
            case (4):
                {
                    MoveOffset(-1, 0); //Set location
                    SetRotationZ(0); //Set rotation
                    CreateProjectile(damage, shotProjectileDodge); //Create attack
                    break;
                }
            case (5):
                {
                    MoveOffset(0, 0); //Set location
                    SetRotationZ(0); //Set rotation
                    CreateProjectile(damage, shotProjectileHit); //Create attack
                    break;
                }
            case (6):
                {
                    MoveOffset(1, 0); //Set location
                    SetRotationZ(0); //Set rotation
                    CreateProjectile(damage, shotProjectileDodge); //Create attack
                    break;
                }
            case (7):
                {
                    MoveOffset(0, 0); //Set location
                    SetRotationZ(-45); //Set rotation
                    CreateProjectile(damage, shotProjectileHit); //Create attack
                    break;
                }
            case (8):
                {
                    //Right attack
                    MoveOffset(0f, 1); //Set location
                    SetRotationZ(-90); //Set rotation
                    CreateProjectile(damage, shotProjectileDodge); //Create attack

                    //Spawn left attack
                    SpawnAttack(10, damage);
                    break;
                }
            case (9):
                {
                    MoveOffset(1, 0); //Set location
                    SetRotationZ(-90); //Set rotation
                    CreateProjectile(damage, shotProjectileHit); //Create attack
                    break;
                }
            case (10):
                {
                    //Left attack
                    MoveOffset(-0.25f, 1); //Set location
                    SetRotationZ(90); //Set rotation
                    CreateProjectile(damage, shotProjectileDodge); //Create attack
                    break;
                }
            case (11):
                {
                    MoveOffset(0f, -1); //Set location
                    SetRotationZ(-90); //Set rotation
                    CreateProjectile(damage, shotProjectileDodge); //Create attack
                    break;
                }
        }
    }

    private void MoveOffset(float dx, float dy)
    {
        parentObj.transform.position = startingPos + new Vector3(dx*offset, dy*offset, 0);
    }

    private void SetRotationZ(float newZ)
    {
        parentObj.transform.rotation = Quaternion.Euler(0, 0, newZ);
    }

    private void CreateProjectile(int damage, GameObject attackType)
    {
        //Create projectile
        GameObject childObject = Instantiate(attackType) as GameObject;

        //Set damage
        attackType.GetComponent<Enemy_Attack>().dmg = damage;
        
        //Align projectile to parent obj
        childObject.transform.parent = parentObj.transform;
        childObject.transform.localPosition = shotParent.transform.position;
        childObject.transform.localRotation = shotParent.transform.rotation;
    }
}
