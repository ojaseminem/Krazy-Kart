using Data;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Ads
{
    public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
    {
        private string _gameId;

        public void InitializeAds(AdsData adsData)
        {
#if UNITY_ANDROID
            _gameId = adsData.androidGameId;
#endif
#if UNITY_EDITOR
            _gameId = adsData.androidGameId;
#endif
            //Add for iOS when we start iOS

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                Advertisement.Initialize(_gameId, adsData.testMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            Debug.Log("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }
    }
}