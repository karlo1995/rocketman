using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Ads;
using Runtime.Manager;
using Runtime.Map_Controller;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Platform : MonoBehaviour
{
    [SerializeField] private bool isRegularPlatform;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Transform launchPosition;
    [SerializeField] private Transform cameraPosition;
    public Transform CameraPosition => cameraPosition;

    public bool LastOne;
    private bool _hasBeenTouched = false;

    private PlatformTimer _platformTimer;
    private Collider2D _col;
    private SpriteRenderer _spriteRenderer;

    [ShowInInspector] private int levelPlatform;

    [ShowInInspector] private int spawnedPlatformIndex;
    public int SpawnedPlatformIndex => spawnedPlatformIndex;
    

    private void Awake()
    {
        _platformTimer = GetComponent<PlatformTimer>();
        _col = GetComponent<Collider2D>();        
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public Vector2 GetSpawnPosition()
    {
        return spawnPosition.position;
    }
    
    public Vector2 GetLaunchPosition()
    {
        return launchPosition.position;
    }
    
    public void InitPlatform(Sprite platformSprite, Vector2 platformPosition, int levelPlatform, int spawnedPlatformIndex)
    {
        this.levelPlatform = levelPlatform;
        this.spawnedPlatformIndex = spawnedPlatformIndex;
        
        _spriteRenderer.sprite = platformSprite;
        transform.DOMove(platformPosition, 0f);
        
        ResetPlatform();
        _col.enabled = true;
        gameObject.SetActive(true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _platformTimer?.StopTimer();

            if (!_hasBeenTouched) return;

            _col.enabled = false;

            foreach (Transform child in transform)
            {
//                child.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void PlayerDragOut()
    {
        _platformTimer?.StopTimer();

        if (!_hasBeenTouched) return;

        _col.enabled = false;

        foreach (Transform child in transform)
        {
//                child.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void TurnOnCollision()
    { 
        _col.enabled = true;
        ResetPlatform();
    }

    public bool StartPlatformTimer()
    {
        //Activate timer on collision regardless of previous interactions
        //Case scenario for normal platform
        if (_platformTimer == null) return true;
        //Case scenario for timer or hazard
        return _platformTimer.StartTimer();
    }

    public void StartCollisionBehaviors(bool isBoss = false)
    {
        if (_hasBeenTouched) return;

        if (LastOne)
        {
            Invoke(nameof(GoToNextStage), 2f);
            return;
        }

        //Only spawn next platform if LevelManager exists...We don't need this for bosses
        if (!isBoss)
        {
            LevelManager.Instance?.SpawnNextPlatform(levelPlatform);
        }

        _hasBeenTouched = true;
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
        _hasBeenTouched = false;
    }
}