﻿using System;
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
    [SerializeField] private PathMove2D pathMove2D;
    [SerializeField] private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;
    [SerializeField] private Transform interactObj;

    //[SerializeField] private List<Transform> hitboxEdges; //right, up, left, down
    [SerializeField] private float walkoffDistance;
    [SerializeField] private float walkoffSpeedMultiplier;
    private List<Transform> rightPoints;
    private List<Transform> leftPoints;
    private List<Transform> upPoints;
    private List<Transform> downPoints;

    [Header("Movement")]
    public Vector2 inputVect;
    public Vector2 dirFacing { get; private set; }
    public float maxMoveSpeed;
    public float accelSpeed;
    public float frictionSpeed;

    private float interactDir = 270;

    public bool canMove { get; private set; }
    public Vector3 startingVelocity;
    public bool isRespawning { get; set; }

    [Header("Ground checking")]
    [SerializeField] private float antiSlideMultiplier;
    public LayerMask walkableGroundLayer;
    [SerializeField] private LayerMask terrainLayer;
    private bool isGrounded = false;
    public bool IsGrounded => isGrounded;
    private List<Vector3> groundedPositions = new List<Vector3>();
    private int maxPositions = 5;
    private float checkIntervalTime = 0.125f;

    [SerializeField] private float raycastHeight;
    [SerializeField] private Transform raycastParent;
    private List<Transform> raycastPoints;

    [SerializeField] private LayerMask fallingPlatformLayer;
    [SerializeField] private Transform centerHitboxLocation;
    //private List<FallingPlatform> platformsToRespawn = new List<FallingPlatform>();
    private List<FallingPlatform> currStoodOnPlatforms = new List<FallingPlatform>();

    [Header("EventArray")]
    [SerializeField] private List<EventNameAndEvent> unityEvents;
    [System.Serializable]
    public struct EventNameAndEvent
    {
        public EventNames eventName;
        public UnityEvent unityEvent;
    }
    [SerializeField]
    public enum EventNames
    {
        pathmoveToPlayermove,
        playermoveToPathmove
    }

    public bool MenusClosed()
    {
        //menu is closed, dialogue is closed
        return ((PauseMenuController.Instance.Interactable == false) &&  //menu not open
                (DialogueUI.Instance.isOpen == false) &&            //dialogue not open
                (enabled));                                         //PlayerMove2D enabled
    }

    public bool CanParry()
    {
        //Not dashing, menu is closed, dialogue is closed
        bool canParry = ((HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) &&        //Not dashing
               MenusClosed());                                                                                   //Can act

        if (canParry)
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

        heroRB.velocity = startingVelocity;

        raycastPoints = Utils.GetChildrenFromParent(raycastParent);

        rightPoints = new List<Transform> { raycastPoints[2], raycastPoints[5], raycastPoints[8] };
        leftPoints = new List<Transform> { raycastPoints[0], raycastPoints[3], raycastPoints[6] };
        upPoints = new List<Transform> { raycastPoints[0], raycastPoints[1], raycastPoints[2] };
        downPoints = new List<Transform> { raycastPoints[6], raycastPoints[7], raycastPoints[8] };


        StartCoroutine(RespawnPointChecking());
    }

    private IEnumerator RespawnPointChecking()
    {
        while (true)
        {
            if (isGrounded)
            {
                RaycastHit hit;
                Ray ray = new Ray(centerHitboxLocation.position, Vector3.down);
                //Debug.DrawLine(ray.origin, ray.origin + ray.direction * (raycastHeight + 0.1f), Color.red, 0.5f);
                if (Physics.Raycast(ray, out hit, raycastHeight, walkableGroundLayer))
                {
                    if (hit.collider.gameObject.CompareTag("NoRespawn"))
                    {
                        //not valid for respawning, so do not add to array of respawn locations
                    }
                    else
                    {
                        //valid respawn location
                        groundedPositions.Add(transform.position);
                    }
                }

                if (groundedPositions.Count > maxPositions)
                {
                    groundedPositions.RemoveAt(0);
                }

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
        canMove = (HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) &&                //Not dashing
                   (PauseMenuController.Instance.Interactable == false) &&                                              //Menu closed
                   (DialogueUI.Instance.isOpen == false) &&                                                        //Dialogue closed
                   (PlayerSwordHandler.Instance.AttackState != PlayerSwordHandler.AttackStateEnum.attacking) &&    //Not attacking
                   !isRespawning;                                                                                  //Not respawning

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

        #region Falling Platforms
        //See which platforms the player is currently standing on
        List<FallingPlatform> platformsHitThisFrame = new List<FallingPlatform>();
        for (int j = 0; j < raycastPoints.Count; j++)
        {
            Transform rayPoint = raycastPoints[j];

            RaycastHit hit;
            Ray ray = new Ray(rayPoint.position, Vector3.down);
            if (Physics.Raycast(ray, out hit, raycastHeight, fallingPlatformLayer))
            {
                FallingPlatform currPlatform = hit.collider.gameObject.GetComponent<FallingPlatform>();
                if (platformsHitThisFrame.Contains(currPlatform))
                    continue;
                else
                    platformsHitThisFrame.Add(currPlatform);
            }
        }

        //make any platforms the player is no longer standing on fall
        int i = 0;
        while (i < currStoodOnPlatforms.Count)
        {
            FallingPlatform currPlatform = currStoodOnPlatforms[i];
            if (platformsHitThisFrame.Contains(currPlatform))
            {
                //player still standing on this platform
            }
            else
            {
                //player stepped off platform, make it fall
                //platformsToRespawn.Add(currPlatform);
                currPlatform.Fall();
                currStoodOnPlatforms.RemoveAt(i);
                continue;
            }
            i++;
        }

        RaycastHit hit0;
        Ray ray0 = new Ray(centerHitboxLocation.position, Vector3.down);
        if (Physics.Raycast(ray0, out hit0, raycastHeight, fallingPlatformLayer))
        {
            FallingPlatform platform = hit0.collider.gameObject.GetComponent<FallingPlatform>();
            if (!currStoodOnPlatforms.Contains(platform))
                currStoodOnPlatforms.Add(platform);
        }
        #endregion

        Vector2 inputVect_Unchanged = new Vector2(0, 0);

        if (canMove)
        {
            inputVect = GetDirectionFromInput();
            inputVect_Unchanged = inputVect;

            float rayDistance = 10;

            //change input vect to prevent walking off edges
            //horizontal
            if (inputVect.x < 0)
            {
                bool terrainHit = false;
                foreach (Transform pos in rightPoints)
                {
                    Vector3 startPos = pos.position + new Vector3(-walkoffDistance, 5f, 5f);
                    //Debug.DrawLine(startPos, new Vector3(0, -1, -1) * rayDistance, Color.red);
                    if (Physics.Raycast(startPos, new Vector3(0, -1, -1), rayDistance, terrainLayer))
                    {
                        terrainHit = true;
                        break;
                    }
                }

                if (terrainHit == false)
                {
                    //going to walk off soon
                    inputVect = new Vector2(inputVect.x * walkoffSpeedMultiplier, inputVect.y);
                    Debug.Log("About to walk off in direction -X");
                }
            }
            else
            {
                if (inputVect.x > 0)
                {
                    bool terrainHit = false;
                    foreach (Transform pos in leftPoints)
                    {
                        Vector3 startPos = pos.position + new Vector3(walkoffDistance, 5f, 5f);
                        //Debug.DrawLine(startPos, new Vector3(0, -1, -1) * rayDistance, Color.red);
                        if (Physics.Raycast(startPos, new Vector3(0, -1, -1), rayDistance, terrainLayer))
                        {
                            terrainHit = true;
                            break;
                        }
                    }

                    if (terrainHit == false)
                    {
                        //going to walk off soon
                        inputVect = new Vector2(inputVect.x * walkoffSpeedMultiplier, inputVect.y);
                        Debug.Log("About to walk off in direction +X");
                    }
                }
            }

            //vertical
            if (inputVect.y > 0)
            {
                bool terrainHit = false;
                foreach (Transform pos in downPoints)
                {
                    Vector3 startPos = pos.position + new Vector3(0, 5f, 5f + walkoffDistance);
                    //Debug.DrawLine(startPos, new Vector3(0, -1, -1) * rayDistance, Color.red);
                    if (Physics.Raycast(startPos, new Vector3(0, -1, -1), rayDistance, terrainLayer))
                    {
                        terrainHit = true;
                        break;
                    }
                }

                if (terrainHit == false)
                {
                    //going to walk off soon
                    inputVect = new Vector2(inputVect.x, inputVect.y * walkoffSpeedMultiplier);
                    Debug.Log("About to walk off in direction +Y");
                }
            }
            else
            {
                if (inputVect.y < 0)
                {
                    bool terrainHit = false;
                    foreach (Transform pos in upPoints)
                    {
                        Vector3 startPos = pos.position + new Vector3(0, 5f, 5f - walkoffDistance);
                        //Debug.DrawLine(startPos, new Vector3(0, -1, -1) * rayDistance, Color.red);
                        if (Physics.Raycast(startPos, new Vector3(0, -1, -1), rayDistance, terrainLayer))
                        {
                            terrainHit = true;
                            break;
                        }
                    }

                    if (terrainHit == false)
                    {
                        //going to walk off soon
                        inputVect = new Vector2(inputVect.x, inputVect.y * walkoffSpeedMultiplier);
                        Debug.Log("About to walk off in direction -Y");
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

            if (newInteractDir != interactDir)
            {
                interactDir = newInteractDir;
                interactObj.localEulerAngles = new Vector3(0, -interactDir, 0);
            }
        }

        SetAnimatorValues(inputVect_Unchanged, isGrounded);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Terrain-Walkable"))
        {
            //Collided with wall, unslide if going up
            if (heroRB.velocity.y > 0)
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

        SetAnimatorValues(dir, isGrounded);
    }

    public void StopNudge()
    {
        if (HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing)
            heroRB.velocity = new Vector3(0, heroRB.velocity.y, 0);
    }

    public Vector2 GetDirectionFromInput()
    {
        Vector2 dir = InputHandler.Instance.Direction;

        return dir;
    }

    public void SetAnimatorValues(Vector2 inputVect, bool isCurrentlyGrounded)
    {
        //Player stopped moving keys, set dir value to get idle anim
        if (inputVect.magnitude < 0.05f)
        {
            switch (heroAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name)
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
        heroAnim.SetBool("InAir", !isCurrentlyGrounded && HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing);
    }

    public void Respawn()
    {
        //transform.position = respawnLocation;
        transform.position = groundedPositions[0];

        heroRB.velocity = new Vector3(0, 0, 0);
        heroSprite.sortingOrder = 1;

        Hero_Stats.Instance.TakeDamage(0.5f);

        /*
        foreach(FallingPlatform platform in platformsToRespawn)
        {
            platform.Rise();
        }*/
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
