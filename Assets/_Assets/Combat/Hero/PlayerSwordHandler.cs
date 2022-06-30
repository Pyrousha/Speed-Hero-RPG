using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwordHandler : MonoBehaviour
{
    public static PlayerSwordHandler Instance;
    private Animator swordAnimator;
    [SerializeField] private Transform swordParent;

    [SerializeField] private float nudgeStrength;
    [SerializeField] private float bigNudgeStrength;

    private bool canQueueNextAttack = true;

    [SerializeField] private bool useMousePosForAttackDir;
    [SerializeField] private LayerMask attackLayer;

    private Vector2 attackDir;

    List<Enemy_Stats> hitEnemies;

    public enum AttackStateEnum
    {
        idle,
        attacking,
        endLag
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

        hitEnemies = new List<Enemy_Stats>();
    }

    private void Update()
    {
        if ((InputHandler.Instance.Attack.down) && (canQueueNextAttack) && (HeroInventory.Instance.HasSword))
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        if (CanStartAttack() == false)
            return;

        if(useMousePosForAttackDir)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit, 100f, attackLayer))
            {
                Vector3 dir = hit.point - transform.position;
                attackDir = new Vector2(dir.x, dir.z);
                attackDir = attackDir.normalized;
            }
            else
            {
                attackDir = new Vector2(0, 0);
                Debug.Log("Did not click a point??");
            }
        }
        else
        {
            attackDir = PlayerMove2D.Instance.GetDirectionFromInput();

            if (attackDir.magnitude < 0.05f) //if not pressing a move button
                attackDir = PlayerMove2D.Instance.dirFacing;
        }

        StopNudge();

        swordAnimator.SetTrigger("StartSwing");

        float angle = Vector2.SignedAngle(attackDir, new Vector2(-1, 0));
        swordParent.transform.eulerAngles = new Vector3(0, angle, 0);

        attackState = AttackStateEnum.attacking;
        canQueueNextAttack = false;
    }

    public bool CanStartAttack()
    {
        bool canMove = (HeroDashManager.Instance.DashState != HeroDashManager.dashStateEnum.dashing) &&        //Not dashing
           (MenuController.Instance.Interactable == false) &&                                                  //Menu closed
           (DialogueUI.Instance.isOpen == false);                                                              //Dialogue closed

        return canMove;
    }

    public void NudgeHero()
    {
        //canQueueNextAttack = true;
        PlayerMove2D.Instance?.NudgeHero(attackDir, nudgeStrength);
    }

    public void BigNudgeHero()
    {
        //canQueueNextAttack = true;
        PlayerMove2D.Instance?.NudgeHero(attackDir, bigNudgeStrength);
    }

    public void SetQueueNextAttackTrue()
    {
        canQueueNextAttack = true;
    }

    public void StopNudge()
    {
        canQueueNextAttack = true;
        PlayerMove2D.Instance?.StopNudge();
    }

    public void OnEndlagStart()
    {
        canQueueNextAttack = true;
        attackState = AttackStateEnum.endLag;

        hitEnemies = new List<Enemy_Stats>();
    }

    public void OnEndAttack()
    {
        attackState = AttackStateEnum.idle;
    }

    public void EnemyHit(Enemy_Stats enemy)
    {
        if (!hitEnemies.Contains(enemy))
        {
            //Enemy has not been hit yet
            hitEnemies.Add(enemy);
            enemy.TakeDamage(Hero_Stats.Instance.Damage);
        }
    }

    /// <summary>
    /// Called when wanting to stop the player from attacking.
    /// For example: being stunned, using dash, or using parry
    /// </summary>
    public void TryCancelAttack()
    {
        if(attackState != AttackStateEnum.idle)
        {
            //Stop animation
            swordAnimator.ResetTrigger("StartSwing");
            swordAnimator.Play("Idle");

            //Reset attack state and hit array
            attackState = AttackStateEnum.idle;
            hitEnemies = new List<Enemy_Stats>();
        }
    }
}
