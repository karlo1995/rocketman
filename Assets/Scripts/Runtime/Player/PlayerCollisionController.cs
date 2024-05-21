using Runtime.Collectibles;
using Runtime.Levels.Platform_Scripts;
using Runtime.Tags;
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
                isLanded = false;
                platform.CollisionExitBehaviour();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlatformLandingTriggerTag _))
        {
            isLanded = false;
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.MED_LANDING_ANIMATION_NAME, false);
        }

        if (col.gameObject.TryGetComponent(out PlatformOutOfEdgeTag _))
        {
            isLanded = false;
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerWalkController.Instance.SetDelayCauseOfLosingBalance(1f);
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

        if (col.gameObject.TryGetComponent(out FuelCollectibleTag _))
        {
            var fuelCollectible = col.gameObject.GetComponent<FuelAmount>();
            
            FuelController.Instance.AddFuel(fuelCollectible.AdditionalFuelAmount);
            fuelCollectible.CollidedToPlayer();
        }
    }

    public bool IsStageCameraActive()
    {
        return virtualCameraForStage.activeInHierarchy;
    }

    public void ResetCamera()
    {
        virtualCameraForStage.SetActive(true);
        virtualCameraForPlayer.SetActive(false);
    }
}