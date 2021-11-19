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
        float zPos = Mathf.RoundToInt(transform.localPosition.z*2)/2f;
        transform.localPosition = new Vector3(xPos, 0, zPos);

        gameObject.layer = 0; //change layer so this cube isn't checked for in collision test

        attackNum = xPos + 5;

        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f, atkCubelayer);
        if (colliders.Length > 0)
        {
            //Debug.Log("Invalid Position, already a cube here");

            if ((Input.GetMouseButtonDown(0))&&(attackNum == 10))
                colliders[0].GetComponent<SongEvent>().IncrementIndex();

            Destroy(gameObject);
        }

        gameObject.layer = 8; //reassign layer

        changeVisualsToMatchAttackNum(attackNum);
    }

    void changeVisualsToMatchAttackNum(int attackNum)
    {
        if ((attackNum < 1) || (attackNum > 10))
        {
            Debug.Log("Invalid attack number");
            return;
        }

        //Set sprite to correct arrow
        sprObj.sprite = spriteArray[attackNum - 1];

        //Set cube to correct material
        if (attackNum % 2 == 0) //even
        {
            if ((attackNum == 2) || (attackNum == 10)) //2 and 10 are white
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

        if (attackNum == 10)
            gameObject.AddComponent<SongEvent>();
    }

    public void AddToEnemyPattern(Enemy_Stats_Combat enemy, float timePerEightNote, float startOffset)
    {
        if (attackNum == 1) //Jump attacks have differnt spawn timings
            enemy.Invoke("Spawn" + attackNum, startOffset + transform.localPosition.z * timePerEightNote - ((32.5f) / 60f));
        else
            enemy.Invoke("Spawn" + attackNum, startOffset + transform.localPosition.z*timePerEightNote - ((20f)/60f));
    }

    public void RemoveFromEnemyPattern(Enemy_Stats_Combat enemy)
    {
        enemy.CancelInvoke();
    }
}
