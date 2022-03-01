using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PathMove2D : MonoBehaviour
{
    [Header("Pathing")]
    [SerializeField] private bool shouldMove = false;
    [SerializeField] private Transform pathObjectParent;

    [SerializeField] private UnityEvent afterPathFinished;

    private List<Vector3> pathPoints = new List<Vector3>();
    private Vector3 targetPosition;

    private Rigidbody rb;

    [Header("Ground checking")]
    public bool onlyMoveOnGround;
    public LayerMask groundLayer;
    bool isGrounded = true;
    [SerializeField] private float raycastHeight;
    [SerializeField] private Transform raycastPointParent;
    private Transform[] raycastPoints;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    private float maxMoveSpeed; //= moveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;
    private float tempFrictionSpeed;


    // Start is called before the first frame update
    void Start()
    {
        maxMoveSpeed = moveSpeed;

        if (raycastPointParent != null)
        {
            raycastPoints = new Transform[raycastPointParent.childCount];

            for (int i = 0; i < raycastPointParent.childCount; i++)
            {
                raycastPoints[i] = raycastPointParent.GetChild(i);
            }
        }

        tempFrictionSpeed = frictionSpeed;

        rb = GetComponent<Rigidbody>();

        GeneratePath(pathObjectParent);

        if (shouldMove)
            AllowMovement();
    }

    public void GeneratePath(Transform pathParent)
    {
        if (pathParent == null)
        {
            Debug.Log("Unable to generate path, parent is null");
            return;
        }

        pathPoints = new List<Vector3>();

        for (int i = 0; i < pathParent.childCount; i++)
        {
            pathPoints.Add(pathParent.GetChild(i).position);
        }

        targetPosition = pathPoints[0];

        pathParent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (onlyMoveOnGround)
            isGrounded = CalcIsGrounded();

        if (shouldMove)
        {
            //if close enough to consider this point reached
            if (Vector3.Distance(transform.position, targetPosition) < 0.25f)
            {
                if (pathPoints.Count <= 1) //No points left, or this is the last one
                {
                    if (GetComponent<PlayerMove2D>() != null)
                        GetComponent<PlayerMove2D>().SetAnimatorValues(Vector2.zero);

                    afterPathFinished.Invoke();
                    DisableMovement();
                }
                else
                {
                    pathPoints.Remove(pathPoints[0]);
                    targetPosition = pathPoints[0];
                }
            }
            //move towards point
            else
            {
                SetVelocity(true);
            }
        }
        else
        {
            SetVelocity(false);
        }
    }

    public void DisableFriction()
    {
        if (frictionSpeed == 0)
            return;

        tempFrictionSpeed = frictionSpeed;
        frictionSpeed = 0;
    }

    public void EnableFriction(float timeUntilEnable)
    {
        if (timeUntilEnable > 0)
            Invoke("EnableFrictionDelayed", timeUntilEnable);
        else
            EnableFrictionDelayed();
    }

    private void EnableFrictionDelayed()
    {
        frictionSpeed = tempFrictionSpeed;
    }


    public void AllowMovement()
    {
        shouldMove = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void DisableMovement()
    {
        shouldMove = false;
        //rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    /// <summary>
    /// Moves linearly towards the next target position
    /// </summary>
    private void SetVelocity(bool shouldMove)
    {
        if (!isGrounded)
            return;

        Vector2 inputVect = new Vector2(0,0);

        //Calculate direction to move
        if (shouldMove)
        {
            Vector3 targetDir = targetPosition - transform.position;
            inputVect = new Vector2(targetDir.x, targetDir.z).normalized;
        }

        //Get variables ready for calculation
        float currSpeedX = rb.velocity.x;
        float currSpeedZ = rb.velocity.z;
        float newSpeedX = 0;
        float newSpeedZ = 0;

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


        //Debug.Log(inputVect);

        //Set velocity after calculation
        rb.velocity = new Vector3(newSpeedX, rb.velocity.y, newSpeedZ);

        if(GetComponent<PlayerMove2D>() != null)
            GetComponent<PlayerMove2D>().SetAnimatorValues(inputVect);
    }

    private bool CalcIsGrounded()
    {
        foreach (Transform pos in raycastPoints)
        {
            if (Physics.Raycast(pos.position, Vector3.down, raycastHeight, groundLayer))
            {
                return true;
            }
        }

        return false;
    }
}
