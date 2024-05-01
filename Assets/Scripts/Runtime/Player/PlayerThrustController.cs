using System;
using Cinemachine;
using UnityEngine;

public class PlayerThrustController : MonoBehaviour
{
    //[SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] public float brakeSpeed;
    [SerializeField] public float fuelSpeed;
    [SerializeField] private float dragTimer = 0.5f;

    private bool isBraked;
    private bool isDragged;
    private float dragTimerCounter;
    private bool canThrust;

    private void Update()
    {
        if (PlayerDragController.Instance.IsReleased && !PlayerCollisionController.Instance.IsLanded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragTimerCounter = 0f;
                isDragged = false;
                isBraked = false;
                canThrust = true;
            }

            if (Input.GetMouseButton(0) && !isDragged)
            {
                dragTimerCounter += Time.deltaTime;

                if (dragTimerCounter >= dragTimer)
                {
                    isDragged = true;
                }
            }

            if (Input.GetMouseButtonUp(0) && canThrust)
            {
                if (!isDragged)
                {
                    isBraked = true;
                }

                canThrust = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (PlayerDragController.Instance.IsReleased)
        {
            if (isDragged)
            {
                if (canThrust)
                {
                    //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.8f;

                    PlayerAnimationController.Instance.PlayAnimation(AnimationNames.THRUST_ANIMATION_NAME, true);
                    PlayerAnimationController.Instance.PlayThrusterAnimation(true, true);

                    rigidbody2D.velocity = PlayerAnimationController.Instance.GetPlayerFlipX() ? 
                        new Vector2(rigidbody2D.velocity.x + fuelSpeed / 10f, rigidbody2D.velocity.y + fuelSpeed) : 
                        new Vector2(rigidbody2D.velocity.x - fuelSpeed / 10f, rigidbody2D.velocity.y + fuelSpeed);
                }
                else
                {
                    PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, false);
                    PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);


                    // if (!isBraked)
                    // {
                    //     virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.2f;
                    //     PlayerAnimationController.Instance.PlayAnimation("hardbreak", false);
                    //
                    //     isDragged = false;
                    //     rigidbody2D.AddForce(-brakeSpeed * rigidbody2D.velocity);
                    // }
                }
                // rigidbody2D.AddForce(fuelSpeed * Vector2.up, ForceMode2D.Force);
            }
            else if (isBraked)
            {
                //virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.2f;
                isBraked = false;
                canThrust = false;

                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.BRAKE_ANIMATION_NAME, false);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
                
                rigidbody2D.AddForce(-brakeSpeed * rigidbody2D.velocity);
                //rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x / 2 - brakeSpeed, rigidbody2D.velocity.y);
                //Invoke(nameof(RemoveForce), 2f);
            }
        }
    }

    private void RemoveForce()
    {
        if (!isDragged)
        {
            rigidbody2D.velocity = Vector2.zero;
            isBraked = false;
        }
    }
}