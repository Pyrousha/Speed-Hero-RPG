using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero_Controller_Combat : MonoBehaviour
{
    #region controls
    private KeyCode leftButton; // = KeyCode.A;
    private KeyCode leftDiagButton; // = KeyCode.Q;
    private KeyCode upButton; // = KeyCode.W;
    private KeyCode rightDiagButton; // = KeyCode.E;
    private KeyCode rightButton; // = KeyCode.D;
    private KeyCode downButton; // = KeyCode.S;
    #endregion

    [Header("Debug Overrides")]
    public bool allowInstantAnimationCancelling;

    Animator animator;

    //State variables
    [System.NonSerialized] private bool canAttack = true;

    [Header("State Stuff")]
    public string currentState;
    public string nextState;
    
    public float queueThreshold;
    public float diagCancelPercent;

    public float animPercent;

    //float inputX;
    //float inputY;
    //float inputDiag;

    public bool xPressedATK;
    public bool yPressedATK;
    public bool diagPressedATK;

    public Vector2 attackInput;

    public HeroSpawnAttack spawnAttack;
    private Hero_Stats_Combat heroStatsCombat;
    [SerializeField] private Transform heroAttackSpawner;
    [SerializeField] private GameObject heroAttackPrefab;

    //Animation States
    const string HERO_NULL = "This is not a valid animation";
    const string HERO_IDLE = "Hero_idle";
    const string HERO_1_JUMP = "Hero_1_jump";
    const string HERO_2_ATTACK = "Hero_2_attack";
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
        heroStatsCombat = GetComponent<Hero_Stats_Combat>();
        currentState = HERO_IDLE;
        nextState = HERO_NULL;

        xPressedATK = false;
        yPressedATK = false;
    }

    public void LoadControls(CombatControlsManager manager)
    {
        leftButton = manager.blockInputLeft;
        leftDiagButton = manager.blockInputDiagLeft;
        upButton = manager.blockInputUp;
        rightDiagButton = manager.blockInputDiagRight;
        rightButton = manager.blockInputRight;
        downButton = manager.blockInputDown;
    }

    /// <summary>
    /// Changes the attack input to only be registered while being pressed down (holding button does not constantly attack)
    /// </summary>
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

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.J))
        {
            GameObject attackObj = Instantiate(heroAttackPrefab, null);
            attackObj.GetComponent<Hero_Attack_Projectile>().SetDamage(heroStatsCombat.Dmg);
        }*/

        //OldInputAttack();
        GetInputAndAttack();
    }

    /*public void OldInputAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentState != HERO_1_JUMP)
        {
            ChangeAnimationState(HERO_1_JUMP);
            return;
        }

        attackInput = getAttackInput();

        #region set animator bools for holding attack
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

        #region Diagonal Cancelling
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
        #endregion

        #region Set nextState based on input
        if ((allowInstantAnimationCancelling) || (currentState == HERO_IDLE) || (animPercent >= queueThreshold))
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
        #endregion

        //Set bool based on if hero has attack queued
        animator.SetBool("AttackQueued", nextState != HERO_NULL);

        //Change animation state
        if ((nextState != HERO_NULL) && ((currentState == HERO_IDLE) || (allowInstantAnimationCancelling)))
        {
            ChangeAnimationState(nextState);
            nextState = HERO_NULL;
        }
    }*/

    public void GetInputAndAttack()
    {
        if (canAttack)
        {
            #region Get nextState based on key down presses
            if (Input.GetKeyDown(leftButton))
                nextState = HERO_2_ATTACK;
            else
            {
                if (Input.GetKeyDown(leftDiagButton))
                    nextState = HERO_3_ATTACK;
                else
                {
                    if (Input.GetKeyDown(upButton))
                        nextState = HERO_5_ATTACK;
                    else
                    {
                        if (Input.GetKeyDown(rightDiagButton))
                            nextState = HERO_7_ATTACK;
                        else
                        {
                            if (Input.GetKeyDown(rightButton))
                                nextState = HERO_9_ATTACK;
                            else
                            {
                                if (Input.GetKeyDown(downButton))
                                    nextState = HERO_8_DODGE;
                                else
                                    nextState = HERO_NULL;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Set Animator bools for holding attacks
            animator.SetBool("Pressing_2", Input.GetKey(leftButton));
            animator.SetBool("Pressing_3", Input.GetKey(leftDiagButton));
            animator.SetBool("Pressing_5", Input.GetKey(upButton));
            animator.SetBool("Pressing_7", Input.GetKey(rightDiagButton));
            animator.SetBool("Pressing_8", Input.GetKey(downButton));
            animator.SetBool("Pressing_9", Input.GetKey(rightButton));
            #endregion

            //Change animation state
            if (nextState != HERO_NULL)
            {
                ChangeAnimationState(nextState);
                nextState = HERO_NULL;
            }
        }
        else //hero cannot attack, so act like nothing is being pressed
        {
            nextState = HERO_NULL;

            #region Set Animator bools for holding attacks
            animator.SetBool("Pressing_2", false);
            animator.SetBool("Pressing_3", false);
            animator.SetBool("Pressing_5", false);
            animator.SetBool("Pressing_7", false);
            animator.SetBool("Pressing_8", false);
            animator.SetBool("Pressing_9", false);
            #endregion
        }


    }

    public void SetCanAttack(bool newCanAttack)
    {
        canAttack = newCanAttack;
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

        //reassign current state
        currentState = newState;
    }

    /// <summary>
    /// Gets the player's input on the attack axis
    /// </summary>
    /// <returns>a normalized 2D vector of the attack input direcion</returns>
    Vector2 getAttackInput()
    {
        //Horizontal Input
        float inputX = Input.GetAxisRaw("Horizontal");

        //Vertical Input
        float inputY = Input.GetAxisRaw("Vertical");

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
