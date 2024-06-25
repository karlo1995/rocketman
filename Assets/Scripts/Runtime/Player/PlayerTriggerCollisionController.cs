using System.Collections;
using Runtime.Collectibles;
using Runtime.Levels.Platform_Scripts;
using Runtime.Tags;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTriggerCollisionController : Script.Misc.Singleton<PlayerTriggerCollisionController>
{
    [SerializeField] private GameObject virtualCameraForStage;
    [SerializeField] private GameObject virtualCameraForPlayer;

    private void Awake()
    {
        //TODO: band aid fix need to change please!!
        var currentSceneName = SceneManager.GetActiveScene().name; //added by kylle, it will check the current scene if this function below is needed.
        if (currentSceneName == "Boss Fight 1")
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlatformLandingTriggerTag _))
        {
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.MED_LANDING_ANIMATION_NAME, false);
        }

        if (col.gameObject.TryGetComponent(out PlatformOutOfEdgeTag edgeTag))
        {
            PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
            PlayerWalkController.Instance.SetDelayCauseOfLosingBalance(edgeTag.IsLeftEdge);
        }

        if (col.gameObject.TryGetComponent(out PlatformCeilingTag _))
        {
            //TODO: band aid fix need to change please!!
            var currentSceneName = SceneManager.GetActiveScene().name; //added by kylle, it will check the current scene if this function below is needed.
            if (currentSceneName == "Boss Fight 1")
            {
                return;
            }
            
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
            
            PlayerDragController.Instance.SetCanDrag(false);

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