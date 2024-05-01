using System.Collections;
using Script.Misc;
using UnityEngine;

public class PlayerCollisionController : Singleton<PlayerCollisionController>
{
    public bool isLanded;
    public bool IsLanded => isLanded;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out Platform platform))
            {
                if (transform.position.y > platform.transform.position.y)
                {
                    Debug.Log("Collided!");
                    isLanded = true;
                    PlayerDragController.Instance.SetCollidedPlatform(platform);
                    platform.StartCollisionBehaviors();

                    StartCoroutine(SetPlayerIdleAnimation());
                    PlayerDragController.Instance.SetCanDrag();
                }
            }
        }
    }

    public void SetIsLandedFalse()
    {
        isLanded = false;
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        //if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformLandingTriggerTag platform))
            {
                if (transform.position.y > platform.transform.position.y)
                {
                    isLanded = false;
                    PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
                    PlayerAnimationController.Instance.PlayAnimation(AnimationNames.MED_LANDING_ANIMATION_NAME, false);
                }
            }
        }
    }

    private IEnumerator SetPlayerIdleAnimation()
    {
        yield return new WaitForSeconds(1f);
        PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.IDLE_ANIMATION_NAME, true);
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out Platform platform))
        {
            platform.StartCollisionOutBehaviors();
        }
    }
}
