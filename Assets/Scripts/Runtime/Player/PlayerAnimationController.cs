using System;
using System.Collections;
using Script.Misc;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : Singleton<PlayerAnimationController>
{
    [SerializeField] private SkeletonAnimation spineSkeleton;
    [SerializeField] private Rigidbody2D rigidbody2D;

    [SerializeField] private SkeletonAnimation[] thrusterAnimationsHands;
    [SerializeField] private SkeletonAnimation[] thrusterAnimationsFeet;
/*
    public GameObject target;


    void Update()
    {

        Vector3 directionToTarget = target.transform.position - transform.position;

        if (spineSkeleton != null)
        {
            // Flip the skeleton based on the x direction of the target
            if (directionToTarget.x < 0)
            {
                spineSkeleton.skeleton.ScaleX = Mathf.Abs(spineSkeleton.Skeleton.ScaleX); // Face right
            }
            else if (directionToTarget.x > 0)
            {
                spineSkeleton.skeleton.ScaleX = -Mathf.Abs(spineSkeleton.Skeleton.ScaleX); // Face left

            }


        }
    }*/

    public bool GetPlayerFlipX()
    {
        return spineSkeleton.skeleton.FlipX;
    }
    
    private void Awake()
    {
        PlayThrusterAnimation(false, false);
        PlayAnimation(AnimationNames.IDLE_ANIMATION_NAME, true);
    }
    
    public void PlayAnimation(string animationName, bool isLoop)
    {
        animationName = "Main_Animations/" + animationName;
        if (spineSkeleton.AnimationName != animationName)
        {
            spineSkeleton.loop = isLoop;
            spineSkeleton.AnimationName = animationName;
        }
    }

    public void PlayThrusterAnimation(bool isFloating, bool isThrusting)
    {
        foreach (var thrusterHands in thrusterAnimationsHands)
        {
            thrusterHands.gameObject.SetActive(isFloating);
        }
        
        foreach (var thrusterFeet in thrusterAnimationsFeet)
        {
            thrusterFeet.gameObject.SetActive(isThrusting);
        }
    }
    
    public void ByRotationPlayerFlipX(Vector3 position)
    {
        var vel = transform.rotation * position;

        spineSkeleton.skeleton.FlipX = vel.x switch
        {
            > 0 => false,
            < 0 => true,
            _ => spineSkeleton.skeleton.FlipX
        };
    }
    
    public void ByPositionPlayerFlipX(Vector3 position)
    {
        spineSkeleton.skeleton.FlipX = transform.position.x < position.x;
    }
}