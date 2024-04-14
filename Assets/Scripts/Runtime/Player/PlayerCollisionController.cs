using System.Collections;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out Platform platform))
        {
            if (transform.position.y > platform.transform.position.y)
            {
                PlayerDragController.Instance.SetCollidedPlatform(platform);
                platform.StartCollisionBehaviors();

                StartCoroutine(SetPlayerIdleAnimation());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlatformLandingTriggerTag platform))
        {
            if (transform.position.y > platform.transform.position.y)
            {
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.MED_LANDING_ANIMATION_NAME, false);
            }
        }
    }

    private IEnumerator SetPlayerIdleAnimation()
    {
        yield return new WaitForSeconds(1f);
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
