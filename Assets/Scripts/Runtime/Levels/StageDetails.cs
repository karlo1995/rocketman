using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Levels
{
    [CreateAssetMenu(fileName = "New Stage Data", menuName = "Scriptable Object / Stage Data")]
    public class StageDetails : ScriptableObject
    {
        public Platforms[] Platforms;
        public Sprite BackgroundSprite;
        public bool IsLastStage;
        public bool IsNextStageBoss;
    }

    [System.Serializable]
    public class Platforms
    {
        [Title("LEVEL PLATFORM", horizontalLine: true, bold: true)]
        public PlatformData[] PlatformData;
    }

    [System.Serializable]
    public class PlatformData
    {
        public string PlatformId;
        public Vector2 PlatformPosition;
        public Sprite PlatformSprite;
    }
}
