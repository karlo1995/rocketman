using Cinemachine;
using DG.Tweening;
using Script.Misc;
using UnityEngine;

public class PlayerDragController : Singleton<PlayerDragController>
{
    [SerializeField] private SpriteRenderer arrowToolSpriteRenderer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private float dragLimit = 3f;
    [SerializeField] private float forceToAdd = 10f;

    private Camera mainCamera;
    private Platform collidedPlatform;

    private bool isDragging;
    public bool canDrag;

    private Vector2 finalForce;

    private bool isReleased;
    public bool IsReleased => isReleased;

    public void SetCollidedPlatform(Platform collidedPlatform)
    {
        this.collidedPlatform = collidedPlatform;
    }

    private Vector3 mousePosition
    {
        get
        {
            var pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0f;
            return pos;
        }
    }

    public void SetCanDrag()
    {
        canDrag = true;
        isReleased = false;
    }
    
    private void Start()
    {
        mainCamera = Camera.main;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector2.zero);
        lineRenderer.SetPosition(1, Vector2.zero);
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (canDrag)
        {
            if (Input.GetMouseButtonDown(0) && !isDragging)
            {
                DragStart();
            }

            if (isDragging)
            {
                Drag();
            }
        }
    }

    private void FixedUpdate()
    {
        if (canDrag)
        {
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                DragEnd();
            }
        }
    }

    private void DragStart()
    {
        lineRenderer.enabled = true;
        isDragging = true;

        lineRenderer.SetPosition(DragStyleController.Instance.IsAngryBirdController ? 1 : 0, mousePosition);
    }

    private void Drag()
    {
        var startPos = lineRenderer.GetPosition(DragStyleController.Instance.IsAngryBirdController ? 1 : 0);
        var currentPos = mousePosition;
        var distance = currentPos - startPos;

        if (distance.magnitude <= dragLimit)
        {
            lineRenderer.SetPosition(DragStyleController.Instance.IsAngryBirdController ? 0 : 1, currentPos);
        }
        else
        {
            var limitVector = startPos + (distance.normalized * dragLimit);
            lineRenderer.SetPosition(DragStyleController.Instance.IsAngryBirdController ? 0 : 1, limitVector);
        }
        
        var trajectoryCurrent = lineRenderer.GetPosition(1);
        var trajectoryStartPos = lineRenderer.GetPosition(0);
        var trajectoryDistance = trajectoryCurrent - trajectoryStartPos;
        var finalForce = trajectoryDistance * forceToAdd;
        
        // TrajectoryController.Instance.UpdateDots(rigidbody2D.transform.position, -finalForce);
        // TrajectoryController.Instance.Show();
        
        var angle = Mathf.Atan2(trajectoryDistance.y, trajectoryDistance.x) * Mathf.Rad2Deg;
        angle += 500f;
        
        
        arrowToolSpriteRenderer.transform.DORotate(new Vector3(0f, 0f, angle), 0.2f);

        var scale = finalForce.magnitude * 0.1f;
        if (scale < 0.3f)
        {
            scale = 0.3f;
        }

        if (scale > 0.9f)
        {
            scale = 0.9f;
        }
        
        //arrowToolSpriteRenderer.transform.DOMove(new Vector2(trajectoryCurrent.x, trajectoryCurrent.y), 0.1f);
        arrowToolSpriteRenderer.transform.DOScale(new Vector2(scale, scale), 0.3f);
    }

    private void DragEnd()
    {
        canDrag = false;
        isDragging = false;
        lineRenderer.enabled = false;

        TrajectoryController.Instance.Hide();

        var startPos = lineRenderer.GetPosition(0);
        var currentPos = lineRenderer.GetPosition(1);
        var distance = currentPos - startPos;
        finalForce = distance * forceToAdd;

        PlayerAnimationController.Instance.PlayAnimation
            (distance.magnitude >= 2.5f ? AnimationNames.HARD_LAUNCH_ANIMATION_NAME : AnimationNames.SOFT_LAUNCH_ANIMATION_NAME, false);

        Invoke(nameof(PlayThruster), 0.4f);
    }

    private void PlayThruster()
    {
        PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
        
        if (collidedPlatform != null)
        {
            collidedPlatform.PlayerDragOut();
        }
        
        rigidbody2D.AddForce(-finalForce, ForceMode2D.Impulse);
        isReleased = true;
    }
}