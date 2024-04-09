using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionController : MonoBehaviour
{
    private Platform collidedPlatform;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlatformTriggerItem platformTriggerItem))
        {
            collidedPlatform = platformTriggerItem.Platform;
            PlayerDragController.Instance.SetCollidedPlatform(platformTriggerItem.Platform);
            platformTriggerItem.Platform.StartCollisionBehaviors();
            
            PlayerAnimationController.Instance.PlayAnimation("mediumlanding", false);
        }
    }
}
