using UnityEngine;
using UnityEngine.UI;

public class Orbls : MonoBehaviour
{
    float speed = 1.8f;
    private float timeCounter = 0f;

    [SerializeField]
    public Score score;
    
    void Start()
    {
        score = FindObjectOfType<Score>();
    }
    
    void Update()
    {
        timeCounter += Time.deltaTime * speed;
        float newX = transform.position.x - speed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            score.AddScore();

        }
    }
}
