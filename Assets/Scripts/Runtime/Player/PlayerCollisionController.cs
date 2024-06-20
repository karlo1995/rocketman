using Runtime.Levels.Platform_Scripts;
using Script.Misc;
using UnityEngine;

public class PlayerCollisionController : Singleton<PlayerCollisionController>
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    
    private bool isLanded;
    public bool IsLanded => isLanded;

    private PlatformController currentCollidedPlatform;
    public PlatformController CurrentCollidedPlatform => currentCollidedPlatform;

    private bool isBouncedToWall;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformController platform))
            {
                Debug.Log("Landed player");
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

                isLanded = true;
                
                //reset wall colliders
                if (isBouncedToWall)
                {
                    isBouncedToWall = false;
                    foreach (var wall in LevelManager.Instance.WallToSpawn)
                    {
                        wall.Value.GetComponent<BoxCollider2D>().isTrigger = true;
                    }
                }

                //TODO: band aid fix need to change please!!
                if (LevelManager.Instance != null)
                {
                    if (LevelManager.Instance.LevelCount < 2)
                    {
                        samePlatform = false;
                    }
                }
                else
                {
                    PlayerDragController.Instance.SetCanDrag();
                }

                platform.CollisionEnterBehaviour(samePlatform);

                //modified by karlo
                if (col.gameObject.TryGetComponent(out CollapsingPlatform collapsingPlatform))
                {
                    collapsingPlatform.CallCollapsingFunction();
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out WallTag _))
        {
            // add a bounce effect when colliding to a wall
            isBouncedToWall = true;
            
            rigidbody2D.velocity = Vector3.zero;
            rigidbody2D.AddForce(PlayerAnimationController.Instance.GetPlayerFlipX() ? 
                new Vector2(-150f, 0f) : new Vector2(150f, 0f), ForceMode2D.Force);

            foreach (var wall in LevelManager.Instance.WallToSpawn)
            {
                wall.Value.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}