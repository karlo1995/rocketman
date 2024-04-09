using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class FramerateManager : MonoBehaviour
{
    [SerializeField] [Range(30, 120)] private int _framerate = 60;

    private void Awake()
    {
        Application.targetFrameRate = _framerate;
    }
}
