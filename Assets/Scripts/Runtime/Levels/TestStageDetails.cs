using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Levels
{
    [CreateAssetMenu(fileName = "New Stage Data", menuName = "Scriptable Object / Test Stage Data")]
    public class TestStageDetails : ScriptableObject
    {
        [SerializeField] private TestPlatformData[] platformsData;
        [SerializeField] private StagePrefabs[] stagePrefabs;
        [SerializeField] private TestWallData[] wallsData;
        [SerializeField] private TestCameraData[] cameraData;

        public TestPlatformData[] PlatformsData => platformsData;
        public StagePrefabs[] StagePrefabs => stagePrefabs;
        public TestWallData[] WallsData => wallsData;
        public TestCameraData[] CameraData => cameraData;
        
        
        public StagePrefabs GetPlatformPrefabById(string id)
        {
            foreach (var prefab in stagePrefabs)
            {
                if (prefab.PrefabSpawnId.Equals(id))
                {
                    return prefab;
                }
            }

            return null;
        }
    }
    
    [System.Serializable]
    public class StagePrefabs
    {
        [BoxGroup("$Combined", centerLabel: true)]
        public string Combined => "STAGE PREFAB: " + prefabSpawnId;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private float poolBaseAmount;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private string prefabSpawnId;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private GameObject platformObject;
        
        public float PoolBaseAmount => poolBaseAmount;
        public string PrefabSpawnId => prefabSpawnId;
        public GameObject PlatformObject => platformObject;
    }
    
    [System.Serializable]
    public class TestPlatformData
    { 
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private int platformCount;
        public string Combined => "PLATFORM" + " #" + platformCount;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private string platformPrefabId;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private Vector2 platformPosition;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private string cameraIdToUse;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private float cameraZoomValue;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private string[] wallIdToSpawn;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private TestCollectibleData[] collectibleDataToSpawn;
        
        [BoxGroup("$Combined", centerLabel: true)]
        [SerializeField] private bool shouldRemoveColliderUponExit;
        
        public int PlatformCount => platformCount;
        public string PlatformPrefabId => platformPrefabId;
        public Vector2 PlatformPosition => platformPosition;
        public TestCollectibleData[] CollectibleDataToSpawn => collectibleDataToSpawn;
        public string[] WallIdToSpawn => wallIdToSpawn;
        public string CameraIdToUse => cameraIdToUse;
        public float CameraZoomValue => cameraZoomValue;
        public bool ShouldRemoveColliderUponExit => shouldRemoveColliderUponExit;

        public Vector2 GetSpawnPosition()
        {
            var spawnPosition = platformPosition;
            spawnPosition = new Vector2(spawnPosition.x - 0.3f, spawnPosition.y + 1f);

            return spawnPosition;
        }

        public Vector2 GetLaunchPosition()
        {
            var launchPosition = platformPosition;
            launchPosition = new Vector2(launchPosition.x - 0.3f, launchPosition.y - 2f);

            return launchPosition;
        }
    }

    [System.Serializable]
    public class TestCollectibleData
    {
        [SerializeField] private string collectibleId;
        [SerializeField] private Vector2 collectiblePosition;

        public string CollectibleId => collectibleId;
        public Vector2 CollectiblePosition => collectiblePosition;
    }

    [System.Serializable]
    public class TestWallData
    {
        [SerializeField] private string wallSpawnId;
        [SerializeField] private string wallPrefabId;
        [SerializeField] private Vector2 wallPosition;

        public string WallSpawnId => wallSpawnId;
        public string WallPrefabId => wallPrefabId;
        public Vector2 WallPosition => wallPosition;
    }

    [System.Serializable]
    public class TestCameraData
    {
        [SerializeField] private string cameraSpawnId;
        [SerializeField] private Vector2 cameraPosition;

        public string CameraSpawnId => cameraSpawnId;
        public Vector2 CameraPosition => cameraPosition;
    }
}