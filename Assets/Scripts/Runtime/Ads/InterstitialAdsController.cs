using System;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Runtime.Ads
{
    public class InterstitialAdsController : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        public static InterstitialAdsController Instance;
        [SerializeField] private string _androidAdUnitId = "Interstitial_Android";
        [SerializeField] private string _iOsAdUnitId = "Interstitial_iOS";
        private string _adUnitId;

        private Action adsCompleteCallback;

        private void Awake()
        {
            Instance = this;

            // Get the Ad Unit ID for the current platform:
            _adUnitId = Application.platform == RuntimePlatform.IPhonePlayer
                ? _iOsAdUnitId
                : _androidAdUnitId;
        }

        // Load content to the Ad Unit:
        public void LoadAd(Action adsCompleteCallback)
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + _adUnitId);
            this.adsCompleteCallback = adsCompleteCallback;
            Advertisement.Load(_adUnitId, this);
        }

        // Show the loaded content in the Ad Unit:
        public void ShowAd()
        {
            this.adsCompleteCallback = adsCompleteCallback;
            // Note that if the ad content wasn't previously loaded, this method will fail
            Debug.Log("Showing Ad: " + _adUnitId);
            Advertisement.Show(_adUnitId, this);
        }

        // Implement Load Listener and Show Listener interface methods: 
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            // Optionally execute code if the Ad Unit successfully loads content.
            ShowAd();
        }

        public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
            
            adsCompleteCallback?.Invoke();
        }

        public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
            
            adsCompleteCallback?.Invoke();
        }

        public void OnUnityAdsShowStart(string _adUnitId)
        {
        }

        public void OnUnityAdsShowClick(string _adUnitId)
        {
            adsCompleteCallback?.Invoke();
        }

        public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
           // if (showCompletionState is UnityAdsShowCompletionState.COMPLETED or UnityAdsShowCompletionState.SKIPPED)
            {
                adsCompleteCallback?.Invoke();
            }
        }
    }
}