using UnityEngine;

public class PlayerMove2D : MonoBehaviour
{
    public static PlayerMove2D Instance;

    [Header("Debug Stuff")]
    [SerializeField] private bool hasSpeedCrystal;

    [Header("Self References")]
    public Rigidbody heroRB;
    public Animator heroAnim;
    public SpriteRenderer heroSprite;
    private PathMove2D pathMove2D;

    [Header("Movement")]
    public Vector2 inputVect;
    public Vector2 dirFacing { get; private set; }
    public float maxMoveSpeed;
    public float accelSpeed;
    public float frictionSpeed;

    public bool canMove { get; private set; }
    public Vector3 startingVelocity;

    [Header("Ground checking")]
    public LayerMask groundLayer;
    private bool isGrounded = true;
    public bool IsGrounded => isGrounded;

    [SerializeField] private float raycastHeight;
    public GameObject[] raycastPoints;

    [SerializeField] private Transform respawnTransform;
    private Vector3 respawnLocation; 

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Multiple PlayerMove2Ds found");

        canMove = true;

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

        //Not dashing, menu is closed, dialogue is closed, not attacking
        canMove =  (HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) &&        //Not dashing
                   (MenuController.Instance.Interactable == false) &&                                      //Menu closed
                   (DialogueUI.Instance.isOpen == false) &&                                                //Dialogue closed
                   (PlayerSwordHandler.Instance.AttackState != PlayerSwordHandler.AttackStateEnum.attacking);   //Not attacking


            foreach (GameObject go in raycastPoints) //check ground positions
            {
                if (Physics.Raycast(go.transform.position, Vector3.down, raycastHeight, groundLayer))
                {
                    isGrounded = true;
                    break;
                }
            }

        if (canMove)
        {
            inputVect = GetDirectionFromInput();
        }
        else
        {
            inputVect = new Vector2(0, 0);
        }

        if (inputVect.magnitude > 0)
            dirFacing = inputVect;

        SetAnimatorValues(inputVect);
    }

    private void FixedUpdate()
    {
        if(pathMove2D!= null && pathMove2D.enabled)
            return;

        if (HeroDashManager.Instance.DashState == HeroDashManager.dashStateEnum.dashing)
            return;

        ApplyFrictionAndAcceleration();
    }

    public void ApplyFrictionAndAcceleration()
    {
        if (!isGrounded)
            return;

        float currSpeedX = heroRB.velocity.x;
        float currSpeedZ = heroRB.velocity.z;

        float newSpeedX = currSpeedX;
        float newSpeedZ = currSpeedZ;

        #region Apply Friction
        //X-Friction
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

        //Z-Friction
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
        #endregion

        #region Apply Acceleration
        //X-Acceleration
        if (inputVect.x < 0) //pressing left
        {
            if (currSpeedX > maxMoveSpeed * inputVect.x) //can accelerate more left
            {
                //accelerate left
                newSpeedX = Mathf.Max(currSpeedX - accelSpeed, maxMoveSpeed * inputVect.x);
            }
        }
        else
        {
            if (inputVect.x > 0) //pressing right
            {
                if (currSpeedX < maxMoveSpeed * inputVect.x) //can accelerate more right
                {
                    //accelerate right
                    newSpeedX = Mathf.Min(currSpeedX + accelSpeed, maxMoveSpeed * inputVect.x);
                }
            }
        }

        //Z-Acceleration
        if (inputVect.y < 0) //pressing down
        {
            if (currSpeedZ > maxMoveSpeed * inputVect.y) //can accelerate more down
            {
                //accelerate down
                newSpeedZ = Mathf.Max(currSpeedZ - accelSpeed, maxMoveSpeed * inputVect.y);
            }
        }
        else
        {
            if (inputVect.y > 0) //pressing up
            {
                if (currSpeedZ < maxMoveSpeed * inputVect.y) //can accelerate more up
                {
                    //accelerate up
                    newSpeedZ = Mathf.Min(currSpeedZ + accelSpeed, maxMoveSpeed * inputVect.y);
                }
            }
        }
        #endregion

        heroRB.velocity = new Vector3(newSpeedX, heroRB.velocity.y, newSpeedZ);
    }

    public void NudgeHero(Vector2 dir, float nudgeStrength)
    {
        heroRB.velocity = new Vector3(nudgeStrength * dir.x, heroRB.velocity.y, nudgeStrength * dir.y);

        SetAnimatorValues(dir);
    }

    public void StopNudge()
    {
        if(HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing)
            heroRB.velocity = new Vector3(0, heroRB.velocity.y, 0);
    }

    public Vector2 GetDirectionFromInput()
    {
        Vector2 dir = InputHandler.Instance.Direction;

        return dir;
    }

    public void SetAnimatorValues(Vector2 inputVect)
    {
        //Player stopped moving keys, set dir value to get idle anim
        if (inputVect.sqrMagnitude < 0.05f)        
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
        heroAnim.SetBool("InAir", !isGrounded && HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing);
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

        Hero_Stats.Instance.TakeDamage(0.5f);
    }
}
