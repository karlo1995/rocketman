using System;
using Script.Misc;
using UnityEngine;

public class PlayerDeathController : Singleton<PlayerDeathController>
{
    [SerializeField] private int livesCount;
    [SerializeField] private float failThreshold = -11f;

    private bool isDropped = true;
    public bool IsDropped => isDropped;

    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.y < failThreshold && !isDropped ||
            (PlayerTriggerCollisionController.Instance.IsStageCameraActive() && LevelManager.Instance.IsOutsideLevel()))
        {
            isDropped = true;
            livesCount--;

            if (livesCount <= 0)
            {
            }

            rigidbody2D.velocity = Vector2.zero;
            PlayerDragController.Instance.SetResetToLandingSpot(true);
            PlayerTriggerCollisionController.Instance.ResetCamera();
            LevelManager.Instance.ResetLevel();
        }
    }

    public void ResetDropStatus()
    {
        isDropped = false;
    }
}