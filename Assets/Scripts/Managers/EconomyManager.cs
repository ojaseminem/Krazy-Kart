using System;
using Data;
using UnityEngine;

namespace Managers
{
    public class EconomyManager : MonoBehaviour
    {
        private EconomyData _economyData;

        public void Init(EconomyData data) => _economyData = data;

        public void Earn(EconomyType economyType, int amount)
        {
            switch (economyType)
            {
                case EconomyType.Coin:
                    _economyData.coinCount += amount;
                    break;
                case EconomyType.Token:
                    _economyData.tokenCount += amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(economyType), economyType, null);
            }
            //SaveData
        }
        
        public void Spend(EconomyType economyType, int amount)
        {
            switch (economyType)
            {
                case EconomyType.Coin:
                    _economyData.coinCount -= amount;
                    break;
                case EconomyType.Token:
                    _economyData.tokenCount -= amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(economyType), economyType, null);
            }
            //SaveData
        }
    }
}