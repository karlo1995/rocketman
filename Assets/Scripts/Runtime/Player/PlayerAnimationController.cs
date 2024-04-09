using System;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public static PlayerAnimationController Instance;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Rigidbody2D rigidbody2D;
    
    private void Awake()
    {
        Instance = this;
    }

    public void PlayAnimation(string animationName, bool isLoop)
    {
        var animName = "novanerd_" + animationName;
        if (skeletonAnimation.AnimationName != animName)
        {
            Debug.Log("sadddd: " + animName + " loop: " + isLoop);
            skeletonAnimation.loop = isLoop;
            skeletonAnimation.AnimationName = animName;
        }
    }

    private void Update()
    {
        var vel = transform.rotation * rigidbody2D.velocity;

        skeletonAnimation.skeleton.FlipX = vel.x switch
        {
            > 0 => true,
            < 0 => false,
            _ => skeletonAnimation.skeleton.FlipX
        };
    }
}
