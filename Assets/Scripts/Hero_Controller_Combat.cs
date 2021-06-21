using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Controller_Combat : MonoBehaviour
{
    [Header("Debug Overrides")]
    public bool allowInstantAnimationCancelling;

    Animator animator;

    [Header("State Stuff")]
    public string currentState;
    public string nextState;
    
    public float queueThreshold;
    public float diagCancelPercent;

    public float animPercent;


    float inputX;
    float inputY;
    float inputDiag;

    public bool xPressedATK;
    public bool yPressedATK;
    public bool diagPressedATK;

    Vector2 moveInput;
    public Vector2 attackInput;

    public HeroSpawnAttack spawnAttack;

    //Animation States
    const string HERO_NULL = "This is not a valid animation";
    const string HERO_IDLE = "Hero_idle";
    const string HERO_2_ATTACK = "Hero_2_attack";
    const string HERO_2_ATTACK_ENDLAG = "Attack_2_exit";
    const string HERO_3_ATTACK = "Hero_3_attack";
    const string HERO_5_ATTACK = "Hero_5_attack";
    const string HERO_7_ATTACK = "Hero_7_attack";
    const string HERO_8_DODGE = "Hero_8_dodge_start";
    const string HERO_9_ATTACK = "Hero_9_attack";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        spawnAttack = GetComponent<HeroSpawnAttack>();
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

        #region set animator bools
        bool press2 = false;
        bool press3 = false;
        bool press5 = false;
        bool press7 = false;
        bool press8 = false;
        bool press9 = false;

        if (attackInput.y > 0) //pressing Up
            press5 = true;
        else
        {
            if (attackInput.y < 0) //pressing Down
            {
                press8 = true;
            }
        }

        if (attackInput.x < 0) //pressing Left
        {
            if (press5)
                press3 = true;
            press2 = true;
        }
        else
        {
            if (attackInput.x > 0) //pressing Right
            {
                if (press5)
                    press7 = true;
                press9 = true;
            }  
        }

        animator.SetBool("Pressing_2", press2);
        animator.SetBool("Pressing_3", press3);
        animator.SetBool("Pressing_5", press5);
        animator.SetBool("Pressing_7", press7);
        animator.SetBool("Pressing_8", press8);
        animator.SetBool("Pressing_9", press9);
        #endregion

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

        //Set nextState based on input
        if ((currentState == HERO_IDLE) || (animPercent >= queueThreshold) || (allowInstantAnimationCancelling))
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
                        {
                            nextState = HERO_5_ATTACK;
                            break;
                        }
                        if (attackInput.y < 0)
                        {
                            nextState = HERO_8_DODGE;
                            break;
                        }

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

        animator.SetBool("AttackQueued", nextState != HERO_NULL);

        if ((nextState != HERO_NULL) && ((currentState == HERO_IDLE) || (allowInstantAnimationCancelling)))
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

    /// <summary> 
    /// Changes what animation the hero sprite is currently playing 
    /// </summary>
    /// <param name="newState">state to start playing</param>
    void ChangeAnimationState(string newState)
    {
        
        //Don't change to Null
        if (newState == HERO_NULL)
        {
            Debug.Log("Tried changed to state HERO_NULL");
            return;
        }

        //stop animator from interrupting itself if invalid
        if (newState == currentState)
        {
            //Debug.Log("Animation " + newState + " interrupted itself");
            if (newState == HERO_IDLE)
                return;
        }

        //Debug.Log("Changing animationstate from: " + currentState + " to: " + newState);

        //play animation
        animator.Play(newState, 0, 0f);


        /*
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(newState))
        {
            //Debug.Log("Unity why ;_;");
        }*/

        //reassign current state and nextstate
        currentState = newState;
    }

    /// <summary>
    /// Gets the player's input on the move axis
    /// </summary>
    /// <returns>a normalized 2D vector of the move input direcion</returns>
    Vector2 getMoveInput()
    {
        //Horizontal Input
        float inputX = Input.GetAxisRaw("MovementX");

        //Vertical Input
        float inputY = Input.GetAxisRaw("MovementY");

        return new Vector2(inputX, inputY).normalized;
    }

    /// <summary>
    /// Gets the player's input on the attack axis
    /// </summary>
    /// <returns>a normalized 2D vector of the attack input direcion</returns>
    Vector2 getAttackInput()
    {
        //Horizontal Input
        float inputX = Input.GetAxisRaw("AttackX");

        //Vertical Input
        float inputY = Input.GetAxisRaw("AttackY");

        //Diag Input
        float inputDiag = Input.GetAxisRaw("AttackDiags");
        if (inputDiag != 0)
        {
            inputY = 1;
            inputX = inputDiag;
        }

        return new Vector2(inputX, inputY).normalized;
    }
}
