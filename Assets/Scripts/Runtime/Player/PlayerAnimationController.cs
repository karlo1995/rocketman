using Script.Misc;
using Spine.Unity;
using UnityEngine;

public class PlayerAnimationController : Singleton<PlayerAnimationController>
{
    [SerializeField] private SkeletonAnimation spineSkeleton;
    [SerializeField] private Rigidbody2D rigidbody2D;

    [SerializeField] private SkeletonAnimation[] thrusterAnimations;

    private void Awake()
    {
        PlayThrusterAnimation(false);
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
    
    public void PlayThrusterAnimation(bool open)
    {
        foreach (var thruster in thrusterAnimations)
        {
            thruster.gameObject.SetActive(open);
        }
    }

    private void Update()
    {
        var vel = transform.rotation * rigidbody2D.velocity;

        spineSkeleton.skeleton.FlipX = vel.x switch
        {
            > 0 => true,
            < 0 => false,
            _ => spineSkeleton.skeleton.FlipX
        };
    }
}
