using UnityEngine;

public class PlayeController : MonoBehaviour
{
    public float speed = 5.5f;
    float maxY = 3.0f; 
    float minY = -3.0f; 
    private float timeCounter = 0f;

    // void Update()
    // {
    //     timeCounter += Time.deltaTime * speed;
    //     float newY = Mathf.Sin(timeCounter) * (maxY - minY) / 2f + (maxY + minY) / 2f;
    //     transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    // }
}
