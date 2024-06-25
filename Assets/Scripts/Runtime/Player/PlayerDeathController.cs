using System;
using Script.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathController : Singleton<PlayerDeathController>
{
    [SerializeField] private int livesCount;
    [SerializeField] private float failThreshold = -11f;

    private bool isDropped = false;
    public bool IsDropped => isDropped;

    private bool isForceDeath;

    private Rigidbody2D rigidbody2D;
    private Camera mainCamera;


    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        //TODO: band aid fix need to change please!!
        string currentSceneName = SceneManager.GetActiveScene().name; //added by kylle, it will check the current scene if this function below is needed.
        if(currentSceneName != "Boss Fight 1")
        {
            if (!DisplayDialogue.Instance.IsOpen && !LevelManager.Instance.IsTransitioning)
            {
                if (isForceDeath ||
                    transform.position.y < failThreshold && !isDropped ||
                    (IsOutsideLevel()))
                {
                    isForceDeath = false;
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
    }

    public void ForceDeath()
    {
        isForceDeath = true;
    }

    public void ResetDropStatus()
    {
        isDropped = false;
    }

    private bool IsOutsideLevel()
    {
        //check if player transform is outside right or left of the level
        var playerPosition = transform.position;
        var cameraX = mainCamera.WorldToScreenPoint(playerPosition).x;

        if (cameraX > Screen.width)
        {
            return true;
        }

        return cameraX < 0f;
    }
}