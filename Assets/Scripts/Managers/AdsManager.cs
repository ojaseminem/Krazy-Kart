using System;
using Data;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Managers
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] private Button watchAdsButton;
        private string _adUnitId;

        private AdsData _adsData;
        private AdsType _adsType;

        public void LoadAd(AdsType adsType)
        {
            _adsData = GameManager.Instance.adsData;
            _adsType = adsType;
            
            _adUnitId = _adsType switch
            {
                AdsType.Interstitial => _adsData.interstitialAndroidAdUnitId,
                AdsType.Rewarded => _adsData.rewardedAndroidAdUnitId,
                AdsType.Banner => _adsData.bannerAndroidAdUnitId,
                _ => throw new ArgumentOutOfRangeException(nameof(adsType), adsType, null)
            };

            Debug.Log("Loading Ad: " + _adUnitId);
            //Advertisement.Load(_adUnitId, this);
        }

        public void ShowAd()
        {
            if (_adsType == AdsType.Rewarded)
            {
                watchAdsButton.interactable = false;
                watchAdsButton.onClick.RemoveAllListeners();
            }
            Debug.Log("Showing Ad: " + _adUnitId);
            //Advertisement.Show(_adUnitId, this);
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            if (adUnitId != _adsData.rewardedAndroidAdUnitId) return;
            
            if (_adUnitId.Equals(adUnitId))
            {
                watchAdsButton.onClick.AddListener(ShowAd);
                watchAdsButton.interactable = true;
            }
        }

        /*public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error Loading Ad Unity: {adUnitId} - {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error Showing Ad Unity {adUnitId}: {error.ToString()} - {message}");
        }

        public void OnUnityAdsShowStart(string adUnitId){}
        public void OnUnityAdsShowClick(string adUnitId){}

        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (_adUnitId.Equals(adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                Debug.Log("Unity Ads Rewarded Ad Completed");
                //Grant Rewards
            }
        }*/
    }
}