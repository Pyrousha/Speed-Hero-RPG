using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackCube : MonoBehaviour
{
    public Sprite[] spriteArray;
    public SpriteRenderer sprObj;


    public Material mat_black;
    public Material mat_white;
    public MeshRenderer cubeRenderer;

    public LayerMask atkCubelayer;

    public int attackNum;

    private void Awake()
    {
        SetAttackNum();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAttackNum()
    {
        int xPos = Mathf.RoundToInt(transform.localPosition.x);
        int zPos = Mathf.RoundToInt(transform.localPosition.z);
        transform.localPosition = new Vector3(xPos, 0, zPos);

        gameObject.layer = 0; //change layer so this cube isn't checked for in collision test

        if (Physics.CheckSphere(transform.position, 0.2f, atkCubelayer))
        {
            Debug.Log("Invalid Position, already a cube here");
            Destroy(gameObject);
        }

        gameObject.layer = 8; //reassign layer

        attackNum = xPos + 5;
        changeVisualsToMatchAttackNum(attackNum);
    }

    void changeVisualsToMatchAttackNum(int attackNum)
    {
        if ((attackNum < 1) || (attackNum > 9))
        {
            Debug.Log("Invalid attack number");
            return;
        }

        //Set sprite to correct arrow
        sprObj.sprite = spriteArray[attackNum - 1];

        //Set cube to correct material
        if (attackNum % 2 == 0) //even
        {
            if (attackNum == 2) //2 is white
            {
                cubeRenderer.material = mat_white;
            }
            else //other evens are black
            {
                cubeRenderer.material = mat_black;
            }
        }
        else //odd
        {
            if (attackNum == 1) //1 is black
            {
                cubeRenderer.material = mat_black;
            }
            else //other odds are white
            {
                cubeRenderer.material = mat_white;
            }
        }
    }

    public void AddToEnemyPattern(Enemy_Stats_Combat enemy, float timePerEightNote)
    {
        enemy.Invoke("Spawn" + attackNum, transform.localPosition.z*timePerEightNote + 1f - (44f/60f));
    }
}
