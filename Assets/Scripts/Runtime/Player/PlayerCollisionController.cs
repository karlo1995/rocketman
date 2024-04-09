using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private Platform collidedPlatform;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out PlatformCollisionTag platformTriggerItem))
        {
            collidedPlatform = platformTriggerItem.Platform;
            PlayerDragController.Instance.SetCollidedPlatform(platformTriggerItem.Platform);
            platformTriggerItem.Platform.StartCollisionBehaviors();
            
            PlayerAnimationController.Instance.PlayAnimation("mediumlanding", false);
            PlayerDragController.Instance.SetCanDrag(true);
        }
    }
}
