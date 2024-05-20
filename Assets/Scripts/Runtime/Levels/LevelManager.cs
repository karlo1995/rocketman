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
    [SerializeField] private Rigidbody2D playerControllerRigidBody;
    [SerializeField] private GameObject ceilingPrefab;

    [ShowInInspector] private Platforms[] stagePlatforms;
    [ShowInInspector] private StageDetails currentStageDetails;
    [ShowInInspector] private Dictionary<int, List<PlatformController>> spawnedPlatforms = new();
    [ShowInInspector] private List<CollectibleData> spawnedCollectibleData = new();

    private int currentLevelIndex;
    private int platformIndex;
    private Vector2 pastPos;
    private bool resetLevel;

    private PlatformController spawnPointPlatform;
    [ShowInInspector] private readonly Dictionary<string, List<GameObject>> prefabPool = new();

    [ShowInInspector] private int spawnedPlatformCount;
    [ShowInInspector] private int levelCount;

    public List<Transform> cameraPositions = new();
    public List<Transform> ceilingPositions = new();

    public PlatformController platformToRemove;

    private void Start()
    {
        SetNewStage();
    }

    private void CreateStagePrefabs(PlatformPrefab[] platformPrefabArray)
    {
        foreach (var prefabToInstantiate in platformPrefabArray)
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
    
    private void SpawnCollectible(CollectibleData[] collectibleData)
    {
        foreach (var collectible in collectibleData)
        {
            if (!spawnedCollectibleData.Contains(collectible))
            {
                if (prefabPool.ContainsKey(collectible.CollectibleId))
                {
                    var spawnedCollectible = false;
                    foreach (var collectibleObjectPool in prefabPool[collectible.CollectibleId])
                    {
                        if (!collectibleObjectPool.activeInHierarchy)
                        {
                            spawnedCollectible = true;
                            
                            collectibleObjectPool.SetActive(true);
                            collectibleObjectPool.transform.position = collectible.CollectiblePosition;
                            
                            spawnedCollectibleData.Add(collectible);
                            break;
                        }
                    }

                    if (!spawnedCollectible)
                    {
                        //if there are no available collectible, create a new one then return
                        var newCollectible = Instantiate(prefabPool[collectible.CollectibleId][0], transform);
                        prefabPool[collectible.CollectibleId].Add(newCollectible);
                        
                        newCollectible.transform.position = collectible.CollectiblePosition;
                        newCollectible.gameObject.SetActive(true);
                        
                        spawnedCollectibleData.Add(collectible);
                    }
                }
            }
        }
    }

    private PlatformController GetPlatform(PlatformData platformDetails)
    {
        if (prefabPool.ContainsKey(platformDetails.PlatformId))
        {
            foreach (var platformObjectPool in prefabPool[platformDetails.PlatformId])
            {
                var platform = platformObjectPool.GetComponent<PlatformController>();
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

            //if there are no available platform, create a new one then return
            var newPlatform = Instantiate(prefabPool[platformDetails.PlatformId][0], transform);
            prefabPool[platformDetails.PlatformId].Add(newPlatform);
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

        //spawn the first 2 platform
        SpawnPlatforms();
        SpawnPlatforms();

        StartCoroutine(SpawnPlayer());
    }

    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(2f);

        playerController.SetActive(true);
        playerControllerRigidBody.isKinematic = true;

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
            });
        });
    }

    private IEnumerator FallAnimationPlayer()
    {
        yield return new WaitForSeconds(0.5f);
        playerControllerRigidBody.isKinematic = false;
    }

    private IEnumerator RemovePlatform(int level, int secondsDelay)
    {
        yield return new WaitForSeconds(secondsDelay);
        if (spawnedPlatforms.ContainsKey(level - 1))
        {
            foreach (var platform in spawnedPlatforms[level - 1])
            {
                platform.gameObject.SetActive(false);
            }

            spawnedPlatforms.Remove(level - 1);
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

    private void SpawnPlatforms()
    {
        //make the past platforms to set active false so they can be use again the platform pool
        StartCoroutine(RemovePlatform(levelCount, 3));

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
                        platform.InitPlatform(platformDetails.PlatformPosition, levelCount, spawnedPlatformCount);

                        if (spawnedPlatforms.ContainsKey(levelCount))
                        {
                            spawnedPlatforms[levelCount].Add(platform);
                        }
                        else
                        {
                            spawnedPlatforms.Add(levelCount, new List<PlatformController> { platform });
                        }

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
                        platformToRemove.gameObject.SetActive(false);
                    }

                    Invoke(nameof(MoveCamera), 1f);
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
        var levelCountToSpawn = 0;
        if (spawnedPlatforms.ContainsKey(levelCount))
        {
            if (spawnedPlatforms[levelCount].Count > 1)
            {
                levelCountToSpawn = levelCount;
            }
            else
            {
                levelCountToSpawn = levelCount - 1;
            }
        }

        return spawnedPlatforms.ContainsKey(levelCountToSpawn) ? spawnedPlatforms[levelCountToSpawn] : null;
    }

    private void MoveCamera()
    {
        var platforms = GetCurrentSpawnedPlatform();
        if (platforms != null)
        {
            var levelCountToSpawn = 0;
            if (spawnedPlatforms.ContainsKey(levelCount))
            {
                if (spawnedPlatforms[levelCount].Count > 1)
                {
                    levelCountToSpawn = levelCount;
                }
                else
                {
                    levelCountToSpawn = levelCount - 1;
                }
            }

            cmVirtualCamera.Follow = cameraPositions[levelCountToSpawn];
            //ceilingPositions[levelCountToSpawn - 1].gameObject.SetActive(false);
            ceilingPositions[levelCountToSpawn].gameObject.SetActive(true); // GetCurrentSpawnedPlatform()[0].CameraPosition;
            Invoke(nameof(UpdateGameSettings), 1f);
        }
    }

    private void UpdateGameSettings()
    {
        WallController.Instance.UpdateWallPosition();
        // PlayerDragController.Instance.SetCanDrag();
    }

    public void SetPlatformToRemove(PlatformController platform)
    {
        platformToRemove = platform;
    }


    public void SpawnNextPlatform(int levelPlatform)
    {
        //Invoke(nameof(TurnOnRadar), 2f);

        if (levelPlatform > 0)
        {
            if (levelPlatform.Equals(levelCount))
            {
                // platformToRemove = platform;
                SpawnPlatforms();
            }
        }
        else
        {
            if (!spawnedPlatforms.ContainsKey(1))
            {
                //  platformToRemove = platform;
                SpawnPlatforms();
            }
        }
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

    public async Task ResetLevel()
    {
        platformToRemove = null;

        //remove the spawned platforms for the next level
        //use async await to make sure that the advance platforms removed first before spawning again
        await AbortPlatformsOnAdvanceLevels();
        await AbortAdvancePlatformsOnTheSameLevel();

        playerControllerRigidBody.isKinematic = true;

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
                spawnedPlatformCount = 2;

                for (var i = 0; i < platforms.Count; i++)
                {
                    //reset all the collision of the platforms on the same level
                    //make sure to set active true only the first 2 platform in the level
                    platforms[i].gameObject.SetActive(i < 2);
                }

                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.BRAKE_ANIMATION_NAME, false);
                PlayerAnimationController.Instance.PlayThrusterAnimation(false, false);

                StartCoroutine(FallAnimationPlayer());
            });
        });
    }
}