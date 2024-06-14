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
    [SerializeField] private int platformPoolBaseAmount;
    [SerializeField] private CinemachineVirtualCamera cmVirtualCamera;

    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject playerDragController;

    [SerializeField] private Rigidbody2D playerControllerRigidBody;
    [SerializeField] private GameObject ceilingPrefab;

    [ShowInInspector] private Platforms[] stagePlatforms;
    [ShowInInspector] private StageDetails currentStageDetails;
    [ShowInInspector] private Dictionary<int, List<PlatformController>> spawnedPlatforms = new();

    [ShowInInspector] private List<CollectibleItem> spawnedCollectibleItem = new();
    [ShowInInspector] private List<CollectibleData> collectedCollectibleData = new();

    private int currentLevelIndex;
    private int platformIndex;
    private Vector2 pastPos;
    private bool resetLevel;

    private PlatformController spawnPointPlatform;
    [ShowInInspector] private readonly Dictionary<string, List<GameObject>> prefabPool = new();
    [ShowInInspector] private int spawnedPlatformCount;
    [ShowInInspector] private int levelCount = 0;

    private readonly List<Transform> cameraPositions = new();
    private readonly List<float> cameraZoomAmounts = new();

    private readonly List<Transform> ceilingPositions = new();
    [ShowInInspector] private readonly List<List<SpriteRenderer>> spawnedWalls = new();


    [ShowInInspector] private Platforms currentPlatform;
    public Platforms CurrentPlatform => currentPlatform;

    [ShowInInspector] private PlatformController platformToRemove;
    private int platformSpawnCounter;
    private int collectibleSpawnCounter;

    private bool needToMoveCamera = true;

    private void Start()
    {
        SetNewStage();
    }

    public void AddCollectibleData(CollectibleData collectibleData)
    {
        if (collectibleData != null)
        {
            collectedCollectibleData.Add(collectibleData);
        }
    }

    public bool IsOutsideLevel()
    {
        var playerPosition = PlayerDeathController.Instance.transform.position;
        var rightMostPositionX = currentPlatform.GetRightmostPlatformDataPosition();
        var leftMostPositionX = currentPlatform.GetLeftmostPlatformDataPosition();

        if (spawnedPlatforms.Count > 1)
        {
            if (spawnedPlatforms.ContainsKey(levelCount))
            {
                foreach (var spawned in spawnedPlatforms[levelCount])
                {
                    if (spawned.transform.position.x > rightMostPositionX)
                    {
                        rightMostPositionX = spawned.transform.position.x;
                    }

                    if (spawned.transform.position.x < leftMostPositionX)
                    {
                        leftMostPositionX = spawned.transform.position.x;
                    }
                }
            }
        }

        if (playerPosition.x > rightMostPositionX)
        {
            return true;
        }

        return playerPosition.x < leftMostPositionX;
    }

    private void CreateStagePrefabs(PlatformPrefab[] platformPrefabArray)
    {
        foreach (var prefabToInstantiate in platformPrefabArray)
        {
            if (prefabToInstantiate.CreatePool)
            {
                for (var i = 0; i < platformPoolBaseAmount; i++)
                {
                    var instantiatedPlatform = Instantiate(prefabToInstantiate.PlatformObject, transform);

                    if (prefabPool.ContainsKey(prefabToInstantiate.PrefabId))
                    {
                        prefabPool[prefabToInstantiate.PrefabId].Add(instantiatedPlatform);
                    }
                    else
                    {
                        prefabPool.Add(prefabToInstantiate.PrefabId, new List<GameObject> { instantiatedPlatform });
                    }

                    instantiatedPlatform.gameObject.SetActive(false);
                }
            }
        }
    }

    private void SpawnCollectible(CollectibleData[] collectibleData)
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

    private PlatformController GetPlatform(PlatformData platformDetails)
    {
        if (prefabPool.ContainsKey(platformDetails.PlatformId))
        {
            if (platformSpawnCounter >= prefabPool[platformDetails.PlatformId].Count)
            {
                platformSpawnCounter = 0;
            }

            if (!prefabPool[platformDetails.PlatformId][platformSpawnCounter].activeInHierarchy)
            {
                var platform = prefabPool[platformDetails.PlatformId][platformSpawnCounter].GetComponent<PlatformController>();

                if (platform != null)
                {
                    if (spawnedPlatforms.ContainsKey(levelCount))
                    {
                        if (!spawnedPlatforms[levelCount].Contains(platform))
                        {
                            if (!platform.gameObject.activeInHierarchy && !platform.gameObject.activeSelf)
                            {
                                platformSpawnCounter++;
                                return platform;
                            }
                        }
                    }
                    else
                    {
                        if (!platform.gameObject.activeInHierarchy && !platform.gameObject.activeSelf)
                        {
                            platformSpawnCounter++;
                            return platform;
                        }
                    }
                }
            }

            /*
            foreach (var platformObjectPool in prefabPool[platformDetails.PlatformId])
            {
                var platform = platformObjectPool.GetComponent<PlatformController>();

                if (platform != null)
                {
                    if (spawnedPlatforms.ContainsKey(levelCount))
                    {
                        if (!spawnedPlatforms[levelCount].Contains(platform))
                        {
                            if (!platform.gameObject.activeInHierarchy && !platform.gameObject.activeSelf)
                            {
                                return platform;
                            }
                        }
                    }
                    else
                    {
                        if (!platform.gameObject.activeInHierarchy && !platform.gameObject.activeSelf)
                        {
                            return platform;
                        }
                    }
                }
            }*/

            //if there are no available platform, create a new one then return
            var newPlatform = Instantiate(prefabPool[platformDetails.PlatformId][0], transform);
            prefabPool[platformDetails.PlatformId].Add(newPlatform);
            platformSpawnCounter++;

            return newPlatform.GetComponent<PlatformController>();
        }

        //if the platform is not instantiated at start, check your scriptable platform details
        Debug.Log("No Platform found in pool for platform " + platformDetails.PlatformId);
        return null;
    }

    private bool IsLastStage()
    {
        return currentStageDetails.IsLastStage;
    }

    public bool IsNextStageBoss()
    {
        return currentStageDetails.IsNextStageBoss;
    }

    private void SetNewStage()
    {
        //add here all the logic to reset the stage like reset levelIndex and platformIndex
        //add here all the logic to clear all the current platform instantiated

        var levelDataName = "Stage Data/Stage " + PlayerDataManager.Instance.CurrentStageLevel;
        //var levelDataName = "Stage Data/Test Stage " + PlayerDataManager.Instance.CurrentStageLevel;

        currentStageDetails = Resources.Load<StageDetails>(levelDataName);
        CreateStagePrefabs(currentStageDetails.PlatformPrefab);

        stagePlatforms = new Platforms[] { };
        currentPlatform = currentStageDetails.Platforms[0];

        if (!IsLastStage())
        {
            stagePlatforms = currentStageDetails.Platforms;
            PlayerDataManager.Instance.AddStageLevel();
        }
        else
        {
            //end game
            SceneManager.LoadScene(2);
        }

        platformIndex = 0;
        currentLevelIndex = 0;

        SpawnCameraPositions();
        SpawnCeilingPositions();
        SpawnWall();

        //spawn the first 2 platform
        SpawnPlatforms();
        SpawnPlatforms();

        StartCoroutine(SpawnPlayer());
    }

    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(2f);

        playerController.SetActive(true);
        playerControllerRigidBody.isKinematic = false;

        var platforms = GetCurrentSpawnedPlatform();
        var spawnPosition = platforms[0].SpawnPosition;
        var launchPosition = platforms[0].LaunchPosition;

        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
        PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);

        //move the player controller in the spawn point platform
        playerController.transform.DOMove(launchPosition.position, 0f).OnComplete(() =>
        {
            playerController.transform.DOMove(spawnPosition.position, 1f).OnComplete(() =>
            {
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.BRAKE_ANIMATION_NAME, false);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);

                StartCoroutine(FallAnimationPlayer());

                //play initial dialogue
                playerDragController.SetActive(true);
                DisplayDialogue.Instance.Open();
            });
        });
    }

    private IEnumerator FallAnimationPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        playerControllerRigidBody.isKinematic = false;
    }

    private IEnumerator RemovePlatform(int secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        if (spawnedPlatforms.ContainsKey(levelCount))
        {
            foreach (var platform in spawnedPlatforms[levelCount])
            {
                platform.gameObject.SetActive(false);
            }

            spawnedPlatforms.Remove(levelCount);
        }
    }

    private void SpawnCameraPositions()
    {
        foreach (var platforms in currentStageDetails.Platforms)
        {
            var cameraTransform = new GameObject
            {
                transform =
                {
                    parent = transform
                }
            };

            cameraTransform.transform.DOLocalMove(platforms.CameraPosition, 0f);
            cameraTransform.gameObject.SetActive(false);
            cameraTransform.name = "Camera Position " + platforms.PlatformName;

            cameraZoomAmounts.Add(platforms.CameraZoomAmount);
            cameraPositions.Add(cameraTransform.transform);
        }
    }

    private void SpawnCeilingPositions()
    {
        foreach (var platforms in currentStageDetails.Platforms)
        {
            var ceilingPosition = Instantiate(ceilingPrefab);
            ceilingPosition.transform.parent = transform;
            ceilingPosition.transform.DOLocalMove(platforms.CeilingPosition, 0f);
            ceilingPosition.gameObject.SetActive(false);
            ceilingPosition.name = "Ceiling Position " + platforms.PlatformName;

            ceilingPositions.Add(ceilingPosition.transform);
        }
    }

    private void SpawnWall()
    {
        foreach (var platforms in currentStageDetails.Platforms)
        {
            List<SpriteRenderer> wallList = new();

            foreach (var wall in platforms.WallData)
            {
                var prefab = currentStageDetails.GetPlatformPrefabById(wall.WallId);

                if (prefab != null)
                {
                    var wallObject = Instantiate(prefab.PlatformObject);
                    wallObject.transform.parent = transform;
                    wallObject.transform.DOLocalMove(wall.WallPosition, 0f);
                    wallObject.gameObject.SetActive(false);
                    wallObject.name = "Wall For " + platforms.PlatformName + " with Wall ID:  " + wall.WallId;
                    wallList.Add(wallObject.GetComponent<SpriteRenderer>());
                }
                else
                {
                    Debug.Log("No Wall Id: " + wall.WallId + " found!");
                }
            }

            spawnedWalls.Add(wallList);
        }
    }

    private void SpawnPlatforms()
    {
        //make the past platforms to set active false so they can be use again the platform pool
        //  StartCoroutine(RemovePlatform(3));
        while (true)
        {
            if (levelCount < stagePlatforms.Length)
            {
                if (spawnedPlatformCount < stagePlatforms[levelCount].PlatformData.Length)
                {
                    //check if the platform is already spawn, to avoid spawning the same platform twice
                    var isAlreadySpawn = false;
                    if (spawnedPlatforms.ContainsKey(levelCount))
                    {
                        foreach (var spawnedPlatform in spawnedPlatforms[levelCount])
                        {
                            if (spawnedPlatform.SpawnedPlatformIndex.Equals(spawnedPlatformCount))
                            {
                                isAlreadySpawn = true;
                                break;
                            }
                        }
                    }

                    //check if it is okay to spawn platform, if yes continue
                    //spawned platform will be added to the spawned platform list
                    if (!isAlreadySpawn)
                    {
                        var platformDetails = stagePlatforms[levelCount].PlatformData[spawnedPlatformCount];
                        var platform = GetPlatform(platformDetails);
                        var willTriggerCameraMove = spawnedPlatformCount == 0;

                        platform.InitPlatform(platformDetails.PlatformPosition, levelCount, spawnedPlatformCount, willTriggerCameraMove);

                        if (spawnedPlatforms.ContainsKey(levelCount))
                        {
                            spawnedPlatforms[levelCount].Add(platform);
                        }
                        else
                        {
                            spawnedPlatforms.Add(levelCount, new List<PlatformController> { platform });
                        }

                        foreach (var collectible in spawnedCollectibleItem)
                        {
                            collectible.SetActive(false, true);
                        }

                        spawnedCollectibleItem.Clear();

                        //it means there are collectibles need to spawn
                        if (platformDetails.CollectibleData.Length > 0)
                        {
                            SpawnCollectible(platformDetails.CollectibleData);
                        }
                    }

                    //break the while loop, if there is a new platform to spawned, so the camera will pan to that platform
                    spawnedPlatformCount++;

                    if (platformToRemove != null)
                    {
                        platformToRemove.RemovePlatform();
                    }

                    if (needToMoveCamera)
                    {
                        needToMoveCamera = false;
                        Invoke(nameof(MoveToNextLevel), 1f);
                    }

                    break;
                }

                spawnedPlatformCount = 0;
                levelCount++;
            }
            else
            {
                //Game Over
                Debug.Log("No more platform left");
                Debug.Log("Go to next level");

                Invoke(nameof(GoToNextStage), 2f);
                break;
            }
        }

        // Debug.Log(Vector2.Distance(PlayerCollisionController.Instance.transform.position,
        //     spawnedPlatforms[0][0].transform.position));
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

    private List<PlatformController> GetCurrentSpawnedPlatform()
    {
        // var levelCountToSpawn = 0;
        // if (spawnedPlatforms.ContainsKey(levelCount))
        // {
        //     if (spawnedPlatforms[levelCount].Count > 1)
        //     {
        //         levelCountToSpawn = levelCount;
        //     }
        //     else
        //     {
        //         levelCountToSpawn = levelCount - 1;
        //     }
        // }

        return spawnedPlatforms.ContainsKey(levelCount) ? spawnedPlatforms[levelCount] : null;
    }

    private void MoveToNextLevel()
    {
        //reset some level related stuff
        foreach (var ceilings in ceilingPositions)
        {
            ceilings.gameObject.SetActive(false);
        }

        if (spawnedPlatforms.ContainsKey(levelCount - 1))
        {
            spawnedPlatforms.Remove(levelCount - 1);

            if (levelCount > 0)
            {
                foreach (var wall in spawnedWalls[levelCount - 1])
                {
                    wall.GetComponent<SpriteRenderer>().DOFade(0f, 0.2f).OnComplete(() =>
                    {
                        wall.gameObject.SetActive(false);
                    }).SetUpdate(true);
                }
            }
        }

        //spawn wall if there is
        if (spawnedWalls[levelCount].Count > 0)
        {
            foreach (var wall in spawnedWalls[levelCount])
            {
                wall.gameObject.SetActive(true);
                wall.GetComponent<SpriteRenderer>().DOFade(1f, 0.2f);
            }
        }

        cmVirtualCamera.Follow = cameraPositions[levelCount];
        cmVirtualCamera.m_Lens.OrthographicSize = cameraZoomAmounts[levelCount];
        currentPlatform = currentStageDetails.Platforms[levelCount];
        //ceilingPositions[levelCount].gameObject.SetActive(true);
    }

    public void SetPlatformToRemove(PlatformController platform)
    {
        platformToRemove = platform;
    }


    public void SpawnNextPlatform(bool willTriggerCameraMove)
    {
        needToMoveCamera = willTriggerCameraMove;
        SpawnPlatforms();


        // if (levelPlatform > 0)
        // {
        //     if (levelPlatform.Equals(levelCount))
        //     {
        //         // platformToRemove = platform;
        //         PlayerTriggerCollisionController.Instance.ResetCamera();
        //         SpawnPlatforms();
        //     }
        // }
        // else
        // {
        //     if (!spawnedPlatforms.ContainsKey(1))
        //     {
        //         //  platformToRemove = platform;
        //         PlayerTriggerCollisionController.Instance.ResetCamera();
        //         SpawnPlatforms();
        //     }
        // }
    }

    private async Task AbortPlatformsOnAdvanceLevels()
    {
        //remove all the spawned platforms since the spawn point platform
        //if there are multiple spawned platforms in the same level, retain only the 2 first platform of that level
        if (spawnedPlatforms.ContainsKey(levelCount - 1))
        {
            if (spawnedPlatforms[levelCount - 1].Count > 1)
            {
                //remove first the multiple spawned platforms
                //retain only the first 2 platforms of that level
                var platformsToRemove = new List<PlatformController>();
                for (var i = 2; i < spawnedPlatforms[levelCount].Count; i++)
                {
                    var platform = spawnedPlatforms[levelCount][i];
                    platform.gameObject.SetActive(false);
                    platformsToRemove.Add(platform);
                }

                foreach (var platform in platformsToRemove)
                {
                    spawnedPlatforms[levelCount].Remove(platform);
                }

                //remove the spawned platform of the next level
                //reduce levelCount to reset again the levels
                if (spawnedPlatforms.ContainsKey(levelCount))
                {
                    foreach (var platform in spawnedPlatforms[levelCount])
                    {
                        platform.gameObject.SetActive(false);
                    }

                    spawnedPlatforms.Remove(levelCount);
                    levelCount -= 1;
                }
            }
        }
    }

    private async Task AbortAdvancePlatformsOnTheSameLevel()
    {
        if (spawnedPlatforms[levelCount].Count > 1)
        {
            //remove the multiple spawned platforms
            //retain only the first 2 platforms of that level'
            var platformsToRemove = new List<PlatformController>();
            for (var i = 2; i < spawnedPlatforms[levelCount].Count; i++)
            {
                var platform = spawnedPlatforms[levelCount][i];
                platform.gameObject.SetActive(false);
                platformsToRemove.Add(platform);
            }

            foreach (var platform in platformsToRemove)
            {
                spawnedPlatforms[levelCount].Remove(platform);
            }
        }
    }

    private void SpawnResetLevel()
    {
        foreach (var platform in spawnedPlatforms)
        {
            foreach (var platformController in platform.Value)
            {
                platformController.RemovePlatform();
            }
        }

        if (spawnedPlatforms.ContainsKey(levelCount - 1))
        {
            levelCount -= 1;
        }

        spawnedPlatforms.Clear();
        spawnedPlatformCount = 0;

        SpawnPlatforms();
        SpawnPlatforms();
    }

    public void ResetLevel()
    {
        platformToRemove = null;

        //remove the spawned platforms for the next level
        //use async await to make sure that the advance platforms removed first before spawning again
        // await AbortPlatformsOnAdvanceLevels();
        // await AbortAdvancePlatformsOnTheSameLevel();

        SpawnResetLevel();
        playerControllerRigidBody.isKinematic = true;

        var spawnPosition = currentPlatform.GetSpawnPosition();
        var launchPosition = currentPlatform.GetLaunchPosition();

        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
        PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);

        //move the player controller in the spawn point platform
        playerController.transform.DOMove(launchPosition, 0f).OnComplete(() =>
        {
            playerController.transform.DOMove(spawnPosition, 0.5f).OnComplete(() =>
            {
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.BRAKE_ANIMATION_NAME, false);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);

                StartCoroutine(FallAnimationPlayer());
            });
        });
    }
}