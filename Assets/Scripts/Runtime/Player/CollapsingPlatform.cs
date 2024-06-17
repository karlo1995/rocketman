using System.Collections;
using Runtime.Levels.Platform_Scripts;
using UnityEngine;
using Spine.Unity;


public class CollapsingPlatform : MonoBehaviour
{
    //this is the available animation for collapsing for now..
    private const string crumble_shake_fast = "crumble_shake_fast";
    private const string crumble_shake_fastest = "crumble_shake_fastest";
    private const string crumble_shake_normal = "crumble_shake_normal";
    private const string fallingRocks = "Falling";
    private const string lookLikeNormal = "paused";
    
    [SerializeField] private SkeletonAnimation CollapsingAnimations;
    private PlatformController platformController;

    private void Awake()
    {
        platformController = GetComponent<PlatformController>();
    }

    public void CallCollapsingFunction()
    {
        StartCoroutine(CollapsingPlatformFunction());
    }

    public void ResetCollapsingPlatform()
    {
        PlayAnimation(crumble_shake_normal, true);
        gameObject.SetActive(true);
    }

    private IEnumerator CollapsingPlatformFunction()
    {
        PlayAnimation(crumble_shake_fastest, true);

        yield return new WaitForSeconds(5f);
        PlayAnimation(fallingRocks, false);
        yield return new WaitForSeconds(1.2f);
        PlayAnimation(lookLikeNormal, true);

        //check if nova nerd still collided with this platform before the collapse started
        //if yes play falling animation
        if (PlayerCollisionController.Instance.CurrentCollidedPlatform == platformController && 
            !PlayerDragController.Instance.IsReleased && !PlayerThrustController.Instance.IsThrusting)
        {
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FALLING_ANIMATION_NAME, true);
        }
        
        CollapsingAnimations.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        CollapsingAnimations.gameObject.SetActive(true);
    }

    private void PlayAnimation(string animationName, bool isLoop)
    {
        if (CollapsingAnimations.AnimationName != animationName)
        {
            CollapsingAnimations.loop = isLoop;
            CollapsingAnimations.AnimationName = animationName;
        }
    }
}
