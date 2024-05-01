using Script.Misc;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : Singleton<PlayerAnimationController>
{
    [SerializeField] private SkeletonAnimation spineSkeleton;
    [SerializeField] private Rigidbody2D rigidbody2D;

    [SerializeField] private SkeletonAnimation[] thrusterAnimationsHands;
    [SerializeField] private SkeletonAnimation[] thrusterAnimationsFeet;

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

    public void FlipX(Vector3 position)
    {
        var vel = transform.rotation * position;

        spineSkeleton.skeleton.FlipX = vel.x switch
        {
            > 0 => false,
            < 0 => true,
            _ => spineSkeleton.skeleton.FlipX
        };
    }
}