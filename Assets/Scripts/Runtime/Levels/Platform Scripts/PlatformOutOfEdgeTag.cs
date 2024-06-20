using UnityEngine;

namespace Runtime.Levels.Platform_Scripts
{
    public class PlatformOutOfEdgeTag : MonoBehaviour
    {
        [SerializeField] private bool isLeftEdge;
        public bool IsLeftEdge => isLeftEdge;
    }
}
