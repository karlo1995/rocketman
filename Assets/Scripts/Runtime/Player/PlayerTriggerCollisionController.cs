using System.Collections;
using Runtime.Collectibles;
using Runtime.Levels.Platform_Scripts;
using Runtime.Tags;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTriggerCollisionController : Script.Misc.Singleton<PlayerTriggerCollisionController>
{
    [SerializeField] private GameObject virtualCameraForStage;
    [SerializeField] private GameObject virtualCameraForPlayer;

    private void Awake()
    {
        virtualCameraForStage.SetActive(true);
        virtualCameraForPlayer.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlatformLandingTriggerTag _))
        {
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.MED_LANDING_ANIMATION_NAME, false);
        }

        if (col.gameObject.TryGetComponent(out PlatformOutOfEdgeTag _))
        {
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

        if (col.gameObject.TryGetComponent(out CrystalCollectibleTag _))
        {
            var crystal = col.gameObject.GetComponent<CrystalAmount>();
            crystal.CollidedToPlayer();
        }

        if (col.gameObject.TryGetComponent(out WallDeathTag wallDeathTag))
        {
            var collider = wallDeathTag.ParentBoxCollider;
            collider.enabled = false;
            
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FALLING_ANIMATION_NAME, true);

            StartCoroutine(ResetWallCollider(collider));
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

    private IEnumerator ResetWallCollider(BoxCollider2D collider2D)
    {
        yield return new WaitForSeconds(2f);
        collider2D.enabled = true;
    }
}