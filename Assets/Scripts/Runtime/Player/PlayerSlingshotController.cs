using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSlingshotController : MonoBehaviour
{
    [SerializeField] private GameObject pivot;
    [SerializeField] private Transform pivotFront;
    [SerializeField] private Transform pivotRear;
    [SerializeField] private LineRenderer lineRenderer;

    private bool isDragging;
    private Camera mainCamera;
    private Rigidbody2D rigidbody2D;
    private SpringJoint2D springJoint2D;

    public bool canDrag;
    private bool playerLaunched;
    
    private void Start()
    {
        mainCamera = Camera.main;
        rigidbody2D = GetComponent<Rigidbody2D>();
        springJoint2D = GetComponent<SpringJoint2D>();

        canDrag = true;
    }

    private void Update()
    {
        DetectTouch();
        DetachBall();
    }

    private void DetectTouch()
    {
        // if screen is not being touched
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            //but was
            if (isDragging)
            {
                LaunchPlayer();
                playerLaunched = true;
            }

            isDragging = false;
            return;
        }

        if (canDrag)
        {
            isDragging = true;
            rigidbody2D.isKinematic = true;

            //get position in pixel
            var touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            
            //convert to world coordinates
            var worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
            worldPosition.z = transform.position.z;

            transform.position = worldPosition;
        }
    }

    private void LaunchPlayer()
    {
        //throw new NotImplementedException();
        rigidbody2D.isKinematic = false;
        canDrag = false;
    }

    private void DetachBall()
    {
        if (transform.position.x > pivot.transform.position.x && !isDragging)
        {
            springJoint2D.enabled = false;
        }
    }

    private void ShowLine()
    {
        if (playerLaunched)
        {
            lineRenderer.SetPosition(1, pivotFront.position);
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position);
        }
        
        lineRenderer.SetPosition(0, pivotFront.position);
        lineRenderer.SetPosition(2, pivotRear.position);
    }

    private void LaunchBall()
    {
        rigidbody2D.isKinematic = false;
        canDrag = false;
    }
}
