using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Cinemachine;
using DG.Tweening;
using Runtime.Boss_Controllers;
using Sirenix.OdinInspector;

internal class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [SerializeField] private bool _isBossRound;

    [ShowIf("_isBossRound")] [SerializeField]
    private BossController _bossController;

    [SerializeField] private Camera _camera;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private GameObject _gameOverOverlay;
    [SerializeField] private float _failThreshold;

    private Vector2 _previousPos;

    [Header("Radius")] [SerializeField] [Range(0, 8)] [Tooltip("When this radius is 2.6, the circle collider radius is 1.9")]
    private float _radius = 4.5f;

    [SerializeField] private CircleCollider2D _launchRadiusCollider;
    [SerializeField] private GameObject _radiusIndicator;

    [Header("Launching")] [SerializeField] private float _launchSpeed = 1;
    private float _launchPower;
    private Vector2 _direction;
    [SerializeField] private GameObject _radarPoint;
    private Vector2 _radarPos;

    private bool _isLaunched = false;
    private bool _isGrounded = true;
    private bool _isHolding = false;
    private bool _isFalling = false;

    [Header("Brakes")] [SerializeField] [Range(0, 1)]
    private float _brakeSpeed = 1;

    private bool _isBraked = false;
    [SerializeField] private TextMeshProUGUI _brakesText;
    [SerializeField] private int _initialBrakes;
    private int _brakes;

    [Header("Fuel")] [SerializeField] [Range(0, 10)]
    private float _fuelSpeed = 1;

    private bool _isFueled = false;
    [SerializeField] private Slider _fuelBar;
    [SerializeField] private Image _fuelFill;
    [SerializeField] private Gradient _fuelFillColor;
    [SerializeField] private float _maxFuel;
    private float _fuel;

    [Header("Lives")] [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private int _initialLives;
    private int _lives;
    private bool _hasFailed;

    [Header("Collision Layers")]
    [SerializeField] private Collider2D _bodyCollision;
    private const string PLAYER_LAYER = "Player";
    private const string PLAYER_GHOST_LAYER = "Player Ghost";
    private const string PLATFORM_LAYER = "Ignore Raycast";

    [Header("Misc")] [SerializeField] private Transform _targetLocationWhenLevelUp;

    private bool _hitWall;
    [SerializeField] private bool _hitEdge;

    private Transform _transform;
    private Rigidbody2D _rb;
    private LineRenderer _lr;

    public Collider2D PlatCol => _platCol;
    [ReadOnly] [ShowInInspector] private Collider2D _platCol; //Collider for only the current platform;


    [SerializeField] private GameObject launcherObjects;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _lr = GetComponent<LineRenderer>();

        _gameOverOverlay.SetActive(false);

        _fuelBar.maxValue = _maxFuel;
        _fuelBar.value = _fuelBar.maxValue;
        _fuel = _maxFuel;

        _lives = _initialLives;
        _livesText.text = _initialLives.ToString();

        _brakes = _initialBrakes;
        _brakesText.text = _initialBrakes.ToString();

        PlayerLevelUp();

        _radarPoint.SetActive(false);
    }

    private void Update()
    {
        if (_camera == null) return;

        //Launching
        if (Input.GetKeyUp(KeyCode.Mouse0) && _isGrounded && _isHolding == true && _launchRadiusCollider.enabled)
        {
            //cinemachineVirtualCamera.enabled = false;
            cinemachineVirtualCamera.Follow = null;
            ClearLineRenderer();
            _launchPower = Mathf.Clamp(_launchPower, 0, _radius);
            _isLaunched = true;
            _isGrounded = false;
            _isHolding = false;
            IndicatorResize(false);
            _radarPoint.SetActive(true);
            _radarPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            _radarPoint.transform.position = _radarPos;
            _radarPoint.SetActive(false);
            launcherObjects.SetActive(false);
        }

        if (Input.GetKey(KeyCode.Mouse0) && !_isGrounded && _fuel > 0 && _rb.velocity != Vector2.zero)
        {
            _isFueled = true;
            _fuel -= Time.deltaTime;
            _fuelBar.value = _fuel;
            _fuelFill.color = _fuelFillColor.Evaluate(_fuel / _maxFuel);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && !_isGrounded || _fuel <= 0)
        {
            _isFueled = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !_isGrounded && _brakes > 0)
        {
            _isBraked = true;
        }
    }

    private void FixedUpdate()
    {
        _isFalling = _rb.velocity.y <= 0;

        if (_isLaunched)
        {
            _isLaunched = false;
            _rb.velocity = _direction * _launchPower * _launchSpeed;
        }

        if (_isFueled)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y + _fuelSpeed);
        }

        if (_isBraked)
        {
            _rb.velocity = new Vector2(_rb.velocity.x / 2 - _brakeSpeed, _rb.velocity.y);
            _isBraked = false;
            _brakes--;
            _brakesText.text = _brakes.ToString();
        }

        if (transform.position.y < _failThreshold)
        {
            //_lives--;
            _hasFailed = true;
            _bodyCollision.enabled = false;
            ScoreManager.Instance.EndCombo();
            _hitWall = false;

            if (_lives <= 0)
            {
                _gameOverOverlay.SetActive(true);
                if (_isBossRound)
                {
                    _bossController.PlayerDied(true);
                }

                return;
            }

            if (_isBossRound)
            {
                _bossController.PlayerDied(false);
            }

            if (_isBossRound)
            {
                _bodyCollision.enabled = true;
            }
            else
            {
                //LevelManager.Instance.ResetLevel();
            }

            _livesText.text = _lives.ToString();
            _rb.velocity = Vector2.zero;
            transform.position = new Vector2(_previousPos.x, _previousPos.y + 0.25f);
            _platCol.enabled = true;
        }
    }
    
    private void OnMouseDrag()
    {
        Debug.Log("Mouse Drag");
        if (!_isGrounded) return;

        if (_lr.GetPosition(0) != _transform.position) _lr.SetPosition(0, _transform.position);

        _isHolding = true;
        _launchPower = Vector2.Distance(_transform.position, _camera.ScreenToWorldPoint(Input.mousePosition));

        _direction = ((Vector2)_camera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)_transform.position).normalized;
        _lr.SetPosition(1, (Vector2)_camera.ScreenToWorldPoint(Input.mousePosition));

        if (_launchPower > _radius)
        {
            _launchPower = _radius;
            _lr.endColor = Color.red;
        }
        else _lr.endColor = Color.green;
    }

    public void HitByBoss()
    {
        if (_isBossRound)
        {
            _bodyCollision.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall") && _isLaunched) _hitWall = true;

        if (collision.gameObject.TryGetComponent(out Platform platform))
        {
            // Debug.Log("is falling: " + _isFalling);
            // var posy = platform.transform.position.y;
            // Debug.Log("pos y: " + posy + 1.1f);
            // var posyone = platform.transform.position.y + 1.1f;
            // Debug.Log("pos y: " + posyone);
            // Debug.Log("player y: " + transform.position.y);


            if (_isFalling && transform.position.y >= platform.transform.position.y)
            {
                //if (!platform.StartPlatformTimer()) return;
                var positionToJump = new Vector2(transform.position.x + 10f, transform.position.y + 10f);
                // cinemachineVirtualCamera.Follow = transform;
                //
                // _camera.transform.DOMove(positionToJump, 2f).OnComplete(() =>
                // {
                //     //cinemachineVirtualCamera.enabled = true;
                //     cinemachineVirtualCamera.Follow = transform;
                // });
               // platform.StartCollisionBehaviors(_isBossRound);
                _platCol = collision.gameObject.GetComponent<Collider2D>();
                _rb.velocity = Vector2.zero;

                if (!_radarPoint.activeInHierarchy && _hasFailed)
                {
                    _radarPoint.SetActive(true);
                    _radarPos += (Vector2)transform.position;
                }

                _isGrounded = true;
                _isBraked = false;
                _isFueled = false;
                _hasFailed = false;

                IndicatorResize(true);
                _launchRadiusCollider.enabled = true;

                if (_isBossRound)
                {
                    TurnOnRadar();
                }
                
                if (!_isBossRound) _failThreshold = transform.position.y - 10;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlatformEdge"))
        {
            _hitEdge = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Player should land if they're falling and close to touching the platform
        if (collision.gameObject.layer == LayerMask.NameToLayer(PLATFORM_LAYER) && _rb.velocity.y <= 0 && _bodyCollision.gameObject.layer != LayerMask.NameToLayer(PLAYER_LAYER))
        {
            _bodyCollision.gameObject.layer = LayerMask.NameToLayer(PLAYER_LAYER);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PLATFORM_LAYER) && _bodyCollision.gameObject.layer != LayerMask.NameToLayer(PLAYER_GHOST_LAYER))
        {
            _bodyCollision.gameObject.layer = LayerMask.NameToLayer(PLAYER_GHOST_LAYER);
        }
    }

    #region OnDrawGizmos

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    #endregion

    public void TurnOnRadar()
    {
        launcherObjects.SetActive(true);
    }

    private void IndicatorResize(bool activate)
    {
        if (activate)
        {
            Vector3 radiusCircle = new Vector3(_radius * 2 / transform.localScale.x, _radius * 2 / transform.localScale.y, _radius * 2 / transform.localScale.z);
            _radiusIndicator.transform.localScale = radiusCircle;
        }
        else _radiusIndicator.transform.localScale = Vector3.zero;
    }

    private void ClearLineRenderer()
    {
        for (int i = 0; i < _lr.positionCount; i++)
        {
            _lr.SetPosition(i, Vector2.zero);
        }
    }

    public void PlayerLevelUp()
    {
        _lives = _initialLives;
        _livesText.text = _lives.ToString();

        _previousPos = transform.position;

        if (ScoreManager.Instance == null) return;

        ScoreManager.Instance.AddScore(100);

        if (_hitWall)
        {
            //ScoreManager.Instance.WallReboundBonus();
            Debug.Log("Rebound Bonus");
            _hitWall = false;
        }

        if (!_hitEdge)
        {
            ScoreManager.Instance.PrecisionBonus();
            Debug.Log("Precision Bonus");
        }

        _hitEdge = false;
    }

    public void DisableBodyCollider()
    {
        _bodyCollision.enabled = false;
    }
    
    public void EnableBodyCollider()
    {
        _bodyCollision.enabled = true;
    }

    public void DisableLaunchCapabilities()
    {
        IndicatorResize(false);
        _launchRadiusCollider.enabled = false;
        _lr.SetPosition(1, transform.position);
        _launchPower = 0;
        _isGrounded = false;
        _isHolding = false;
    }

    public void LaunchToNextStage(Action callback)
    {
        transform.DOMoveY(_targetLocationWhenLevelUp.position.y, 1f).OnComplete(() =>
        {
            _rb.constraints = (RigidbodyConstraints2D)RigidbodyConstraints.FreezePosition; 
            callback?.Invoke();
        });
    }
}