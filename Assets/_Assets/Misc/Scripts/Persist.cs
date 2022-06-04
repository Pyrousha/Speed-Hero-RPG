using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persist : MonoBehaviour
{
    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.parent = null;
            Debug.Log(gameObject.name + "'s parent set to null");
        }
        DontDestroyOnLoad(gameObject);
    }
}
