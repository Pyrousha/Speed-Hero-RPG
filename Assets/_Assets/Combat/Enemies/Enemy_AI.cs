using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_AI : MonoBehaviour
{
    [SerializeField] private float distanceToAggro;
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private NavMeshAgent distanceCalculator;
    [SerializeField] private Animator anim;

    public enum EnemyStateEnum
    {
        Roaming,
        Aggrod,
        Attacking,
        Dead
    }

    private EnemyStateEnum state;

    [Header("Strafe Properties")]
    [SerializeField] private float maxStrafeSpeed;
    [SerializeField] private float minRandStrafeTime;
    [SerializeField] private float maxRandStrafeTime;
    private float nextStrafeSwapTime;
    private float strafeSpeed;

    [Header("Attack Properties")]
    [SerializeField] private float minRandAttackTime;
    [SerializeField] private float maxRandAttackTime;
    private float nextAttackTime;

    [System.Serializable]
    public struct attack
    {
        public string[] triggerNames;
        public float targDistance;
    }
    [SerializeField] private attack[] attacks;

    private attack nextAttack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        switch (state)
        {
            case EnemyStateEnum.Roaming:
                {
                    distanceCalculator.SetDestination(PlayerMove2D.Instance.PlayerTransform.position);
                    float distToPlayer = distanceCalculator.remainingDistance;

                    if(distToPlayer <= distanceToAggro)
                    {
                        state = EnemyStateEnum.Aggrod;
                    }
                    break;
                }
            case EnemyStateEnum.Aggrod:
                {
                    //Set strafe speed
                    if (Time.time >= nextStrafeSwapTime)
                    {
                        float sign = Random.Range(-1, 1);
                        if (sign >= 0)
                            sign = 1;
                        else
                            sign = -1;

                        strafeSpeed = sign * maxStrafeSpeed;

                        nextStrafeSwapTime = Time.time + Random.Range(minRandStrafeTime, maxRandStrafeTime);
                    }


                    if (Time.time >= nextAttackTime) //Time to start new attack (or at least check)
                    {
                        if (InRange())
                        {
                            //Start Attack
                            DoAttack();
                            state = EnemyStateEnum.Attacking;

                            //Stop strafing
                            agent.SetDestination(transform.position);
                            agent.speed = 0;

                            //Look at player
                            Vector3 lookPos = PlayerMove2D.Instance.PlayerTransform.position - transform.position;
                            lookPos.y = 0;
                            Quaternion rot = Quaternion.LookRotation(lookPos);
                            transform.rotation = rot;

                            return;
                        }
                        else //Move towards target
                        {
                            //get direction to player
                            Vector3 dir = (PlayerMove2D.Instance.PlayerTransform.position - transform.position).normalized;
                            dir.y = 0;

                            //spir dir left or right depending on strafe speed
                            dir = Quaternion.Euler(0, 90, 0) * dir;
                            dir *= strafeSpeed / 8f;

                            Debug.Log("Dir after rotate: " + dir);

                            //Adjust target position based on distance to player
                            float distanceAway = (PlayerMove2D.Instance.PlayerTransform.position - transform.position).magnitude;
                            float distToMoveForward = distanceAway - nextAttack.targDistance;
                            dir += ((PlayerMove2D.Instance.PlayerTransform.position - transform.position).normalized * distToMoveForward);
                            dir.y = 0;

                            Debug.Log("Dir plus towards/away player: " + dir);

                            //Move in target direction
                            agent.SetDestination(transform.position + dir);

                            //Look at player (not needed for sprites)
                            /*Vector3 lookPos = PlayerMove2D.Instance.PlayerTransform.position - transform.position;
                            lookPos.y = 0;
                            Quaternion rot = Quaternion.LookRotation(lookPos);
                            transform.rotation = rot;*/

                            //Set animation values + navmesh Speed
                            //float strafeSpeedLerped = Mathf.Lerp(anim.GetFloat("StrafeSpeed"), strafeSpeed, Time.deltaTime * 8);
                            agent.speed = Mathf.Abs(strafeSpeed);
                            //anim.SetFloat("Speed", Mathf.Abs(strafeSpeed));
                        }
                    }
                    else  //Circle around player
                    {
                        //get direction to player
                        Vector3 dir = (PlayerMove2D.Instance.PlayerTransform.position - transform.position).normalized;
                        dir.y = 0;

                        //spir dir left or right depending on strafe speed
                        dir = Quaternion.Euler(0, 90, 0) * dir;
                        dir *= strafeSpeed;

                        //Adjust target position based on distance to player
                        float distanceAway = (PlayerMove2D.Instance.PlayerTransform.position - transform.position).magnitude;
                        float distDiff = distanceAway - nextAttack.targDistance;
                        Vector3 toPlayer = (PlayerMove2D.Instance.PlayerTransform.position - transform.position).normalized * distDiff;
                        toPlayer.y = 0;

                        dir += toPlayer;

                        //Move in target direction
                        agent.SetDestination(transform.position + dir);

                        //Look at player
                        Vector3 lookPos = PlayerMove2D.Instance.PlayerTransform.position - transform.position;
                        lookPos.y = 0;
                        Quaternion rot = Quaternion.LookRotation(lookPos);
                        transform.rotation = rot;

                        //Set animation values + navmesh Speed
                        //float strafeSpeedLerped = Mathf.Lerp(anim.GetFloat("StrafeSpeed"), strafeSpeed, Time.deltaTime * 8);
                        agent.speed = Mathf.Abs(strafeSpeed);
                        //anim.SetFloat("Speed", Mathf.Abs(strafeSpeed));
                    }

                    break;
                }
            case EnemyStateEnum.Attacking:
                {
                    break;
                }
        }
    }

    public bool InRange()
    {
        Vector3 dragPos = transform.position;
        Vector3 playerPos = PlayerMove2D.Instance.PlayerTransform.position;
        dragPos.y = 0;
        playerPos.y = 0;

        float distToTarg = Mathf.Abs((dragPos - playerPos).magnitude - nextAttack.targDistance);

        Debug.Log("DistToTarg: " + distToTarg);

        if (distToTarg <= 2.5f)
            return true;

        return false;
    }


    public void DoAttack()
    {
        foreach (string triggerStr in nextAttack.triggerNames)
            anim.SetTrigger(triggerStr);

        GetNextAttack();
    }

    private void GetNextAttack()
    {
        //Make next attack unique from the last one
        int nextAttackIndex = Random.Range(0, attacks.Length);
        /*while (nextAttackIndex == lastAttackIndex) //make attacks unique
        {
            nextAttackIndex = Random.Range(0, attacks.Length);
        }*/

        nextAttack = attacks[nextAttackIndex];
        //lastAttackIndex = nextAttackIndex;

        nextAttackTime = Time.time + Random.Range(minRandAttackTime, maxRandAttackTime);
    }
}
