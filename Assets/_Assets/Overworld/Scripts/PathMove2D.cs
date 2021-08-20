using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMove2D : MonoBehaviour
{
    [SerializeField] private bool shouldMove = false;
    [SerializeField] private Transform pathObjectParent;
    
    private List<Vector3> pathPoints = new List<Vector3>();
    private Vector3 targetPosition;

    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelSpeed;
    [SerializeField] private float frictionSpeed;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < pathObjectParent.childCount; i++)
        {
            pathPoints.Add(pathObjectParent.GetChild(i).position);
        }

        targetPosition = pathPoints[0];

        pathObjectParent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shouldMove)
        {
            //if close enough to consider this point reached
            if (Vector3.Distance(transform.position, targetPosition) < 0.25f)
            {
                if (pathPoints.Count <= 1) //No points left, or this is the last one
                {
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
                Move();
            }
        }
    }

    public void AllowMovement()
    {
        shouldMove = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void DisableMovement()
    {
        shouldMove = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    /// <summary>
    /// Moves linearly towards the next target position
    /// </summary>
    private void Move()
    {
        //Calculate direction to move
        Vector3 targetDir = targetPosition - transform.position;
        Vector2 inputVect = new Vector2(targetDir.x, targetDir.z).normalized;

        //Get variables ready for calculation
        float currSpeedX = rb.velocity.x;
        float currSpeedZ = rb.velocity.z;
        float newSpeedX = 0;
        float newSpeedZ = 0;

        #region calculate xSpeed
        if (inputVect.x < 0) //pressing left
        {
            //accelerate left
            newSpeedX = Mathf.Max(currSpeedX - accelSpeed, moveSpeed * inputVect.x);
        }
        else
        {
            if (inputVect.x > 0) //pressing right
            {
                //accelerate right
                newSpeedX = Mathf.Min(currSpeedX + accelSpeed, moveSpeed * inputVect.x);
            }
            else //pressing nothing, x-friction
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
                //accelerate up
                newSpeedZ = Mathf.Min(currSpeedZ + accelSpeed, moveSpeed * inputVect.y);

            }
            else //pressing nothing, z-friction
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
        #endregion

        //Set velocity after calculation
        rb.velocity = new Vector3(newSpeedX, rb.velocity.y, newSpeedZ);
    }
}
