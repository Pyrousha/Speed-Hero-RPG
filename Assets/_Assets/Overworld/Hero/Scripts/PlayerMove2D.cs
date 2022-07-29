using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMove2D : Singleton<PlayerMove2D>
{
    [Header("Self References")]
    public Rigidbody heroRB;
    public Animator heroAnim;
    [SerializeField] private SpriteRenderer heroSprite;
    public SpriteRenderer HeroSprite => heroSprite;
    private PathMove2D pathMove2D;
    [SerializeField] private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;
    [SerializeField] private Transform interactObj;

    [SerializeField] private List<Transform> hitboxEdges; //right, up, left, down
    [SerializeField] private float walkoffDistance;
    [SerializeField] private float walkoffSpeedMultiplier;

    [Header("Movement")]
    public Vector2 inputVect;
    public Vector2 dirFacing { get; private set; }
    public float maxMoveSpeed;
    public float accelSpeed;
    public float frictionSpeed;

    private float interactDir = 270;

    public bool canMove { get; private set; }
    public Vector3 startingVelocity;

    [Header("Ground checking")]
    [SerializeField] private float antiSlideMultiplier;
    public LayerMask walkableGroundLayer;
    [SerializeField] private LayerMask terrainLayer;
    private bool isGrounded = true;
    public bool IsGrounded => isGrounded;
    private List<Vector3> groundedPositions = new List<Vector3>();
    private int maxPositions = 5;
    private float checkIntervalTime = 0.125f;

    [SerializeField] private float raycastHeight;
    [SerializeField] private Transform raycastParent;
    private List<Transform> raycastPoints;

    [SerializeField] private Transform respawnTransform;
    private Vector3 respawnLocation;

    [Header("EventArray")]
    [SerializeField] private List<EventNameAndEvent> unityEvents;
    [System.Serializable]
    public struct EventNameAndEvent
    {
        public EventNames eventName;
        public UnityEvent unityEvent;
    }
    [SerializeField] public enum EventNames
    {
        pathmoveToPlayermove,
        playermoveToPathmove
    }

    public bool MenusClosed()
    {
        //menu is closed, dialogue is closed
        return ((MenuController.Instance.Interactable == false) &&  //menu not open
                (DialogueUI.Instance.isOpen == false) &&            //dialogue not open
                (enabled));                                         //PlayerMove2D enabled
    }

    public bool CanParry()
    {
        //Not dashing, menu is closed, dialogue is closed
        bool canParry = ((HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) &&        //Not dashing
               MenusClosed());                                                                                   //Can act

        if(canParry)
            PlayerSwordHandler.Instance.TryCancelAttack();

        return canParry;
    }

    public bool CanAttack()
    {
        return ((HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) && MenusClosed());
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;

        if (respawnTransform != null)
            respawnLocation = respawnTransform.position;
        else
            respawnLocation = transform.position;

        heroRB.velocity = startingVelocity;

        pathMove2D = GetComponent<PathMove2D>();

        raycastPoints = Utils.GetChildrenFromParent(raycastParent);

        StartCoroutine(RespawnPointChecking());
    }

    private IEnumerator RespawnPointChecking()
    {
        while (true)
        {
            if (isGrounded)
            {
                if (groundedPositions.Count == maxPositions)
                {
                    groundedPositions.RemoveAt(0);
                }

                groundedPositions.Add(transform.position);
                yield return new WaitForSeconds(checkIntervalTime);
            }
            else
            {
                yield return null;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Not dashing, menu is closed, dialogue is closed, not attacking
        canMove =  (HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) &&        //Not dashing
                   (MenuController.Instance.Interactable == false) &&                                      //Menu closed
                   (DialogueUI.Instance.isOpen == false) &&                                                //Dialogue closed
                   (PlayerSwordHandler.Instance.AttackState != PlayerSwordHandler.AttackStateEnum.attacking);   //Not attacking

        isGrounded = false;
        foreach (Transform rayPoint in raycastPoints) //check ground positions
        {
            //Debug.DrawLine(rayPoint.position, rayPoint.position + Vector3.down*raycastHeight, Color.magenta, 0.1f, false);
            if (Physics.Raycast(rayPoint.position, Vector3.down, raycastHeight, walkableGroundLayer))
            {
                isGrounded = true;
                break;
            }
        }

        Vector2 inputVect_Unchanged = new Vector2(0, 0);

        if (canMove)
        {
            inputVect = GetDirectionFromInput();
            inputVect_Unchanged = inputVect;

            //change input vect to prevent walking off edges
            //horizontal
            if (inputVect.x < 0)
            {
                Vector3 startPos = hitboxEdges[0].transform.position + new Vector3(-walkoffDistance, 0.1f, 0.1f);
                if (!Physics.Raycast(startPos, new Vector3(0,-1,-1), 1, terrainLayer))
                {
                    //going to walk off soon
                    inputVect = new Vector2(inputVect.x * walkoffSpeedMultiplier, inputVect.y);
                }
            }
            else
            {
                if (inputVect.x > 0)
                {
                    Vector3 startPos = hitboxEdges[2].transform.position + new Vector3(walkoffDistance, 0.1f, 0.1f);
                    if (!Physics.Raycast(startPos, new Vector3(0, -1, -1), 1, terrainLayer))
                    {
                        //going to walk off soon
                        inputVect = new Vector2(inputVect.x * walkoffSpeedMultiplier, inputVect.y);
                    }
                }
            }

            //vertical
            if (inputVect.y > 0)
            {
                Vector3 startPos = hitboxEdges[3].transform.position + new Vector3(0, 0.1f, 0.1f + walkoffDistance);
                if (!Physics.Raycast(startPos, new Vector3(0, -1, -1), 1, terrainLayer))
                {
                    //going to walk off soon
                    inputVect = new Vector2(inputVect.x, inputVect.y * walkoffSpeedMultiplier);
                }
            }
            else
            {
                if (inputVect.y < 0)
                {
                    Vector3 startPos = hitboxEdges[1].transform.position + new Vector3(0, 0.1f, 0.1f - walkoffDistance);
                    if (!Physics.Raycast(startPos, new Vector3(0, -1, -1), 1, terrainLayer))
                    {
                        //going to walk off soon
                        inputVect = new Vector2(inputVect.x * walkoffSpeedMultiplier, inputVect.y * walkoffSpeedMultiplier);
                    }
                }
            }
        }
        else
        {
            inputVect = new Vector2(0, 0);
        }

        if (inputVect_Unchanged.magnitude > 0.05f)
        {
            dirFacing = inputVect_Unchanged;

            float unroundedAngle = Vector2.Angle(Vector2.right, inputVect_Unchanged.normalized);
            float newInteractDir = Utils.RoundToNearest(unroundedAngle, 45f);
            if (inputVect_Unchanged.y < 0)
                newInteractDir *= -1;

            if(newInteractDir != interactDir)
            {
                interactDir = newInteractDir;
                interactObj.localEulerAngles = new Vector3(0, -interactDir, 0);
            }
        }

        SetAnimatorValues(inputVect_Unchanged);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Terrain-Walkable"))
        {
            //Collided with wall, unslide if going up
            if(heroRB.velocity.y > 0)
            {
                heroRB.velocity = new Vector3(heroRB.velocity.x, -10f, heroRB.velocity.z);
            }
        }
    }

    private void FixedUpdate()
    {
        if (pathMove2D != null && pathMove2D.enabled)
            return;

        if (HeroDashManager.Instance.DashState == HeroDashManager.dashStateEnum.dashing)
            return;

        //Apply directional gravity when in air
        if (!isGrounded)
        {
            //Vector3 zGrav = new Vector3(0, 0, -9.81f);
            //heroRB.AddForce(zGrav * heroRB.mass);
        }
        else
        {
            ApplyFrictionAndAcceleration();
        }
    }

    public void ApplyFrictionAndAcceleration()
    {
        //This is only called when the player is grounded

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
        if (inputVect.magnitude < 0.05f)        
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
        //transform.position = respawnLocation;
        transform.position = groundedPositions[0];

        heroRB.velocity = new Vector3(0, 0, 0);
        heroSprite.sortingOrder = 1;

        Hero_Stats.Instance.TakeDamage(0.5f);
    }

    public void MoveAlongPath(Transform pathParent)
    {
        unityEvents[(int)EventNames.playermoveToPathmove].unityEvent.Invoke();
        pathMove2D.GeneratePath(pathParent);
        pathMove2D.AllowMovement();
    }

    public void SetCanMoveAfterPath(bool canMoveAfterPath)
    {
        if (canMoveAfterPath)
            pathMove2D.SetAfterPathEvent(unityEvents[(int)EventNames.pathmoveToPlayermove].unityEvent);
        else
            pathMove2D.SetAfterPathEvent(null);
    }
}
