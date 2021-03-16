using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Controller_Combat : MonoBehaviour
{
    Animator animator;
    public string currentState;
    public string nextState;
    
    public float queueThreshold;
    public float diagCancelPercent;

    public float animPercent;


    float inputX;
    float inputY;

    public bool xPressedATK;
    public bool yPressedATK;

    Vector2 moveInput;
    public Vector2 attackInput;

    public SpawnAttack spawnAttack;

    //Animation States
    const string HERO_NULL = "This is not a valid animation";
    const string HERO_IDLE = "Hero_idle";
    const string HERO_2_ATTACK = "Hero_2_attack";
    const string HERO_3_ATTACK = "Hero_3_attack";
    const string HERO_5_ATTACK = "Hero_5_attack";
    const string HERO_7_ATTACK = "Hero_7_attack";
    const string HERO_9_ATTACK = "Hero_9_attack";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spawnAttack = GetComponent<SpawnAttack>();
        currentState = HERO_IDLE;
        nextState = HERO_NULL;

        xPressedATK = false;
        yPressedATK = false;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = getMoveInput();

    }

    void AttackAxisToInputDown()
    {
        //X axis
        if (attackInput.x != 0)
        {
            if (xPressedATK == false) //X is not being pressed, update var and proceed as normal
            {
                xPressedATK = true;
            }
            else //X has already been pressed
            {
                attackInput = new Vector2(0, attackInput.y);
            }
        }
        else //X is not being pressed
        {
            xPressedATK = false;
        }

        //Y axis
        if (attackInput.y != 0)
        {
            if (yPressedATK == false) //Y is not being pressed, update var and proceed as normal
            {
                yPressedATK = true;
            }
            else //Y has already been pressed
            {
                attackInput = new Vector2(attackInput.x, 0); //Set Y input to 0
            }
        }
        else //Y is not being pressed
        {
            yPressedATK = false;
        }

        //Normalize vector
        attackInput = attackInput.normalized;
    }

    void TransitionLeftDiag()
    {
        ChangeAnimationState(HERO_3_ATTACK);
    }

    void TransitionRightDiag()
    {
        ChangeAnimationState(HERO_7_ATTACK);
    }

    private void FixedUpdate()
    {
        attackInput = getAttackInput();
        AttackAxisToInputDown();

        animPercent = (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f);

        //Diagonal Cancelling
        if (animPercent <= diagCancelPercent)
        {
            switch (currentState)
            {
                case (HERO_2_ATTACK): //Attacking left
                    {
                        if (attackInput.y > 0) //pressed up
                        {
                            TransitionLeftDiag();
                        }
                        break;
                    }

                case (HERO_5_ATTACK): //Attacking up
                    {
                        if (attackInput.x < 0) //pressed left
                        {
                            TransitionLeftDiag();
                            break;
                        }
                        if (attackInput.x > 0) //pressed right
                        {
                            TransitionRightDiag();
                            break;
                        }
                        break;
                    }

                case (HERO_9_ATTACK): //Attacking right
                    {
                        if (attackInput.y > 0) //pressed up
                        {
                            TransitionRightDiag();
                        }
                        break;
                    }

            }
        }


        //animPercent = (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f);
        if ((currentState == HERO_IDLE) || (animPercent >= queueThreshold))
        {
            switch (attackInput.x)
            {
                case (-1):
                    {
                        //Attack Left
                        nextState = HERO_2_ATTACK;
                        break;
                    }
             
                case (0):
                    {
                        //Attack Up
                        if (attackInput.y > 0)
                            nextState = HERO_5_ATTACK;
                        break;
                    }

                case (1):
                    {
                        //Attack Right
                        nextState = HERO_9_ATTACK;
                        break;
                    }

                default: //X is some value (-1 < x < 0) or (0 < x < 1)
                    {
                        if (attackInput.y > 0) //pressing up
                        { 
                            if (attackInput.x < 0) //up left
                            {
                                nextState = HERO_3_ATTACK;
                                break;
                            }
                            else //up right
                            {
                                nextState = HERO_7_ATTACK;
                                break;
                            }
                        }
                        break;
                    }
            }
        }

        if (currentState == HERO_IDLE)
        {
            ChangeAnimationState(nextState);
            nextState = HERO_NULL;
            //Debug.Log("NextState: " + nextState);
        }

    }

    public void transitionToIdle()
    {
        //Debug.Log("Changing Anim to Idle. CurrState: " + currentState + " NextState: " + nextState);
        ChangeAnimationState(HERO_IDLE);
        //Debug.Log("ANIM CHANGED. CurrState: " + currentState + " NextState: " + nextState);
    }

    void ChangeAnimationState(string newState)
    {
        
        //Don't change to Null
        if (newState == HERO_NULL)
        {
            newState = HERO_IDLE;
        }

        //stop animator from interrupting itself
        if (newState == currentState)
        {
            return;
        }

        //Debug.Log("Changing animationstate from: " + currentState + " to: " + newState);
        
        //play animation
        animator.Play(newState);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(newState))
        {
            //Debug.Log("FUCK YOU UNITY");
        }

        //reassign current state and nextstate
        currentState = newState;
    }

    Vector2 getMoveInput()
    {
        //Horizontal Input
        float inputX = Input.GetAxisRaw("MovementX");

        //Vertical Input
        float inputY = Input.GetAxisRaw("MovementY");

        return new Vector2(inputX, inputY).normalized;
    }

    Vector2 getAttackInput()
    {
        //Horizontal Input
        float inputX = Input.GetAxisRaw("AttackX");

        //Vertical Input
        float inputY = Input.GetAxisRaw("AttackY");

        return new Vector2(inputX, inputY).normalized;
    }
}
