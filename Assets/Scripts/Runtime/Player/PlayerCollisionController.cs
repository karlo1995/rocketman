using Runtime.Levels.Platform_Scripts;
using Script.Misc;
using UnityEngine;

public class PlayerCollisionController : Singleton<PlayerCollisionController>
{
    public bool isLanded;
    public bool IsLanded => isLanded;

    private PlatformController currentCollidedPlatform;
    public PlatformController CurrentCollidedPlatform => currentCollidedPlatform;
    
    [SerializeField] private Rigidbody2D rigidbody2D;
    
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
                
                isLanded = true;
                platform.CollisionEnterBehaviour(samePlatform);

                //made by kylle
                // if (col.gameObject.CompareTag("Collapsing"))
                // {
                //     CollapsingPlatform.Instance.CallCollapsingFunction();
                //     Debug.Log("collapse platform");
                // }

                //modified by karlo
                if (col.gameObject.TryGetComponent(out CollapsingPlatform collapsingPlatform))
                {
                    collapsingPlatform.CallCollapsingFunction();
                }
                
                if (col.gameObject.TryGetComponent(out WallTag _))
                {
                    // Magnitude of the velocity vector is speed of the object (we will use it for constant speed so object never stop)
                    var speed = rigidbody2D.velocity.magnitude;
 
                    // Reflect params must be normalized so we get new direction
                    var direction = Vector3.Reflect(rigidbody2D.velocity.normalized, col.contacts[0].normal);
 
                    // Like earlier wrote: velocity vector is magnitude (speed) and direction (a new one)
                    rigidbody2D.velocity = direction * speed * 2f;
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
        // if (col.TryGetComponent(out WallTag _))
        // {
        //     // Magnitude of the velocity vector is speed of the object (we will use it for constant speed so object never stop)
        //     var speed = rigidbody2D.velocity.magnitude;
        //
        //     // Reflect params must be normalized so we get new direction
        //     var direction = Vector3.Reflect(rigidbody2D.velocity.normalized, col.GetComponent<Collision2D>().contacts[0].normal);
        //
        //     // Like earlier wrote: velocity vector is magnitude (speed) and direction (a new one)
        //     rigidbody2D.velocity = direction * speed;
        // }
    }
}