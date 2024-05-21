using System;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class ScoreController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        private int scoreCount;

        private void Awake()
        {
            scoreCount = 0;
            scoreText.text = scoreCount.ToString();
        }

        public void UpdateScoreText(int amount)
        {
            scoreCount += amount;
            scoreText.text = scoreCount.ToString();
        }
    }
}
