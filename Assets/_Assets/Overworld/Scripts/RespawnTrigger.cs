using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    [SerializeField] private int layerAfterRespawn;

    public void OnTriggerEnter(Collider other)
    {
        PlayerMove2D.Instance.Respawn();
        PlayerMove2D.Instance.HeroSprite.sortingOrder = layerAfterRespawn;
    }
}
