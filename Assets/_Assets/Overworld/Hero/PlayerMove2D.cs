using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    public Rigidbody heroRB;
    public Animator heroAnim;

    public GameObject heroSpriteObj;

    public Vector2 inputVect;
    public float moveSpeed;

    public LayerMask groundLayer;
    bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, groundLayer);

        if (isGrounded)
            inputVect = GetDirectionFromInput();
        else
            inputVect = new Vector2(0, 0);

        SetAnimatorValues(inputVect);
    }

    private void FixedUpdate()
    {
        heroRB.MovePosition(heroRB.position + new Vector3(inputVect.x, 0, inputVect.y) * moveSpeed * Time.fixedDeltaTime);
    }

    public Vector2 GetDirectionFromInput()
    {
        //Vertical Input
        float inputY = Input.GetAxisRaw("Vertical");

        //Horizontal Input
        float inputX = Input.GetAxisRaw("Horizontal"); 

        Vector2 dirVector = new Vector2(inputX, inputY);
        dirVector.Normalize();

        return dirVector;
    }

    public void SetAnimatorValues(Vector2 inputVect)
    {
        //Player stopped moving keys, set dir value to get idle anim
        if (inputVect.sqrMagnitude < 0.01)
        {
            switch(heroAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name)
            {
                case ("Hero-right"):
                    {
                        heroAnim.SetInteger("Dir", 0);
                        break;
                    }
                case ("Hero-up"):
                    {
                        heroAnim.SetInteger("Dir", 1);
                        break;
                    }
                case ("Hero-left"):
                    {
                        heroAnim.SetInteger("Dir", 2);
                        break;
                    }
                case ("Hero-down"):
                    {
                        heroAnim.SetInteger("Dir", 3);
                        break;
                    }
            }

        }
        heroAnim.SetFloat("Horizontal", inputVect.x * 2);
        heroAnim.SetFloat("Vertical", inputVect.y);
        heroAnim.SetFloat("Speed", inputVect.sqrMagnitude);
    }

}
