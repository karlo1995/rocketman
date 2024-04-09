using Cinemachine;
using Script.Misc;
using UnityEngine;

public class PlayerDragController : Singleton<PlayerDragController>
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private float dragLimit = 3f;
    [SerializeField] private float forceToAdd = 10f;

    private Camera mainCamera;
    private Platform collidedPlatform;

    private bool isDragging;
    public bool canDrag;
    private bool isReleased;

    private Vector2 finalForce;

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

    public void SetCanDrag(bool canDrag)
    {
        this.canDrag = canDrag;
        if (this.canDrag)
        {
            PlayerAnimationController.Instance.PlayAnimation("idle", true);
            // vcCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.8f;
            // vcCamera.m_Lens.OrthographicSize = 5f;

            isReleased = false;
        }
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
            //lineRenderer.SetPosition(0, limitVector);
            lineRenderer.SetPosition(DragStyleController.Instance.IsAngryBirdController ? 0 : 1, limitVector);
        }
        
        var trajectoryCurrent = lineRenderer.GetPosition(1);
        var trajectoryStartPos = lineRenderer.GetPosition(0);
        var trajectoryDistance = trajectoryCurrent - trajectoryStartPos;
        var finalForce = trajectoryDistance * forceToAdd;

        TrajectoryController.Instance.UpdateDots(rigidbody2D.transform.position, -finalForce);
        TrajectoryController.Instance.Show();
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
        
        //isReleased = true;
        PlayerAnimationController.Instance.PlayAnimation("softlaunch2", false);
        Invoke(nameof(PlayThruster), 1f);

        // rigidbody2D.AddForce(-finalForce, ForceMode2D.Impulse);
    }

    private void PlayThruster()
    {
        PlayerAnimationController.Instance.PlayAnimation("floating", true);
        
        if (collidedPlatform != null)
        {
            collidedPlatform.PlayerDragOut();
        }
        
        rigidbody2D.AddForce(-finalForce, ForceMode2D.Impulse);
        isReleased = true;
    }
}