using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Misc;
using Store;
using UnityEngine;

namespace Managers
{
    public class StoreManager : MonoBehaviour
    {
        public List<StoreController> storeControllerList;

        public bool indicate;

        private LevelData _levelData;

        private void Awake() => GameManager.Instance.storeManager = this;

        private void Start() => _levelData = GameManager.Instance.levelData;

        public void CheckStore(StoreType storeType)
        {
            foreach (var storeController in storeControllerList)
            {
                if (storeController.currStoreType != storeType) return;
                if (!storeController.isDangerous) return;
                IndicateDanger(storeController);
            }
        }

        private void IndicateDanger(StoreController storeController)
        {
            indicate = true;
            
            foreach (var storeLight in storeController.currStoreLights)
            {
                StartCoroutine(Indicate(storeLight));
            }
            
            IEnumerator Indicate(Light currLight)
            {
                if(SkyboxController.SkyboxCountDown() != null) StopCoroutine(SkyboxController.SkyboxCountDown());

                while (indicate)
                {
                    var t = Mathf.PingPong(Time.time * _levelData.colorChangeSpeed, 1f);
                    var newColor = Color32.Lerp(_levelData.defaultColor, _levelData.dangerColor, t);
                    currLight.color = newColor;

                    var newValue = Mathf.Lerp(0.3f, 1f, t);
                    _levelData.skyboxMat.SetFloat(_levelData.CubemapTransition, newValue);
                    
                    yield return null;
                }
                
                currLight.color = _levelData.defaultColor;

                StartCoroutine(SkyboxController.SkyboxCountDown(
                    _levelData.skyboxMat.GetFloat(_levelData.CubemapTransition),
                    0f, true, 2f));
            }
        }
    }
}