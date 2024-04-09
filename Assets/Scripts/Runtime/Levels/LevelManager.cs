using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using DG.Tweening;
using Runtime.Levels;
using Runtime.Manager;
using Runtime.Map_Controller;
using UnityEngine.SceneManagement;

//TODO: Clear current list except first two if we die
public class LevelManager : SerializedMonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level")] [SerializeField] private AudioClip _levelUpSFX;
    [SerializeField] private TextMeshProUGUI _levelText;
    private int _level = 0;

    [Header("Camera")] [SerializeField] private CinemachineVirtualCamera _camera;

    [Header("Platform")] [SerializeField] private SpriteRenderer _levelBG;

    [SerializeField] private int _levelIndex; //Any reason why this is visible in the inspector?

    //[SerializeField] private List<List<PlatformData>> _platformDatas = new List<List<PlatformData>>();
    private int _platformIndex;
    private Vector2 _pastPos;
    private bool _resettedLevel;

    private Platform spawnPointPlatform;

    [ShowInInspector] private Platforms[] _stagePlatforms;
    [ShowInInspector] private StageDetails _currentStageDetails;

    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private int platformPoolBaseAmount;
    private readonly List<Platform> platformPrefabPool = new();

    public int spawnedPlatformCount;
    public int levelCount;

    [ShowInInspector] private Dictionary<int, List<Platform>> spawnedPlatforms = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CreateInitialPlatformPool();
    }

    private void Start()
    {
        SetNewStage();
    }

    private void CreateInitialPlatformPool()
    {
        for (var i = 0; i < platformPoolBaseAmount; i++)
        {
            var platform = Instantiate(platformPrefab, transform);
            platformPrefabPool.Add(platform.GetComponent<Platform>());
            platform.gameObject.SetActive(false);
        }
    }

    private Platform GetPlatform()
    {
        foreach (var platform in platformPrefabPool)
        {
            if (!platform.gameObject.activeInHierarchy && !platform.gameObject.activeSelf)
            {
                return platform;
            }
        }

        //if there are no available platform, create a new one then return
        var newPlatform = Instantiate(platformPrefab, transform);
        platformPrefabPool.Add(newPlatform.GetComponent<Platform>());
        return newPlatform.GetComponent<Platform>();
    }

    public bool IsLastStage()
    {
        return _currentStageDetails.IsLastStage;
    }

    public bool IsNextStageBoss()
    {
        return _currentStageDetails.IsNextStageBoss;
    }

    private void SetNewStage()
    {
        //add here all the logic to reset the stage like reset levelIndex and platformIndex
        //add here all the logic to clear all the current platform instantiated

        var levelDataName = "Stage Data/Stage " + PlayerDataManager.Instance.CurrentStageLevel;
        //var levelDataName = "Stage Data/Test Stage " + PlayerDataManager.Instance.CurrentStageLevel;

        _currentStageDetails = Resources.Load<StageDetails>(levelDataName);
        _stagePlatforms = new Platforms[] { };

        if (!IsLastStage())
        {
            //  _platformDatas = _currentStageDetails.PlatformDatas;
            //_levelBG.sprite = _currentStageDetails.BackgroundSprite;
            _stagePlatforms = _currentStageDetails.Platforms;
            PlayerDataManager.Instance.AddStageLevel();
        }
        else
        {
            //end game
            SceneManager.LoadScene(2);
        }

        _platformIndex = 0;
        _levelIndex = 0;

        SpawnPlatforms();
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
    
    private void SpawnPlatforms()
    {
        //make the past platforms to set active false so they can be use again the platform pool
        StartCoroutine(RemovePlatform(levelCount, 3));

        while (true)
        {
            if (levelCount < _stagePlatforms.Length)
            {
                if (spawnedPlatformCount < _stagePlatforms[levelCount].PlatformData.Length)
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
                        var platformDetails = _stagePlatforms[levelCount].PlatformData[spawnedPlatformCount];
                        var platform = GetPlatform();
                        platform.InitPlatform(platformDetails.PlatformSprite, platformDetails.PlatformPosition, levelCount, spawnedPlatformCount);

                        if (spawnedPlatforms.ContainsKey(levelCount))
                        {
                            spawnedPlatforms[levelCount].Add(platform);
                        }
                        else
                        {
                            spawnedPlatforms.Add(levelCount, new List<Platform> { platform });
                        }
                    }

                    //break the while loop, if there is a new platform to spawned, so the camera will pan to that platform
                    spawnedPlatformCount++;
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
    }

    private void GoToNextStage()
    {
        PlayerNextStageController.Instance.LaunchToNextStage(() =>
        {
            if (true)//PlayerDataManager.Instance.IsRemoveStageClearAds())
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

    private List<Platform> GetCurrentSpawnedPlatform()
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
            //_camera.Follow = GetCurrentSpawnedPlatform()[0].CameraPosition;
            
            Invoke(nameof(UpdateGameSettings), 1f);
        }
    }

    private void UpdateGameSettings()
    {
        WallController.Instance.UpdateWallPosition();
        PlayerDragController.Instance.SetCanDrag(true);
    }

    private void TurnOnRadar()
    {
        //PlayerController.Instance.TurnOnRadar();
    }
    
    public void SpawnNextPlatform(int levelPlatform)
    {
        Invoke(nameof(TurnOnRadar), 2f);

        if (levelPlatform > 0)
        {
            if (levelPlatform.Equals(levelCount))
            {
                SpawnPlatforms();
            }
        }
        else
        {
            if (!spawnedPlatforms.ContainsKey(1))
            {
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
                for (var i = 0; i < spawnedPlatforms[levelCount - 1].Count; i++)
                {
                    if (i >= 2)
                    {
                        spawnedPlatforms[levelCount - 1][i].gameObject.SetActive(false);
                        spawnedPlatforms[levelCount - 1].RemoveAt(i);
                    }
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
            //retain only the first 2 platforms of that level
            for (var i = 0; i < spawnedPlatforms[levelCount].Count; i++)
            {
                if (i >= 2)
                {
                    spawnedPlatforms[levelCount][i].gameObject.SetActive(false);
                    spawnedPlatforms[levelCount].RemoveAt(i);
                }
            }
        }
    }

    public async Task ResetLevel()
    {
        Debug.Log("RESET LEVEL");
        Invoke(nameof(TurnOnRadar), 2f);

        //remove the spawned platforms for the next level
        //use async await to make sure that the advance platforms removed first before spawning again
        await AbortPlatformsOnAdvanceLevels();
        await AbortAdvancePlatformsOnTheSameLevel();

        spawnedPlatformCount = 1;

        var platforms = GetCurrentSpawnedPlatform();
        var spawnPosition = platforms[0].GetSpawnPosition();
        var launchPosition = platforms[0].GetLaunchPosition();

        //move the player controller in the spawn point platform
        PlayerDeathController.Instance.transform.DOMove(launchPosition, 0f).OnComplete(() =>
        {
            PlayerDeathController.Instance.transform.DOMove(spawnPosition, 0.5f).OnComplete(() =>
            {
                for (var i = 0; i < platforms.Count; i++)
                {
                    //reset all the collision of the platforms on the same level
                    platforms[i].TurnOnCollision();

                    //make sure to set active true only the first 2 platform in the level
                    platforms[i].gameObject.SetActive(i < 2);
                    
                    PlayerDragController.Instance.SetCanDrag(true);
                }
            });
        });
    }
}