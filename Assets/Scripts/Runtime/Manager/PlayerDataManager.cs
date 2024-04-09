using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Manager
{
    public class PlayerDataManager : MonoBehaviour
    {
        public static PlayerDataManager Instance;

        [ShowInInspector] private int _currentStageLevel = 1;
        public int CurrentStageLevel => _currentStageLevel;

        [ShowInInspector] private int _isRemovedStageClearAds;

        private void Awake()
        {
            Instance = this;

            var stageClearAds = PlayerPrefs.GetInt("IsRemovedStageClearAds", 0);
            _isRemovedStageClearAds = stageClearAds;

            DontDestroyOnLoad(this);
        }

        public bool IsRemoveStageClearAds()
        {
            return !_isRemovedStageClearAds.Equals(0);
        }

        public void AddStageLevel()
        {
            _currentStageLevel++;
        }

        public void RemoveAds()
        {
            _isRemovedStageClearAds = 1;
            PlayerPrefs.SetInt("IsRemovedStageClearAds", 1);
        }
    }
}