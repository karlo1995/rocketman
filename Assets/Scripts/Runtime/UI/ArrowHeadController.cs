using System;
using UnityEngine;

public class ArrowHeadController : MonoBehaviour
{
    [SerializeField] private Transform rotationToFollow;
    [SerializeField] private Transform positionToFollow;

    private void Update()
    {
        transform.position = positionToFollow.transform.position;
        transform.rotation = rotationToFollow.rotation;
    }
}
