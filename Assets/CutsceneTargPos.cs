using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutsceneTargPos : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraMove2D camMove2D;
    [SerializeField] private PlayerMove2D playerMove;

    [Header("Settings")]
    [SerializeField] private float cameraMoveSpeed;
    [SerializeField] private float secondsToWait;
    [SerializeField] private bool unlockCameraAfterDone;
    
    [SerializeField] private bool disablePlayerMoveOnStart;
    [SerializeField] private bool enablePlayermoveAfterWait;
    [SerializeField] private UnityEvent onArriveEvent;
    [SerializeField] private UnityEvent afterWaitEvent;

    public void MoveCameraToPos()
    {
        if (disablePlayerMoveOnStart)
            playerMove.inCutscene = true;

        StartCoroutine(camMove2D.LerpToPos(transform, cameraMoveSpeed, DoArriveEvent, secondsToWait, DoAfterWaitEvent, unlockCameraAfterDone));
    }

    public void DoArriveEvent()
    {
        onArriveEvent?.Invoke();
    }

    public void DoAfterWaitEvent()
    {
        if (enablePlayermoveAfterWait)
            playerMove.inCutscene = false;

        afterWaitEvent?.Invoke();
    }
}
