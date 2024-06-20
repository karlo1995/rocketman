using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Waveform : MonoBehaviour
{
    int resolution = 100;
    float amplitude = 0.5f;
    float lineWidth = 0.1f;
    Vector2 startingPosition = new Vector2(-7.7f, 0f);
    private LineRenderer lineRenderer;
    public float frequency;

    public float crawlSpeed = -2f; // speed of the crawling animation like a snake
    public float crawlOffset = 0f;
    public float horizontalLength = 5f;
    [SerializeField]
    public PlayeController _playerController;

    public GameObject attachedObjectPrefab; // srefab of the object to attach
    private GameObject attachedObject;

    void Start()
    {
        amplitude = 3;
        lineRenderer = GetComponent<LineRenderer>();
        UpdateLineRenderer();
        _playerController = FindObjectOfType<PlayeController>();
    }

    float timer = 0f;
    float interval = 2f; // interval between instantiations
    int objectsToInstantiate = 3;

    public GameObject orbPrefab;

    void Update()
    {
        crawlOffset += Time.deltaTime * crawlSpeed;
        UpdateAmplitude();
        UpdateLineRenderer();

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            InstantiateObjects(objectsToInstantiate);
            timer = 0f; // Reset the timer
        }


        //iere will destory if the object reached the -14 xpos
        GameObject[] orbs = GameObject.FindGameObjectsWithTag("orbs");
        foreach (GameObject orb in orbs)
        {
            if (Mathf.Abs(orb.transform.position.x - (-14f)) < 0.01f)
            {
                Destroy(orb);
            }
        }
    }

    void UpdateAmplitude()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                if (touch.deltaPosition.y > 0)
                {
                    amplitude = Mathf.Min(amplitude + 0.1f, 3.0f);
                    _playerController.speed = Mathf.Min(_playerController.speed + 1, 5.5f);
                }
                else if (touch.deltaPosition.y < 0)
                {
                    _playerController.speed = Mathf.Max(_playerController.speed - 1, 1);
                    amplitude = Mathf.Max(amplitude - 0.1f, 0.5f);
                }
            }
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = resolution;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] points = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float t = i / (float)(resolution - 1);
            float x = t * 2f - 1f;
            float y = Mathf.Sin((x + crawlOffset) * frequency) * amplitude;

            x *= horizontalLength;

            points[i] = new Vector3(x, y, 0f);
        }
        lineRenderer.SetPositions(points);

        Vector3 endPointPosition = FindEndPointOfLeftLine(points);

        // here will instantiate or update the attached object at the end point position
        if (attachedObject == null && attachedObjectPrefab != null)
        {
            attachedObject = Instantiate(attachedObjectPrefab, endPointPosition, Quaternion.identity);
        }
        else if (attachedObject != null)
        {
            attachedObject.transform.position = endPointPosition;
        }
    }

    void InstantiateObjects(int count)
    {
        // get the positions of the line renderer
        Vector3[] linePositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(linePositions);

        // find the maximum x-coordinate of the line points
        float maxX = float.MinValue;
        foreach (Vector3 point in linePositions)
        {
            if (point.x > maxX)
            {
                maxX = point.x;
            }
        }

        // instantiate objects at the right edge
        for (int i = 0; i < count; i++)
        {
            //float t = Random.value; // Random position along the line
            float x = maxX; // Set x-coordinate to the maximum value
            float y = Mathf.Sin((x + crawlOffset) * frequency) * amplitude;

            Vector3 position = new Vector3(x, y, 0f);

            GameObject orb = Instantiate(orbPrefab, position, Quaternion.identity);
            orb.transform.parent = transform; // ensure the orbs are part of the line
        }
    }




    //this function will follow the left line render
    Vector3 FindEndPointOfLeftLine(Vector3[] points)
    {
        Vector3 endPoint = points[0];
        foreach (Vector3 point in points)
        {
            if (point.x < endPoint.x)
            {
                endPoint = point;
            }
        }
        return endPoint;
    }

}