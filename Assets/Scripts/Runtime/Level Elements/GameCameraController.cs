using Script.Misc;
using UnityEngine;

public class GameCameraController : Singleton<GameCameraController>
{
    [SerializeField] private GameObject virtualCameraForStage;
    [SerializeField] private GameObject virtualCameraForPlayer;
    
    public void SetHighestCeilingPositionInY()
    {
        virtualCameraForStage.SetActive(true);
        virtualCameraForPlayer.SetActive(false);
    }

    private void Update()
    {
        return;
        //TODO : band aid fix for now , checker if player animation is not null
        if (PlayerAnimationController.Instance != null)
        {
            if (!DisplayDialogue.Instance.IsOpen && !LevelManager.Instance.IsTransitioning)
            {
                if (IsOnCeiling())
                {
                    if (!virtualCameraForPlayer.activeInHierarchy)
                    {
                        virtualCameraForStage.SetActive(false);
                        virtualCameraForPlayer.SetActive(true);
                    }
                }
                else
                {
                    if(!virtualCameraForStage.activeInHierarchy)
                    {

                        virtualCameraForStage.SetActive(true);
                        virtualCameraForPlayer.SetActive(false);
                    }
                }
            }
        }
    }

    private bool IsOnCeiling()
    {
        //check if player transform is outside right or left of the level
        var playerPosition = PlayerAnimationController.Instance.transform.position;
        Debug.Log(virtualCameraForStage.transform.position);
        return false;
        //playerPosition.y > LevelManager.Instance.HighestCeilingPlatform.position.y;
    }
}