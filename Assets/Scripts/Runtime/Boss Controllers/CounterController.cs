using System;
using System.Globalization;
using Runtime.Boss_Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.UI_Controller
{
    public class CounterController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _fpsCounter;
        [SerializeField] private TextMeshProUGUI _timerCounterMinutes;
        [SerializeField] private TextMeshProUGUI _timerCounterSeconds;
        [SerializeField] private BossController _bossController;
        [SerializeField] private GameObject _gameOverGameObject;
        [SerializeField] private int _totalSeconds;

        private float _fpsUpdateTimer = 0.2f;
        private float _currentFps;
        private float _timerValue;

        private int m_currentMinutes;
        public int CurrentMinutes => m_currentMinutes;
        
        private int m_currentSeconds;
        public int CurrentSeconds => m_currentSeconds;

        private void Awake()
        {
            _timerValue = _totalSeconds;
        }

        private void UpdateFPSCounter()
        {
            _fpsUpdateTimer -= Time.deltaTime;
            if (_fpsUpdateTimer <= 0f)
            {
                _currentFps = 1f / Time.unscaledDeltaTime;
                _fpsCounter.text = "FPS: " + Mathf.Round(_currentFps);
                _fpsUpdateTimer = 0.2f;
            }
        }

        private void UpdateTimer()
        {
            _timerValue -= Time.deltaTime;

            if (_timerValue < 0)
            {
                if (!_gameOverGameObject.activeInHierarchy)
                {
                    SceneManager.LoadScene(2);
                }

                return;
            }
                

            m_currentMinutes = Mathf.FloorToInt(_timerValue / 60);
            if (m_currentMinutes < 10)
            {
                _timerCounterMinutes.text = "0" + m_currentMinutes.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                _timerCounterMinutes.text = m_currentMinutes.ToString(CultureInfo.InvariantCulture);
            }

            m_currentSeconds = Mathf.FloorToInt(_timerValue % 60);
            if (m_currentSeconds < 10)
            {
                _timerCounterSeconds.text = "0" + m_currentSeconds.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                _timerCounterSeconds.text = m_currentSeconds.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void Update()
        {
            //UpdateFPSCounter();
            UpdateTimer();
        }
    }
}