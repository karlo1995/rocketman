using System;
using Script.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        string currentSceneName = SceneManager.GetActiveScene().name; //added by kylle, it will check the current scene if this function below is needed.
        //the reason why added here this checker scene because the mechanics of stage level logic is not same for this boss fight so need to adjust some function that has character dependencies
        if(currentSceneName != "Boss Fight 1")
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
        
    }

    public void ResetDropStatus()
    {
        isDropped = false;
    }
}