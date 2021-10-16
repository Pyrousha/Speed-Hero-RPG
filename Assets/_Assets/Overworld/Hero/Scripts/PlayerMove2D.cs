using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    [Header("Debug Stuff")]
    [SerializeField] private bool hasSpeedCrystal;

    [Header("Dashing")]
    private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    private float dashTimer;
    enum dashStateEnum
    {
        charged,
        dashing,
        recharging
    }
    [SerializeField] private dashStateEnum dashState;

    [Header("Self References")]
    public Rigidbody heroRB;
    public Animator heroAnim;
    public SpriteRenderer heroSprite;
    private PathMove2D pathMove2D;

    [Header("Movement")]
    public Vector2 inputVect;
    public Vector2 dirFacing;
    public float moveSpeed;
    public float accelSpeed;
    public float frictionSpeed;

    public bool canMove;
    public Vector3 startingVelocity;

    [Header("Ground checking")]
    public LayerMask groundLayer;
    bool isGrounded = true;

    [SerializeField] private float raycastHeight;
    public GameObject[] raycastPoints;

    [SerializeField] private Transform respawnTransform;
    private Vector3 respawnLocation; 

    [Header("Dialogue References")]
    [SerializeField] private OverworldInputHandler overworldInputHandler;
    [SerializeField] private DialogueUI dialogueUI;

    // Start is called before the first frame update
    void Start()
    {
        if (respawnTransform != null)
            respawnLocation = respawnTransform.position;
        else
            respawnLocation = transform.position;

        heroRB.velocity = startingVelocity;

        pathMove2D = GetComponent<PathMove2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = false;

        foreach (GameObject go in raycastPoints)
        {
            if (Physics.Raycast(go.transform.position, Vector3.down, raycastHeight, groundLayer))
            {
                isGrounded = true;
                break;
            }
        }

        //if (Mathf.Abs(heroRB.velocity.y) < 0.1)
        //  isGrounded = true;
        if ((canMove) && (!dialogueUI.isOpen))
            inputVect = GetDirectionFromInput();
        else
            inputVect = new Vector2(0, 0);

        if (inputVect.magnitude > 0)
            dirFacing = inputVect;

        switch (dashState)
        {
            case dashStateEnum.charged:
                {
                    if ((isGrounded) && (Input.GetKeyDown(dashKey)))
                        StartDash();
                    break;
                }
            case dashStateEnum.dashing:
                {
                    dashTimer -= Time.deltaTime;
                    if (dashTimer <= 0)
                        EndDash();
                    break;
                }
            case dashStateEnum.recharging:
                {
                    break;
                }
        }

        if (dashState != dashStateEnum.dashing)
        {
            if (!isGrounded)
            {
                inputVect *= 0.5f;
                SetAnimatorValues(new Vector2(0, 0));
            }
            else
                SetAnimatorValues(inputVect);
        }
    }

    private void StartDash()
    {
        canMove = false;
        Vector3 dashDir = new Vector3(dirFacing.x, 0, dirFacing.y);
        heroRB.velocity = dashDir * dashSpeed;
        heroRB.useGravity = false;

        dashState = dashStateEnum.dashing;
        dashTimer = dashDuration;
    }

    private void EndDash()
    {
        canMove = true;
        heroRB.useGravity = true;
        heroRB.velocity *= (1f / 5f);

        dashState = dashStateEnum.charged;
    }

    private void FixedUpdate()
    {
        if(pathMove2D!= null && pathMove2D.enabled)
            return;

        if (dashState == dashStateEnum.dashing)
            return;

        ApplyFriction();
    }

    public void ApplyFriction()
    {
        float currSpeedX = heroRB.velocity.x;
        float currSpeedZ = heroRB.velocity.z;

        float newSpeedX = currSpeedX;
        float newSpeedZ = currSpeedZ;

        if (isGrounded)
        {
            #region calculate xSpeed
            if (inputVect.x < 0) //pressing left
            {
                //if (currSpeedX > moveSpeed * inputVect.x)
                {
                    //accelerate left
                    newSpeedX = Mathf.Max(currSpeedX - accelSpeed, moveSpeed * inputVect.x);
                }
            }
            else
            {
                if (inputVect.x > 0) //pressing right
                {
                    //if (currSpeedX < moveSpeed * inputVect.x)
                    {
                        //accelerate right
                        newSpeedX = Mathf.Min(currSpeedX + accelSpeed, moveSpeed * inputVect.x);
                    }
                }
                else //pressing nothing, x-friction
                {
                    //if (isGrounded)
                    {
                        if (currSpeedX < 0) //moving left
                        {
                            newSpeedX = Mathf.Min(0, currSpeedX + frictionSpeed);
                        }
                        else
                        {
                            if (currSpeedX > 0) //moving right
                            {
                                newSpeedX = Mathf.Max(0, currSpeedX - frictionSpeed);
                            }
                        }
                    }
                }
            }
            #endregion

            #region calculate zSpeed
            if (inputVect.y < 0) //pressing down
            {
                //if (currSpeedZ > -moveSpeed)
                {
                    //accelerate down
                    newSpeedZ = Mathf.Max(currSpeedZ - accelSpeed, moveSpeed * inputVect.y);
                }
            }
            else
            {
                if (inputVect.y > 0) //pressing up
                {
                    //if (currSpeedZ < moveSpeed)
                    {
                        //accelerate up
                        newSpeedZ = Mathf.Min(currSpeedZ + accelSpeed, moveSpeed * inputVect.y);
                    }
                }
                else //pressing nothing, z-friction
                {
                    //if (isGrounded)
                    {
                        if (currSpeedZ < 0) //moving left
                        {
                            newSpeedZ = Mathf.Min(0, currSpeedZ + frictionSpeed);
                        }
                        else
                        {
                            if (currSpeedZ > 0) //moving right
                            {
                                newSpeedZ = Mathf.Max(0, currSpeedZ - frictionSpeed);
                            }
                        }
                    }
                }
            }
            #endregion
        }

        //heroRB.MovePosition(heroRB.position + new Vector3(inputVect.x, 0, inputVect.y) * moveSpeed * Time.fixedDeltaTime);

        //heroRB.velocity = new Vector3(inputVect.x * moveSpeed, heroRB.velocity.y, inputVect.y * moveSpeed);

        heroRB.velocity = new Vector3(newSpeedX, heroRB.velocity.y, newSpeedZ);
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
                        dirFacing = new Vector2(1, 0);
                        heroAnim.SetInteger("Dir", 0);
                        break;
                    }
                case ("Hero-up"):
                    {
                        dirFacing = new Vector2(0, 1);
                        heroAnim.SetInteger("Dir", 1);
                        break;
                    }
                case ("Hero-left"):
                    {
                        dirFacing = new Vector2(-1, 0);
                        heroAnim.SetInteger("Dir", 2);
                        break;
                    }
                case ("Hero-down"):
                    {
                        dirFacing = new Vector2(0, -1);
                        heroAnim.SetInteger("Dir", 3);
                        break;
                    }
            }

        }
        heroAnim.SetFloat("Horizontal", inputVect.x * 2);
        heroAnim.SetFloat("Vertical", inputVect.y);
        heroAnim.SetFloat("Speed", inputVect.sqrMagnitude);
        heroAnim.SetBool("InAir", !isGrounded);
    }

    public void SetRespawnLocation(Transform newRespawn)
    {
        respawnLocation = newRespawn.position;
    }

    public void Respawn()
    {
        transform.position = respawnLocation;
        heroRB.velocity = new Vector3(0, 0, 0);
        heroSprite.sortingOrder = 1;
    }

    public void SetCanMove(bool newCanMove)
    {
        canMove = newCanMove;
    }
}
