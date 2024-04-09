using DG.Tweening;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public static WallController Instance;
    [SerializeField] private Transform wallLeft;
    [SerializeField] private Transform wallRight;

    private void Awake()
    {
        Instance = this;
    }
    
    private Bounds GetCameraOrthographicBounds()
    {
        var screenAspect = Screen.width / Screen.height;
        var cameraHeight = Camera.main.orthographicSize * 2;
        var bounds = new Bounds(Camera.main.transform.position, new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        
        return bounds;
    }

    public void UpdateWallPosition()
    {
        var cameraBound = GetCameraOrthographicBounds();
        wallLeft.DOMove(new Vector2(cameraBound.min.x - 0.5f, cameraBound.min.y), 0f);
        wallRight.DOMove(new Vector2(cameraBound.max.x + 0.5f, cameraBound.max.y), 0f);
    }
}