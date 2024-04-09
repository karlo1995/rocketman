using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLivesManager : MonoBehaviour
{
    public static PlayerLivesManager Instance;
    private const int MAX_LIVES = 5;
    [SerializeField] private TextMeshProUGUI livesCounter;

    private int lifeCounter;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        lifeCounter = MAX_LIVES;
        livesCounter.text = lifeCounter.ToString();
    }

    public void AddLife()
    {
        lifeCounter++;
        livesCounter.text = lifeCounter.ToString();
    }

    public void DeductLife()
    {
        lifeCounter--;
        if (lifeCounter <= 0)
        {
            livesCounter.text = "0";
            //Game over logic
        }
        else
        {
            livesCounter.text = lifeCounter.ToString();
        }
    }
}