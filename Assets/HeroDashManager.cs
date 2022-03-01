using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroDashManager : MonoBehaviour
{
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

    Vector2 dir;

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
                    if (playerController.isGrounded)
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

        //No movin
        playerController.SetCanMove(false);

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

    private void EndDash(bool refundDash)
    {
        //Can move again
        playerController.SetCanMove(true);

        //Make gravy normal
        rb.useGravity = true;

        rb.velocity *= endDashSpeedMultiplier;

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
            newAfterImage.transform.position += new Vector3(0, -0.05f, 0.05f);
        }

        newAfterImage.GetComponent<PlayerAfterImageSprite>().SetSprite(spriteRenderer.sprite);

        nextAfterimageTime = Time.time + secondsPerAfterimage;
    }
}
