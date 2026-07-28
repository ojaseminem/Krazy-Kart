using System.Collections.Generic;
using Flow;
using UnityEngine;

namespace Managers
{
    /// Owns *what* the game claims to be loading and how long each step lingers.
    ///
    /// This deliberately lives on the gameplay/environment side rather than inside the loading
    /// screen: the loading screen only renders progress, while this describes the work. Tune the
    /// stage lists in the Inspector without touching any UI prefab.
    public class GameplayLoadDirector : MonoBehaviour
    {
        private static GameplayLoadDirector _instance;

        public static GameplayLoadDirector Instance => _instance;

        [Header("Target Scenes")]
        [SerializeField] private string gameplaySceneName = "GameplayScene";

        [Header("Normal Run Stages")]
        [SerializeField]
        private List<LoadStage> normalRunStages = new List<LoadStage>
        {
            new LoadStage("Unlocking the shutters", 1.0f, 0.8f),
            new LoadStage("Stocking the shelves", 1.4f, 1.1f),
            new LoadStage("Waking the mall cops", 1.0f, 0.9f),
            new LoadStage("Warming the tyres", 1.2f, 1.0f)
        };

        [Header("Free Run Stages")]
        [SerializeField]
        private List<LoadStage> freeRunStages = new List<LoadStage>
        {
            new LoadStage("Clearing the floor", 1.0f, 0.8f),
            new LoadStage("Stocking the shelves", 1.4f, 1.1f),
            new LoadStage("Sending the cops home", 1.0f, 0.7f),
            new LoadStage("Warming the tyres", 1.2f, 1.0f)
        };

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// Kicks off the menu → gameplay transition using the stage list for the chosen mode.
        public void LoadGameplay(GameMode mode)
        {
            GameSession.SelectMode(mode);
            SceneFlow.Instance.TransitionTo(gameplaySceneName, StagesFor(mode));
        }

        public IReadOnlyList<LoadStage> StagesFor(GameMode mode)
        {
            return mode == GameMode.FreeRun ? freeRunStages : normalRunStages;
        }
    }
}
