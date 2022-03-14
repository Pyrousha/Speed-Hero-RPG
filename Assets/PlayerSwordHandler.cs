using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordHandler : MonoBehaviour
{
    public static PlayerSwordHandler Instance;
    private Animator swordAnimator;
    [SerializeField] private Transform swordParent;

    [SerializeField] private float nudgeStrength;

    private bool canQueueNextAttack = true;

    public enum AttackStateEnum
    {
        idle,
        attacking,
        endLad
    }

    private AttackStateEnum attackState;
    public AttackStateEnum AttackState => attackState;

    // Start is called before the first frame update
    void Start()
    {
        swordAnimator = GetComponent<Animator>();

        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Multiple PlayerSwordHandlers found");
    }

    private void Update()
    {
        if ((InputHandler.Instance.Attack.down) && (canQueueNextAttack))
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        //if (PlayerMove2D.Instance.canMove == false)
            //return;

        StopNudge();

        swordAnimator.SetTrigger("StartSwing");

        float angle = Vector2.SignedAngle(PlayerMove2D.Instance.dirFacing, new Vector2(-1, 0));
        swordParent.transform.eulerAngles = new Vector3(0, angle, 0);

        attackState = AttackStateEnum.attacking;
    }

    public void NudgeHero()
    {
        canQueueNextAttack = true;
        PlayerMove2D.Instance.NudgeHero(nudgeStrength);
    }

    public void StopNudge()
    {
        canQueueNextAttack = true;
        PlayerMove2D.Instance.StopNudge();
    }

    public void EndAttack()
    {
        attackState = AttackStateEnum.idle;
    }
}
