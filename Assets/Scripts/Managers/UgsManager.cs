using System;
using Unity.Services.Core;
using UnityEngine;

namespace Managers
{
    public class UgsManager : MonoBehaviour
    {
        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}