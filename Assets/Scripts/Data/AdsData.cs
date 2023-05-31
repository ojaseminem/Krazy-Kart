using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "AdsData", menuName = "Data/AdsData")]
    public class AdsData : ScriptableObject
    {
        [Header("Game Id")]
        public string androidGameId;
        public bool testMode = true;

        [Header("Ad Unit Id")]
        public string interstitialAndroidAdUnitId = "Interstitial_Android";
        public string rewardedAndroidAdUnitId = "Rewarded_Android";
        public string bannerAndroidAdUnitId = "Banner_Android";
    }
}