using System;
using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    public static PlayerDeathController Instance;
    [SerializeField] private int livesCount;
    [SerializeField] private float failThreshold = -5f;

    private Rigidbody2D rigidbody2D;

    private void Awake()
    {
        Instance = this;
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (transform.position.y < failThreshold)
        {
           // PlayerDragController.Instance.SetCanDrag(false);
            livesCount--;
            //_hasFailed = true;
            //_bodyCollision.enabled = false;
            // ScoreManager.Instance.EndCombo();
            //_hitWall = false;

            if (livesCount <= 0)
            {
                //_gameOverOverlay.SetActive(true);
                // if (_isBossRound)
                // {
                //     _bossController.PlayerDied(true);
                // }

               // return;
            }

            // if (_isBossRound)
            // {
            //     _bossController.PlayerDied(false);
            // }
            //
            // if (_isBossRound)
            // {
            //     _bodyCollision.enabled = true;
            // }
          //  else
            {
                //LevelManager.Instance.ResetLevel();
            }

            // _livesText.text = _lives.ToString();
            rigidbody2D.velocity = Vector2.zero;
            //transform.position = new Vector2(_previousPos.x, _previousPos.y + 0.25f);
            //_platCol.enabled = true;
        }
    }
}