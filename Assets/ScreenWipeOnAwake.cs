using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWipeOnAwake : MonoBehaviour
{
    [SerializeField] Animator anim;

    private void Start()
    {
        anim.SetTrigger("StartScreenWipe");
    }

    private void OnEnable()
    {
        anim.SetTrigger("StartScreenWipe");
    }
}
