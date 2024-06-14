using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1_Red_Crystals_Tag : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision object has the tag "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            // Destroy this game object
            Debug.Log("Collided with Player");
            Destroy(gameObject);
        }
    }

    public void DestroyRedCrystal()
    {
        Debug.Log("collided");
        Destroy(gameObject);
    }
}
