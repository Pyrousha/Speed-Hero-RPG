using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDashManager : MonoBehaviour
{
    public static HeroDashManager Instance;

    public enum dashStateEnum
    {
        charged,
        dashing,
        recharging
    }

    [SerializeField] private dashStateEnum dashState;
    public dashStateEnum DashState => dashState;

    [SerializeField] private float dashDurationSecs;
    private float dashEndTime;

    [SerializeField] private float dashSpeed;

    [SerializeField] private float secondsPerAfterimage;
    private float nextAfterimageTime;

    [SerializeField] private float endDashSpeedMultiplier;

    [Header("References")]
    [SerializeField] private PlayerMove2D playerController;
    [SerializeField] private Transform playerVisualTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;

    Vector2 dir;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("Multiple HeroDashManagers found");
    }

    private void Update()
    {
        switch (dashState)
        {
            case dashStateEnum.charged:
                {
                    if (InputHandler.Instance.Dash.down)
                    {
                        StartDash();
                    }
                    break;
                }
            case dashStateEnum.dashing:
                {
                    //Check if dash should end
                    if (Time.time >= dashEndTime)
                    {
                        EndDash(false);
                        break;
                    }

                    //Set velocity
                    //rb.velocity = dir * dashSpeed;

                    //Check if afterimage should be spawned
                    if (Time.time >= nextAfterimageTime)
                    {
                        SpawnAfterImage();
                    }

                    break;
                }
            case dashStateEnum.recharging:
                {
                    if (playerController.IsGrounded)
                    {
                        dashState = dashStateEnum.charged;
                    }

                    break;
                }
        }
    }

    private void StartDash()
    {
        //Get input
        Vector2 directionHeld = playerController.dirFacing;

        if (directionHeld.magnitude <= 0.05f)
            return; //cancel dash

        dir = directionHeld;

        //Don't start dash if player can't move
        if (playerController.canMove == false)
            return;

        //Set dash sprite
        SetDashAnim(dir);

        //No gravy
        rb.useGravity = false;

        //Set timer and state
        dashState = dashStateEnum.dashing;
        dashEndTime = Time.time + dashDurationSecs;

        //Set velocity
        rb.velocity = new Vector3(dir.x * dashSpeed, rb.velocity.y+0.1f, dir.y * dashSpeed);

        //Spawn first afterimage + afterimage timer
        SpawnAfterImage();
    }

    private void SetDashAnim(Vector2 dir)
    {
        anim.ResetTrigger("DashEnd");

        if (dir.x >= 0.5f)
        {
            anim.Play("Hero_Dash_Right");
            return;
        }

        if (dir.x <= -0.5f)
        {
            anim.Play("Hero_Dash_Left");
            return;
        }

        if (dir.y >= 0.5f)
        {
            anim.Play("Hero_Dash_Up");
            return;
        }

        if (dir.y <= -0.5f)
        {
            anim.Play("Hero_Dash_Down");
            return;
        }
    }

    private void EndDash(bool refundDash)
    {
        //Make gravy normal
        rb.useGravity = true;

        rb.velocity *= endDashSpeedMultiplier;

        anim.SetTrigger("DashEnd");

        //Set state
        if (refundDash)
            dashState = dashStateEnum.charged;
        else
            dashState = dashStateEnum.recharging;
    }

    private void SpawnAfterImage()
    {
        GameObject newAfterImage = PlayerAfterImagePool.Instance.GetFromPool();
        newAfterImage.transform.position = transform.position;
        newAfterImage.transform.rotation = playerVisualTransform.rotation;

        //if(dir.y <= 0)
        {
            //Give priority to hero sprite over afterimage ones
            newAfterImage.transform.position += new Vector3(0, -0.005f, 0.005f);
        }

        newAfterImage.GetComponent<PlayerAfterImageSprite>().SetSprite(spriteRenderer.sprite);

        nextAfterimageTime = Time.time + secondsPerAfterimage;
    }
}
