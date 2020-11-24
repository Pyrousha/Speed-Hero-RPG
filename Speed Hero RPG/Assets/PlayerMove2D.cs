using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    public Rigidbody2D heroRB;

    public float moveSpeed;

    //Movement Keys
    KeyCode upKey = KeyCode.W;
    KeyCode downKey = KeyCode.S;
    KeyCode leftKey = KeyCode.A;
    KeyCode rightKey = KeyCode.D;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 velocityVect = moveSpeed * GetDirectionFromInput();
        heroRB.velocity = velocityVect;
    }

    public Vector2 GetDirectionFromInput()
    {
        //Vertical Input
        float inputY = 0;

        if (Input.GetKey(upKey))
            inputY++;

        if (Input.GetKey(downKey))
            inputY--;


        //Horizontal Input
        float inputX = 0;

        if (Input.GetKey(rightKey))
            inputX++;

        if (Input.GetKey(leftKey))
            inputX--;


        Vector2 dirVector = new Vector2(inputX, inputY);
        dirVector.Normalize();

        return dirVector;
    }

}
