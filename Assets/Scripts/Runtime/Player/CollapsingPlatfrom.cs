using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Script.Misc;
using Spine.Unity;


public class CollapsingPlatfrom : Singleton<CollapsingPlatfrom>
{
    [SerializeField] private SkeletonAnimation CollapsingAnimations;
    [SerializeField] private BoxCollider2D CollapsingCollider2D;

    //this is the available animation for collapsing for now..
    string crumble_shake_fast = "crumble_shake_fast";
    string crumble_shake_fastest = "crumble_shake_fastest";
    string crumble_shake_normal = "crumble_shake_normal";
    string fallingRocks = "Falling";
    string lookLikeNormal = "paused";

    public void CallCollapsingFunction()
    {
        StartCoroutine(CollapsingPlatformFunction());
    }

    IEnumerator CollapsingPlatformFunction()
    {
        PlayAnimation(crumble_shake_fastest, true);

        yield return new WaitForSeconds(5f);
        PlayAnimation(fallingRocks, false);
        yield return new WaitForSeconds(1.2f);
        PlayAnimation(lookLikeNormal, true);
        CollapsingAnimations.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        CollapsingAnimations.gameObject.SetActive(true);
        
        Debug.Log("collapse working");

    }

    public void PlayAnimation(string animationName, bool isLoop)
    {
        if (CollapsingAnimations.AnimationName != animationName)
        {
            CollapsingAnimations.loop = isLoop;
            CollapsingAnimations.AnimationName = animationName;
        }
    }
}
