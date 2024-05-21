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
    }

    [System.Serializable]
    public class Platforms
    {
        [Title("LEVEL PLATFORM", horizontalLine: true, bold: true)]
        public string PlatformName;
        public PlatformData[] PlatformData;
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
    }

    [System.Serializable]
    public class PlatformPrefab
    {
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
}