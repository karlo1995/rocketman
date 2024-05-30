using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Levels
{
    [CreateAssetMenu(fileName = "New Stage Data", menuName = "Scriptable Object / Stage Data")]
    public class StageDetails : ScriptableObject
    {
        public Platforms[] Platforms;
        public PlatformPrefab[] PlatformPrefab;

        public bool IsLastStage;
        public bool IsNextStageBoss;

        public PlatformPrefab GetPlatformPrefabById(string id)
        {
            foreach (var prefab in PlatformPrefab)
            {
                if (prefab.PrefabId.Equals(id))
                {
                    return prefab;
                }
            }

            return null;
        }
    }

    [System.Serializable]
    public class Platforms
    {
        [Title("LEVEL PLATFORM", horizontalLine: true, bold: true)]
        public string PlatformName;
        public PlatformData[] PlatformData;
        public WallData[] WallData;
        public Vector2 CameraPosition;
        public Vector2 CeilingPosition;
        
        public float GetRightmostPlatformDataPosition()
        {
            var farthestXDistance = Mathf.NegativeInfinity;
            
            foreach (var data in PlatformData)
            {
                if (data.PlatformPosition.x > farthestXDistance)
                {
                    farthestXDistance = data.PlatformPosition.x;
                }
            }

            return farthestXDistance + 5f;
        }
        
        public float GetLeftmostPlatformDataPosition()
        {
            var farthestXDistance = Mathf.Infinity;
            
            foreach (var data in PlatformData)
            {
                if (data.PlatformPosition.x < farthestXDistance)
                {
                    farthestXDistance = data.PlatformPosition.x;
                }
            }

            return farthestXDistance - 5f;
        }
        
        public Vector2 GetSpawnPosition()
        {
            var spawnPosition = PlatformData[0].PlatformPosition;
            spawnPosition = new Vector2(spawnPosition.x - 0.5f, spawnPosition.y + 2f);

            return spawnPosition;
        }
        
        public Vector2 GetLaunchPosition()
        {
            var launchPosition = PlatformData[0].PlatformPosition;
            launchPosition = new Vector2(launchPosition.x - 0.5f, launchPosition.y - 2f);
            
            return launchPosition;
        }
    }

    [System.Serializable]
    public class PlatformPrefab
    {
        public bool CreatePool;
        public string PrefabId;
        public GameObject PlatformObject;
    }

    [System.Serializable]
    public class PlatformData
    {
        public string PlatformId;
        public Vector2 PlatformPosition;
        public CollectibleData[] CollectibleData;
    }
    
    [System.Serializable]
    public class CollectibleData
    {
        public string CollectibleId;
        public Vector2 CollectiblePosition;
    }
    
    [System.Serializable]
    public class WallData
    {
        public string WallId;
        public Vector2 WallPosition;
    }
}