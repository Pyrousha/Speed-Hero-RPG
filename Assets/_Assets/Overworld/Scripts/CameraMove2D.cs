using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove2D : Singleton<CameraMove2D>
{
    [SerializeField] private Transform target;

    [SerializeField] private Transform minPosX;
    [SerializeField] private Transform maxPosX;
    [SerializeField] private Transform minPosZ;
    [SerializeField] private Transform maxPosZ;

    private float minX = -9999;
    private float maxX = 9999;

    private float minZ = -9999;
    private float maxZ = 9999;

    private bool shouldLerp;
    [SerializeField] private float camMoveSpeed;

    [SerializeField] private List<GameObject> sideWalls;
    [SerializeField] private List<bool> wallsActive;

    private bool inCutscene;

    //[SerializeField] private float camMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        if (minPosX)
            minX = minPosX.position.x;
        if (maxPosX)
            maxX = maxPosX.position.y;

        if (minPosZ)
            minZ = minPosZ.position.z;
        if (maxPosZ)
            maxZ = maxPosZ.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (inCutscene)
            return;

        if (transform.position != target.position)
        {
            Vector3 targetPos = target.position;

            if (shouldLerp)
            {
                float moveSpeedThisFrame = camMoveSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.position, targetPos) <= moveSpeedThisFrame)
                    transform.position = targetPos;
                else
                {
                    Vector3 targetDir = (targetPos - transform.position).normalized;
                    transform.position += targetDir * moveSpeedThisFrame;
                }
            }
            else
            {
                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
                targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);

                transform.position = targetPos;
            }
        }
        else
        {
            if (shouldLerp)
                LerpDone();
        }
    }

    public void OnRespawn()
    {
        for(int i = 0; i< sideWalls.Count; i++)
        {
            wallsActive[i] = sideWalls[i].activeInHierarchy;
        }

        shouldLerp = true;

        PlayerMove2D.Instance.isRespawning = true;
    }

    public void LerpDone()
    {
        shouldLerp = false;
        PlayerMove2D.Instance.isRespawning = false;
    }

    public IEnumerator LerpToPos(Transform target, float moveSpeed, Action onArriveEvent, float secondsToWait, Action afterWaitEvent, bool moveToPlayerAfterDone)
    {
        inCutscene = true;
        while(true)
        {
            Vector3 targetPos = target.position;

            float moveSpeedThisFrame = moveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, targetPos) <= moveSpeedThisFrame)
            {
                transform.position = targetPos;
                break;
            }
            else
            {
                Vector3 targetDir = (targetPos - transform.position).normalized;
                transform.position += targetDir * moveSpeedThisFrame;
            }

            yield return null;
        }

        onArriveEvent?.Invoke();

        yield return new WaitForSeconds(secondsToWait);

        afterWaitEvent?.Invoke();

        if (moveToPlayerAfterDone)
            inCutscene = false;
    }
}
