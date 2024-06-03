using Runtime.Collectibles;
using Runtime.Levels.Platform_Scripts;
using Runtime.Tags;
using Script.Misc;
using UnityEngine;

public class PlayerCollisionController : Singleton<PlayerCollisionController>
{
    public bool isLanded;
    public bool IsLanded => isLanded;

    public PlatformController currentCollidedPlatform;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformController platform))
            {
                var samePlatform = false;
                if (currentCollidedPlatform == null)
                {
                    currentCollidedPlatform = platform;
                }
                else
                {
                    if (currentCollidedPlatform == platform)
                    {
                        samePlatform = true;
                    }
                    else
                    {
                        currentCollidedPlatform = platform;
                    }
                }
                Debug.Log("lumapag na part 1");
                isLanded = true;
                platform.CollisionEnterBehaviour(samePlatform);

                if (col.gameObject.CompareTag("Collapsing"))
                {
                    CollapsingPlatfrom.Instance.CallCollapsingFunction();
                    Debug.Log("collapse platform");
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformController platform))
            {
                isLanded = false;
                platform.CollisionExitBehaviour();
            }
        }
    }

}