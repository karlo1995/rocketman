using Cinemachine;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cameraToFollow;

    private void Update()
    {
        transform.position = cameraToFollow.transform.position;
    }
}
