using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Slider slider; // Progress filler
    float fillDuration = 60f;
    int Scores = 0;
    public Text ScoreText;
    void Start()
    {
        Scores = 0;
        StartCoroutine(ProgressBar());
    }

    public void AddScore()
    {
        Scores ++;
        ScoreText.text = Scores.ToString("");
    }

    IEnumerator ProgressBar()
    {
        float timer = 0f;
        float startValue = slider.value;
        float endValue = 1f; 

        // Fill the slider over the specified duration
        while (timer < fillDuration)
        {
            float progress = timer / fillDuration;
            slider.value = Mathf.Lerp(startValue, endValue, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        slider.value = endValue;
    }
}
