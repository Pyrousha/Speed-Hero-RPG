using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenwipeCanvas : Singleton<ScreenwipeCanvas>
{
    [SerializeField] private Animator anim;
    [SerializeField] private bool blackToClearOnStart;

    // Start is called before the first frame update
    void Start()
    {
        if (blackToClearOnStart)
            BlackToClear();
    }

    public void BlackToClear()
    {
        anim.SetTrigger("BlackToClear");
    }

    public void ClearToBlack()
    {
        anim.SetTrigger("ClearToBlack");
    }
}
