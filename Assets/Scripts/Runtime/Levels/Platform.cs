using System;
using DG.Tweening;
using Runtime.Ads;
using Runtime.Manager;
using Runtime.Map_Controller;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform launchPosition;
    [SerializeField] private Collider2D[] landingTrigger;
    [SerializeField] private bool canlog;
    [SerializeField] private Transform cameraPosition;
    public Transform CameraPosition => cameraPosition;

    private bool hasBeenTouched;

    private Collider2D colliderAttached;

    private int levelPlatform;

    private int spawnedPlatformIndex;
    public int SpawnedPlatformIndex => spawnedPlatformIndex;

    private void Awake()
    {
        colliderAttached = GetComponent<Collider2D>();
    }
    
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

        ResetPlatform();
        colliderAttached.enabled = true;
        gameObject.SetActive(true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!hasBeenTouched) return;

        colliderAttached.enabled = false;
    }

    public void PlayerDragOut()
    {
        if (!hasBeenTouched) return;

        colliderAttached.enabled = false;

        foreach (Transform child in transform)
        {
//                child.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void TurnOnCollision()
    {
        colliderAttached.enabled = true;
        ResetPlatform();
    }

    public void StartCollisionOutBehaviors()
    {
        if (!hasBeenTouched) return;
        colliderAttached.enabled = false;
    }

    public void StartCollisionBehaviors()
    {
        if (hasBeenTouched) return;

        //if (LastOne)
        // {
        //     Invoke(nameof(GoToNextStage), 2f);
        //     return;
        // }

        //Only spawn next platform if LevelManager exists...We don't need this for bosses
        //if (!isBoss)
        {
            LevelManager.Instance.SpawnNextPlatform(levelPlatform);
        }

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

    private void ResetPlatform()
    {
        hasBeenTouched = false;
    }
}