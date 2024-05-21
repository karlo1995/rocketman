using System.Collections;
using DG.Tweening;
using Runtime.Ads;
using Runtime.Manager;
using Runtime.Map_Controller;
using UnityEngine;

public class OldPlatform : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform launchPosition;
    [SerializeField] private Collider2D[] landingTrigger;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform midLandingPosition;
    [SerializeField] private Transform midLandingWalkingPosition;

    private bool hasBeenTouched;
    private int levelPlatform;
    private int spawnedPlatformIndex;
    public int SpawnedPlatformIndex => spawnedPlatformIndex;
    
    public Vector2 GetSpawnPosition()
    {
        return spawnPosition.position;
    }

    public Vector2 GetLaunchPosition()
    {
        return launchPosition.position;
    }

    public void InitPlatform(Vector2 platformPosition, int levelPlatform, int spawnedPlatformIndex)
    {
        this.levelPlatform = levelPlatform;
        this.spawnedPlatformIndex = spawnedPlatformIndex;

        transform.DOMove(platformPosition, 0f);
        gameObject.SetActive(true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerCollisionController playerCollisionController))
        {
            //playerCollisionController.SetIsLandedFalse();
           // LevelManager.Instance.SetPlatformToRemove(this);
        }
    }
    
    public void StartCollisionBehaviors()
    {
        var needToWalkToMid = CheckIfNeedToWalkToMid();
        if (!needToWalkToMid)
        {
            SpawnNextPlatform();
        }
    }

    private IEnumerator StartSpawningPlatformCountdown()
    {
        yield return new WaitForSeconds(1f);
        
        //LevelManager.Instance.SpawnNextPlatform(levelPlatform, this);
        hasBeenTouched = true;
    }

    private void GoToNextStage()
    {
        PlayerNextStageController.Instance.LaunchToNextStage(() =>
        {
            if (PlayerDataManager.Instance.IsRemoveStageClearAds())
            {
                MapController.Instance.OpenMap();
            }
            else
            {
                InterstitialAdsController.Instance.LoadAd(MapController.Instance.OpenMap);
            }
        });
    }

    private bool CheckIfNeedToWalkToMid()
    {
        var distanceX = Vector2.Distance(midLandingPosition.position, PlayerWalkController.Instance.transform.position);
        if (distanceX > 1f)
        {
            PlayerWalkController.Instance.MoveTowardMid(midLandingWalkingPosition, distanceX, SpawnNextPlatform);
            return true;
        }

        return false;
    }

    private void SpawnNextPlatform()
    {
        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.IDLE_ANIMATION_NAME, true);
        PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
        StartCoroutine(StartSpawningPlatformCountdown());
            
        PlayerDragController.Instance.SetCanDrag();
    }
}