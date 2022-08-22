using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairParent : MonoBehaviour
{
    [SerializeField] private int floorNum;
    public int FloorNum => floorNum;

    [System.Serializable]
    public enum Dir
    {
        left,
        right
    }

    [Header("Self-References (do not change)")]
    [SerializeField] private Transform destinationPos;
    public Vector3 TargPos => destinationPos.position;

    [Header("Settings")]
    [SerializeField] private Dir direction;
    [SerializeField] private StairParent linkedStair;

    public void TriggerEnter()
    {
        StairCanvas.Instance.TriggerHit(direction, linkedStair.TargPos, floorNum, linkedStair.FloorNum);

        gameObject.GetComponent<SingleSFXManager>().PlayClip(0);
    }
}
