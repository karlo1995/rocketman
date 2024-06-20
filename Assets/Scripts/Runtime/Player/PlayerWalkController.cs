using System;
using System.Collections;
using DG.Tweening;
using Script.Misc;
using UnityEngine;

public class PlayerWalkController : Singleton<PlayerWalkController>
{
    [SerializeField] private Transform pivotPoint;
    public Transform PivotPoint => pivotPoint;

    private bool isLosingBalance;
    private Transform midPosition;
    private Action doneWalkingCallback;
    private bool isLeftEdge;

    public void SetDelayCauseOfLosingBalance(bool isLeftEdge)
    {
        isLosingBalance = true;
        this.isLeftEdge = isLeftEdge;
        //StartCoroutine(LosingBalanceCoroutine(distance));
    }

    public void MoveTowardMid(Transform midPosition, float distance, Action doneWalkingCallback)
    {
        this.midPosition = midPosition;
        this.doneWalkingCallback = doneWalkingCallback;

        StartCoroutine(!isLosingBalance ? MoveTowardMiddleCoroutine(distance) : LosingBalanceCoroutine(distance));
    }

    private IEnumerator MoveTowardMiddleCoroutine(float distance)
    {
        yield return new WaitForSeconds(distance);
        if (midPosition != null)
        {
            PlayerAnimationController.Instance.ByRotationPlayerFlipX(transform.position - midPosition.transform.position);
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.WALK_ANIMATION_NAME, true);
            transform.DOMoveX(midPosition.position.x, 0.5f).OnComplete(() =>
            {
                doneWalkingCallback?.Invoke();
                doneWalkingCallback = null;
                midPosition = null;
            });
        }
    }

    private IEnumerator LosingBalanceCoroutine(float distance)
    {
        //TODO: band aid fix need to change please!!
        if(LevelManager.Instance != null)
        {
            yield return new WaitForSeconds(0.1f);

            if (midPosition != null)
            {
                //check what balance animation will play
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);

                var isLeftTarget = LevelManager.Instance.CurrentLandingPlatform.transform.position.x > LevelManager.Instance.CurrentTargetPlatform.transform.position.x;

                if (isLeftTarget)
                {
                    PlayerAnimationController.Instance.PlayAnimation(isLeftEdge ? AnimationNames.LOSING_BALANCE_NEAR_EDGE_NAME : AnimationNames.LOSING_BALANCE_FAR_EDGE_NAME, false);
                }
                else
                {
                    PlayerAnimationController.Instance.PlayAnimation(isLeftEdge ? AnimationNames.LOSING_BALANCE_FAR_EDGE_NAME : AnimationNames.LOSING_BALANCE_NEAR_EDGE_NAME, false);
                }

                yield return new WaitForSeconds(0.2f);

                StartCoroutine(MoveTowardMiddleCoroutine(distance));
                isLosingBalance = false;
            }
        }
    }
}