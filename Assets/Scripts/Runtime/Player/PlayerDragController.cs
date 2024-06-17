using System.Collections;
using System.Collections.Generic;
using Runtime.Levels.Platform_Scripts;
using Script.Misc;
using UnityEngine;

public class PlayerDragController : Singleton<PlayerDragController>
{
    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private float dragLimit = 1f;
    [SerializeField] private float hideDragLimit = 0.3f;
    [SerializeField] private float forceToAdd = 15f;

    [SerializeField] private DragVisualController dragVisualController;

    private Camera mainCamera;

    private bool isDragging;
    private bool canDrag;

    private Vector2 finalForce;

    private bool isReleased;
    public bool IsReleased => isReleased;

    private bool isResetToLandingSpot;
    public bool IsResetToLandingSpot => isResetToLandingSpot;

    private readonly List<PlatformController> platformControllerTriggerNeedsToCheck = new();

    public void SetResetToLandingSpot(bool reset)
    {
        isResetToLandingSpot = reset;
    }

    public void AddPlatformControllerToCheck(PlatformController platformController)
    {
        platformControllerTriggerNeedsToCheck.Add(platformController);
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
        StartCoroutine(SetActiveDrag());
    }

    private IEnumerator SetActiveDrag()
    {
        yield return new WaitForSeconds(1f);
        SetResetToLandingSpot(false);

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

        SetResetToLandingSpot(false);

        canDrag = false;
        isReleased = false;
    }

    private void Update()
    {
        if (!DisplayDialogue.Instance.IsOpen && !LevelManager.Instance.IsTransitioning)
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

        if (isReleased)
        {
            if (platformControllerTriggerNeedsToCheck.Count > 0)
            {
                if (rigidbody2D.velocity.magnitude < 1f)
                {
                    foreach (var platform in platformControllerTriggerNeedsToCheck)
                    {
                        platform.OpenAnimationTriggers();
                    }

                    platformControllerTriggerNeedsToCheck.Clear();
                }
            }
        }
    }

    private void DragStart()
    {
        lineRenderer.enabled = true;
        isDragging = true;

        lineRenderer.SetPosition(1, mousePosition);
        dragVisualController.SetActiveRadar(true);
    }

    public void StopDrag()
    {
        isDragging = false;
        lineRenderer.enabled = false;
    }

    private void Drag()
    {
        var startPos = lineRenderer.GetPosition(1);
        var currentPos = mousePosition;

        var distance = currentPos - startPos;
        if (distance.magnitude <= hideDragLimit)
        {
            var limitVector = startPos + distance.normalized * hideDragLimit;
            lineRenderer.SetPosition(0, limitVector);
        }
        else if (distance.magnitude <= dragLimit)
        {
            lineRenderer.SetPosition(0, currentPos);
        }
        else
        {
            var limitVector = startPos + distance.normalized * dragLimit;
            lineRenderer.SetPosition(0, limitVector);
        }

        var trajectoryCurrent = lineRenderer.GetPosition(1);
        var trajectoryStartPos = lineRenderer.GetPosition(0);
        var trajectoryDistance = trajectoryCurrent - trajectoryStartPos;
        var finalForce = trajectoryDistance * forceToAdd;

        ///check if dragging is outside the line bounds angle
        var angle = Mathf.Atan2(trajectoryDistance.y, trajectoryDistance.x) * Mathf.Rad2Deg;
        if (angle > 0f && angle != 0f)
        {
            isDragging = false;
            lineRenderer.enabled = false;
            dragVisualController.SetActiveRadar(false);
            FuelController.Instance.ResetPredictionFuel();
            return;
        }

        var dragDistance = Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
        FuelController.Instance.UsePredictionFuel(dragDistance);

        dragVisualController.SetRadarDirectionAndArrow(finalForce, trajectoryDistance, angle);
    }

    private void DragEnd()
    {
        return;

        isDragging = false;
        lineRenderer.enabled = false;

        dragVisualController.SetActiveRadar(false);

        var startPos = lineRenderer.GetPosition(0);
        var currentPos = lineRenderer.GetPosition(1);
        var distance = currentPos - startPos;
        finalForce = distance * forceToAdd;

        if (finalForce.magnitude > 0f)
        {
            canDrag = false;

            if (distance.magnitude >= 2.5f)
            {
                Invoke(nameof(PlayThruster), 0.2f);
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.HARD_LAUNCH_ANIMATION_NAME, false);
            }
            else
            {
                rigidbody2D.velocity = Vector2.zero;

                Invoke(nameof(PlayThruster), 0.2f);
                PlayerAnimationController.Instance.PlayAnimation(AnimationNames.SOFT_LAUNCH_ANIMATION_NAME, false);
            }
        }
    }

    private void PlayThruster()
    {
        if (finalForce.magnitude > 2f)
        {
            PlayerAnimationController.Instance.PlayAnimation(AnimationNames.FLOATING_ANIMATION_NAME, true);
            PlayerAnimationController.Instance.PlayThrusterAnimation(true, false);
        }


        rigidbody2D.AddForce(-finalForce, ForceMode2D.Impulse);
        isReleased = true;

        FuelController.Instance.ApplyPredictionFuel();
    }
}