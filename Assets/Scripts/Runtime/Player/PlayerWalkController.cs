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
    
    public void SetDelayCauseOfLosingBalance(float distance)
    {
        isLosingBalance = true;
        StartCoroutine(LosingBalanceCoroutine(distance));

    }
    
    public void MoveTowardMid(Transform midPosition, float distance, Action doneWalkingCallback)
    {
        this.midPosition = midPosition;
        this.doneWalkingCallback = doneWalkingCallback;
        
        if (!isLosingBalance)
        {
            StartCoroutine(MoveTowardMiddleCoroutine(distance));
        }
    }

    private IEnumerator MoveTowardMiddleCoroutine(float distance)
    {
        yield return new WaitForSeconds(distance);
        if (midPosition != null)
        {
            PlayerAnimationController.Instance.FlipX(transform.position - midPosition.transform.position);
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
        //if (PlayerCollisionController.Instance.isLanded)
        {
            yield return new WaitForSeconds(0.1f);

            if (midPosition != null)
            {
                PlayerAnimationController.Instance.FlipX(midPosition.transform.position - transform.position);
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.LOSING_BALANCE_NAME, false);

                yield return new WaitForSeconds(0.2f);

                StartCoroutine(MoveTowardMiddleCoroutine(distance));
                isLosingBalance = false;
            }
        }
    }
}
