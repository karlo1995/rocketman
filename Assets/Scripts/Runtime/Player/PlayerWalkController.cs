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
    
    public void SetDelayCauseOfLosingBalance()
    {
        isLosingBalance = true;
        StartCoroutine(LosingBalanceCoroutine());

    }
    
    public void MoveTowardMid(Transform midPosition, Action doneWalkingCallback)
    {
        this.midPosition = midPosition;
        this.doneWalkingCallback = doneWalkingCallback;
        
        if (!isLosingBalance)
        {
            StartCoroutine(MoveTowardMiddleCoroutine());
        }
    }

    private IEnumerator MoveTowardMiddleCoroutine()
    {
        yield return new WaitForSeconds(1f);
        
        PlayerAnimationController.Instance.FlipX(transform.position - midPosition.transform.position);
        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.WALK_ANIMATION_NAME, true);
        transform.DOMoveX(midPosition.position.x, 0.5f).OnComplete(() =>
        {
            doneWalkingCallback?.Invoke();
            doneWalkingCallback = null;
        });
    }

    private IEnumerator LosingBalanceCoroutine()
    {
        //if (PlayerCollisionController.Instance.isLanded)
        {
            yield return new WaitForSeconds(0.1f);

            PlayerAnimationController.Instance.FlipX(midPosition.transform.position - transform.position);
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.LOSING_BALANCE_NAME, false);

            yield return new WaitForSeconds(0.2f);

            StartCoroutine(MoveTowardMiddleCoroutine());
            isLosingBalance = false;
        }
    }
}
