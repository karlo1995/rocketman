using Runtime.Levels.Platform_Scripts;
using Script.Misc;
using UnityEngine;

public class PlayerCollisionController : Singleton<PlayerCollisionController>
{
    [SerializeField] private GameObject virtualCameraForStage;
    [SerializeField] private GameObject virtualCameraForPlayer;

    public bool isLanded;
    public bool IsLanded => isLanded;

    public PlatformController currentCollidedPlatform;

    private void Awake()
    {
        virtualCameraForStage.SetActive(true);
        virtualCameraForPlayer.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformController platform))
            {
                Debug.Log("Collided!!!");
                
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
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformController platform))
            {
                //if (transform.position.y > platform.transform.position.y)
                {
                    isLanded = false;
                    platform.CollisionExitBehaviour();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //if (col.enabled)
        {
            if (col.gameObject.TryGetComponent(out PlatformLandingTriggerTag _))
            {
                //  if (transform.position.y > platform.transform.position.y)
                {
                    Debug.Log("Trigger enter!!!");

                    
                    isLanded = false;
                    PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
                    PlayerAnimationController.Instance.PlayAnimation(AnimationNames.MED_LANDING_ANIMATION_NAME, false);
                }
            }

            if (col.gameObject.TryGetComponent(out PlatformOutOfEdgeTag _))
            {
                isLanded = false;
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
                //PlayerAnimationController.Instance.PlayAnimation(AnimationNames.LOSING_BALANCE_NAME, false);
                PlayerWalkController.Instance.SetDelayCauseOfLosingBalance();
            }

            if (col.gameObject.TryGetComponent(out PlatformCeilingTag _))
            {
                if (virtualCameraForStage.activeInHierarchy)
                {
                    virtualCameraForStage.SetActive(false);
                    virtualCameraForPlayer.SetActive(true);
                }
                else
                {
                    virtualCameraForStage.SetActive(true);
                    virtualCameraForPlayer.SetActive(false);
                }
            }
        }
    }
}