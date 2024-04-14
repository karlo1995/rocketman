using Script.Misc;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : Singleton<PlayerAnimationController>
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Rigidbody2D rigidbody2D;
    
    public void PlayAnimation(string animationName, bool isLoop)
    {
        var animName = "novanerd_" + animationName;
        if (skeletonAnimation.AnimationName != animName)
        {
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
