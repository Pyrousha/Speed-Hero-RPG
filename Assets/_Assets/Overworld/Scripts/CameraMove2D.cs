using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove2D : MonoBehaviour
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
        if (transform.position != target.position)
        {
            Vector3 targetPos = target.position;

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);

            transform.position = targetPos;
        }
    }
}
