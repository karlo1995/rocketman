using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet_Rotation : MonoBehaviour
{
    private Transform Planet;

    public float rotation = .00001f;

    void Start()
    {
        Planet = GetComponent<Transform>();
    }

    void Update ()
    {
        Planet.Rotate(new Vector3(0, rotation, 0), Space.World);
    }
}
