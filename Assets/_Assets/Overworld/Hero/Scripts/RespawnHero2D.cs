using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnHero2D : MonoBehaviour
{
    public Transform heroTransform;
    Vector3 respawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        respawnPoint = heroTransform.position;
    }

    public void Respawn()
    {
        heroTransform.position = respawnPoint;
    }
}
