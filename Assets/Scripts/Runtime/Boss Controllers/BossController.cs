using DG.Tweening;
using UnityEngine;

namespace Runtime.Boss_Controllers
{
    public class BossController : MonoBehaviour
    {
        [SerializeField] private float _bossMovementSpeed = 1f;
        [SerializeField] private PlayerController _player;
        [SerializeField] private Transform _resetSpot;
        [SerializeField] private float _radiusDetection = 5f;

        private Rigidbody2D _bossRigidbody2D;
        private Vector2 _bossMovementDirection;

        private bool _isReset;
        private bool _isLockMovement = true;
        private bool _isGameOver;

        private void Awake()
        {
            if (gameObject.TryGetComponent(out Rigidbody2D rigidbody2D))
            {
                _bossRigidbody2D = rigidbody2D;
            }

            transform.position = _resetSpot.transform.position;
            ResetPosition();
            Invoke(nameof(RemoveReset), 2f);
        }

        private void RemoveReset()
        {
            _isLockMovement = false;
            _isReset = false;
        }

        private void ResetPosition()
        {
            _isReset = true;
        }

        public void PlayerDied(bool p_isGameOver)
        {
            _isLockMovement = p_isGameOver;
            ResetPosition();
        }

        private void Update()
        {
            if (!_isLockMovement)
            {
                if (!_isReset)
                {
                    if (_player.transform)
                    {
                        var direction = (_player.transform.position - transform.position).normalized;
                        _bossMovementDirection = direction;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (!_isLockMovement)
            {
                if (_isReset)
                {
                    transform.DOMove(_resetSpot.position, 0.5f).OnComplete( () => 
                    {
                        _isLockMovement = true;
                        Invoke(nameof(RemoveReset), 2f);
                        
                    });
                }
                else if (_player.transform)
                {
                    _bossRigidbody2D.velocity = _bossMovementDirection * _bossMovementSpeed;

                    //Detect Player Around
                    var collidedObjects = Physics2D.OverlapCircleAll(transform.position, _radiusDetection);
                    foreach (var item in collidedObjects)
                    {
                        if (item.TryGetComponent(out PlayerController playerController))
                        {
                            ResetPosition();
                            playerController.HitByBoss();
                        }
                    }
                }
            }
        }
    }
}