using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    public int dmg;

    private void OnDestroy()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}
