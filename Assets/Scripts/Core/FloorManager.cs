using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    /// Persistent singleton. Owns run-wide floor state: current floor, MC totals, cop count,
    /// and additive scene load/unload for floor + corridor scenes.
    public class FloorManager : MonoBehaviour
    {
        public static FloorManager Instance { get; private set; }

        public MallFloor CurrentFloor { get; private set; }
        public int TotalMC { get; private set; }
        public int FloorMC { get; private set; }
        public int ActiveCopCount { get; private set; }
        public int PropsDestroyedThisFloor { get; private set; }

        public event Action<MallFloor> OnFloorChanged;
        public event Action<int> OnMCEarned;
        public event Action<int> OnCopCountChanged;
        /// Fired once when destruction count crosses a cop-spawn threshold. Arg = karts to spawn (1, 1, or 3).
        public event Action<int> OnCopThresholdReached;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetCurrentFloor(MallFloor floor)
        {
            CurrentFloor = floor;
            FloorMC = 0;
            PropsDestroyedThisFloor = 0;
            ActiveCopCount = 0;
            OnFloorChanged?.Invoke(floor);
        }

        public void AddMC(int amount)
        {
            if (amount == 0) return;
            FloorMC += amount;
            TotalMC += amount;
            OnMCEarned?.Invoke(amount);
        }

        public void RegisterPropDestroyed()
        {
            PropsDestroyedThisFloor++;

            if (CurrentFloor == null || !CurrentFloor.CopsEnabled) return;

            if (PropsDestroyedThisFloor == CurrentFloor.CopThreshold1)
                OnCopThresholdReached?.Invoke(1);
            else if (PropsDestroyedThisFloor == CurrentFloor.CopThreshold2)
                OnCopThresholdReached?.Invoke(1);
            else if (PropsDestroyedThisFloor == CurrentFloor.CopThreshold3)
                OnCopThresholdReached?.Invoke(3);
        }

        public void SetActiveCopCount(int count)
        {
            ActiveCopCount = Mathf.Max(0, count);
            OnCopCountChanged?.Invoke(ActiveCopCount);
        }

        public void LoadFloorAdditive(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public void UnloadFloorAdditive(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return;
            var scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
                SceneManager.UnloadSceneAsync(sceneName);
        }
    }
}
