using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using Runtime.Levels;
using Runtime.Levels.Platform_Scripts;
using Runtime.Manager;
using Runtime.Map_Controller;
using Script.Misc;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private CinemachineVirtualCamera cmVirtualCamera;
    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject playerDragController;
    [SerializeField] private Rigidbody2D playerControllerRigidBody;

    [ShowInInspector] private TestPlatformData[] stagePlatforms;
    [ShowInInspector] private TestStageDetails currentStageDetails;

    [ShowInInspector] private List<CollectibleItem> spawnedCollectibleItem = new();
    [ShowInInspector] private List<TestCollectibleData> collectedCollectibleData = new();
    [ShowInInspector] private List<SpriteRenderer> currentlySpawnedWalls = new();

    [ShowInInspector] private readonly Dictionary<string, List<GameObject>> prefabPool = new();

    [ShowInInspector] private int levelCount;
    public int LevelCount => levelCount;

    [ShowInInspector] private readonly Dictionary<string, Transform> cameraPositions = new();
    [ShowInInspector] private readonly Dictionary<string, SpriteRenderer> wallsToSpawn = new();
    public Dictionary<string, SpriteRenderer> WallToSpawn => wallsToSpawn;

    [ShowInInspector] private PlatformController currentLandingPlatform;
    [ShowInInspector] private PlatformController currentTargetPlatform;

    public PlatformController CurrentLandingPlatform => currentLandingPlatform;
    public PlatformController CurrentTargetPlatform => currentTargetPlatform;

    private Transform highestCeilingPlatform;
    public Transform HighestCeilingPlatform => highestCeilingPlatform;
    
    private int platformSpawnCounter;
    private int collectibleSpawnCounter;

    private bool isTransitioning;
    public bool IsTransitioning => isTransitioning;

    private void Start()
    {
        SetNewStage();
    }

    public void AddCollectibleData(TestCollectibleData collectibleData)
    {
        if (collectibleData != null)
        {
            collectedCollectibleData.Add(collectibleData);
        }
    }

    private void CreateStagePrefabs(StagePrefabs[] stagePrefab)
    {
        foreach (var prefabToInstantiate in stagePrefab)
        {
            if (prefabToInstantiate.PoolBaseAmount > 0)
            {
                for (var i = 0; i < prefabToInstantiate.PoolBaseAmount; i++)
                {
                    var instantiatedPlatform = Instantiate(prefabToInstantiate.PlatformObject, transform);

                    if (prefabPool.ContainsKey(prefabToInstantiate.PrefabSpawnId))
                    {
                        prefabPool[prefabToInstantiate.PrefabSpawnId].Add(instantiatedPlatform);
                    }
                    else
                    {
                        prefabPool.Add(prefabToInstantiate.PrefabSpawnId, new List<GameObject> { instantiatedPlatform });
                    }

                    instantiatedPlatform.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SpawnCollectible(TestCollectibleData[] collectibleData)
    {
        foreach (var collectible in collectibleData)
        {
            var alreadyCollected = false;
            foreach (var item in collectedCollectibleData)
            {
                if (item.Equals(collectible))
                {
                    alreadyCollected = true;
                    break;
                }
            }

            if (!alreadyCollected)
            {
                if (prefabPool.ContainsKey(collectible.CollectibleId))
                {
                    var spawnedCollectible = false;
                    if (collectibleSpawnCounter >= prefabPool[collectible.CollectibleId].Count)
                    {
                        collectibleSpawnCounter = 0;
                    }

                    if (!prefabPool[collectible.CollectibleId][collectibleSpawnCounter].activeInHierarchy)
                    {
                        var collectibleObjectPool = prefabPool[collectible.CollectibleId][collectibleSpawnCounter].gameObject;

                        collectibleObjectPool.transform.position = collectible.CollectiblePosition;

                        if (collectibleObjectPool.TryGetComponent(out CollectibleItem item))
                        {
                            collectibleSpawnCounter++;

                            item.Init(collectible);
                            spawnedCollectibleItem.Add(item);
                        }
                    }
                    else
                    {
                        //if there are no available collectible, create a new one then return
                        var newCollectible = Instantiate(prefabPool[collectible.CollectibleId][0], transform);
                        prefabPool[collectible.CollectibleId].Add(newCollectible);

                        newCollectible.transform.position = collectible.CollectiblePosition;

                        if (newCollectible.TryGetComponent(out CollectibleItem item))
                        {
                            collectibleSpawnCounter++;

                            item.Init(collectible);
                            spawnedCollectibleItem.Add(item);
                        }
                    }
                }
            }
        }
    }

    private PlatformController GetPlatform(TestPlatformData platformDetails)
    {
        if (prefabPool.ContainsKey(platformDetails.PlatformPrefabId))
        {
            if (platformSpawnCounter >= prefabPool[platformDetails.PlatformPrefabId].Count)
            {
                platformSpawnCounter = 0;
            }

            if (!prefabPool[platformDetails.PlatformPrefabId][platformSpawnCounter].activeInHierarchy)
            {
                var platform = prefabPool[platformDetails.PlatformPrefabId][platformSpawnCounter].GetComponent<PlatformController>();

                if (platform != null)
                {
                    if (!platform.gameObject.activeInHierarchy && !platform.gameObject.activeSelf)
                    {
                        platformSpawnCounter++;
                        return platform;
                    }
                }
            }

            //if there are no available platform, create a new one then return
            var newPlatform = Instantiate(prefabPool[platformDetails.PlatformPrefabId][0], transform);
            prefabPool[platformDetails.PlatformPrefabId].Add(newPlatform);
            platformSpawnCounter++;

            return newPlatform.GetComponent<PlatformController>();
        }

        //if the platform is not instantiated at start, check your scriptable platform details
        Debug.Log("No Platform found in pool for platform " + platformDetails.PlatformPrefabId);
        return null;
    }

    private bool IsLastStage()
    {
        return false; //currentStageDetails.IsLastStage;
    }

    public bool IsNextStageBoss()
    {
        return false; //currentStageDetails.IsNextStageBoss;
    }

    private void SetNewStage()
    {
        //add here all the logic to reset the stage like reset levelIndex and platformIndex
        //add here all the logic to clear all the current platform instantiated

        var levelDataName = "Stage Data/Test Stage " + PlayerDataManager.Instance.CurrentStageLevel;
        //var levelDataName = "Stage Data/Test Stage " + PlayerDataManager.Instance.CurrentStageLevel;

        currentStageDetails = Resources.Load<TestStageDetails>(levelDataName);
        CreateStagePrefabs(currentStageDetails.StagePrefabs);

        stagePlatforms = new TestPlatformData[] { };

        if (!IsLastStage())
        {
            stagePlatforms = currentStageDetails.PlatformsData;
            PlayerDataManager.Instance.AddStageLevel();
        }
        else
        {
            //end game
            SceneManager.LoadScene(2);
        }

        //spawn other stage relative things
        SpawnCameraPositions();
        SpawnWall();

        //Open Dialogue
        DisplayDialogue.Instance.Open();
    }

    public void SpawnPlayerAtTheStartOfTheGame()
    {
        //spawn player
        StartCoroutine(SpawnPlayer());
    }

    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(0.3f);

        playerController.SetActive(true);
        playerControllerRigidBody.isKinematic = false;

        var platforms = currentStageDetails.PlatformsData[0];
        var spawnPosition = platforms.GetSpawnPosition();
        var launchPosition = platforms.GetLaunchPosition();

        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
        PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);

        //play initial dialogue
        playerDragController.SetActive(true);

        //spawn the first platform
        SpawnPlatforms();

        //move the player controller in the spawn point platform
        playerController.transform.DOMove(launchPosition, 0f).OnComplete(() =>
        {
            playerController.transform.DOMove(spawnPosition, 1f).OnComplete(() =>
            {
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.BRAKE_ANIMATION_NAME, false);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);

                StartCoroutine(FallAnimationPlayer());
            });
        });
    }

    private IEnumerator FallAnimationPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        playerControllerRigidBody.isKinematic = false;
        playerControllerRigidBody.velocity = Vector2.zero;
    }

    private void SpawnCameraPositions()
    {
        foreach (var cameraData in currentStageDetails.CameraData)
        {
            var cameraTransform = new GameObject
            {
                transform =
                {
                    parent = transform
                }
            };

            cameraTransform.transform.DOLocalMove(cameraData.CameraPosition, 0f);
            cameraTransform.gameObject.SetActive(false);
            cameraTransform.name = "Camera Position " + cameraData.CameraSpawnId;

            cameraPositions.Add(cameraData.CameraSpawnId, cameraTransform.transform);
        }

        //set initial camera position
        cmVirtualCamera.Follow = cameraPositions[currentStageDetails.PlatformsData[0].CameraIdToUse];
        cmVirtualCamera.m_Lens.OrthographicSize = currentStageDetails.PlatformsData[0].CameraZoomValue;
    }

    private void SpawnWall()
    {
        foreach (var wall in currentStageDetails.WallsData)
        {
            var prefab = currentStageDetails.GetPlatformPrefabById(wall.WallPrefabId);

            if (prefab != null)
            {
                var wallObject = Instantiate(prefab.PlatformObject);
                wallObject.transform.parent = transform;
                wallObject.transform.DOLocalMove(wall.WallPosition, 0f);
                wallObject.gameObject.SetActive(false);
                wallObject.name = "Wall ID: " + wall.WallSpawnId;
                wallsToSpawn.Add(wall.WallSpawnId, wallObject.GetComponent<SpriteRenderer>());
            }
            else
            {
                Debug.Log("No Wall Id: " + wall.WallSpawnId + " found!");
            }
        }
    }

    private void SpawnPlatforms()
    {
        while (true)
        {
            if (levelCount < stagePlatforms.Length)
            {
                var platformDetails = stagePlatforms[levelCount];
                var platform = GetPlatform(platformDetails);

                platform.InitPlatform(platformDetails);

                foreach (var collectible in spawnedCollectibleItem)
                {
                    collectible.SetActive(false, true);
                }

                spawnedCollectibleItem.Clear();

                //spawn collectible if needed 
                if (platformDetails.CollectibleDataToSpawn.Length > 0)
                {
                    SpawnCollectible(platformDetails.CollectibleDataToSpawn);
                }

                //spawn wall if needed
                //remove spawned walls if not needed anymore
                foreach (var (key, wall) in wallsToSpawn)
                {
                    var isEnable = false;

                    if (platformDetails.WallIdToSpawn.Length > 0)
                    {
                        foreach (var wallId in platformDetails.WallIdToSpawn)
                        {
                            if (wallId.Equals(key))
                            {
                                isEnable = true;

                                if (!wall.gameObject.activeInHierarchy)
                                {
                                    wallsToSpawn[wallId].gameObject.SetActive(true);
                                    wallsToSpawn[wallId].DOFade(1f, 0.2f).SetUpdate(true);
                                    break;
                                }
                            }
                        }
                    }

                    if (!isEnable)
                    {
                        if (wall.gameObject.activeInHierarchy)
                        {
                            wall.DOFade(0f, 0.2f).OnComplete(() => { wall.gameObject.SetActive(false); }).SetUpdate(true);
                        }
                    }
                }


                //check if need to change camera and check zoom
                if (currentLandingPlatform != null)
                {
                    if (!currentLandingPlatform.GetCameraIdToUse().Equals(platformDetails.CameraIdToUse))
                    {
                        //set is transitioning true to tell to others that camera is currently changing its follow
                        isTransitioning = true;
                        cmVirtualCamera.Follow = cameraPositions[platformDetails.CameraIdToUse];
                        
                        GameCameraController.Instance.SetHighestCeilingPositionInY();

                        //reset transitioning status
                        StartCoroutine(ResetTransitioningStatus());
                    }
                }

                //set current target platform 
                currentTargetPlatform = platform;

                //get highest ceiling platform
                if (currentLandingPlatform != null && currentTargetPlatform != null)
                {
                    if (currentTargetPlatform.transform.position.y > currentLandingPlatform.transform.position.y)
                    {
                        highestCeilingPlatform = currentTargetPlatform.transform;
                    }
                    else
                    {
                        highestCeilingPlatform = currentLandingPlatform.transform;
                    }
                }

                //adjust camera zoom 
                cmVirtualCamera.m_Lens.OrthographicSize = platformDetails.CameraZoomValue;

                //increment level count
                levelCount++;

                //flip player controller based on where the next platform is
                StartCoroutine(FlipPlayerToTargetPlatform());
                break;
            }

            //Game Over
            Debug.Log("No more platform left");
            Debug.Log("Go to next level");

            Invoke(nameof(GoToNextStage), 2f);
            break;
        }
    }

    private IEnumerator ResetTransitioningStatus()
    {
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }

    private IEnumerator FlipPlayerToTargetPlatform()
    {
        yield return new WaitForSeconds(0.2f);

        //flip player controller based on where the next platform is
        if (PlayerAnimationController.Instance != null)
        {
            PlayerAnimationController.Instance.ByPositionPlayerFlipX(currentTargetPlatform.transform.position);
        }
    }

    private void GoToNextStage()
    {
        PlayerNextStageController.Instance.LaunchToNextStage(() =>
        {
            if (true) //PlayerDataManager.Instance.IsRemoveStageClearAds())
            {
                MapController.Instance.OpenMap();
            }
            else
            {
#if UNITY_EDITOR
                MapController.Instance.OpenMap();
#elif UNITY_ANDROID || UNITY_IOS
              //  InterstitialAdsController.Instance.LoadAd(MapController.Instance.OpenMap);
                                MapController.Instance.OpenMap();

#endif
            }
        });
    }

    public void SpawnNextPlatform(PlatformController platformController)
    {
        //remove last platform
        if (currentLandingPlatform != null && levelCount > 1)
        {
            currentLandingPlatform.RemovePlatform();
        }

        currentLandingPlatform = platformController;

        SpawnPlatforms();
    }

    public void ResetLevel()
    {
        StartCoroutine(ResetLevelCoroutine());
    }

    private IEnumerator ResetLevelCoroutine()
    {
        playerControllerRigidBody.isKinematic = true;
        var spawnPosition = currentLandingPlatform.GetSpawnPosition();
        var launchPosition = currentLandingPlatform.GetLaunchPosition();

        //check if reset platform is collapsing platform
        if (currentLandingPlatform.IsCollapsingPlatform)
        {
            currentLandingPlatform.CollapsingPlatform.ResetCollapsingPlatform();

            //add position x and y because collapsing platform 
            spawnPosition.y += 2f;
            launchPosition.y += 1f;
        }

        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
        PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);

        //check if reset platform is collapsing platform
        if (currentLandingPlatform.IsCollapsingPlatform)
        {
            while (!currentLandingPlatform.CollapsingPlatform.gameObject.activeInHierarchy)
            {
                yield return null;
            }
        }
        
        //move the player controller in the spawn point platform
        playerController.transform.DOMove(launchPosition, 0f).OnComplete(() =>
        {
            //flip player controller based on where the next platform is
            PlayerAnimationController.Instance.ByPositionPlayerFlipX(currentTargetPlatform.transform.position);
            
            playerController.transform.DOMove(spawnPosition, 0.5f).OnComplete(() =>
            {
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.BRAKE_ANIMATION_NAME, false);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);
                StartCoroutine(FallAnimationPlayer());
            });
        });
    }
}