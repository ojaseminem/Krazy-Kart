using System.Collections;
using Flow;
using Managers;
using UnityEngine;

namespace MainMenu
{
    /// Orchestrates the whole menu flow.
    ///
    /// ENTER: fade up from black → 3D drift cinematic → UI pops in.
    /// EXIT:  UI slides out → kart drives past camera → SceneFlow fades out and hands over to
    ///        the loading screen, whose staged pacing is owned by GameplayLoadDirector.
    public class MainMenuManager : MonoBehaviour
    {
        [Header("Layers")]
        [SerializeField] private MainMenuCinematic cinematic;
        [SerializeField] private MainMenuUi ui;

        [Header("Loading")]
        [Tooltip("Supplies the staged load plan. Left empty, a default plan is used.")]
        [SerializeField] private GameplayLoadDirector loadDirector;
        [SerializeField] private string gameplaySceneName = "GameplayScene";

        [Header("Timing")]
        [SerializeField] private float openingFadeDuration = 0.6f;
        [Tooltip("Beat between the kart settling and the UI arriving.")]
        [SerializeField] private float uiDelay = 0.15f;
        [Tooltip("Head start the kart gets before the screen begins fading on exit.")]
        [SerializeField] private float exitFadeLead = 0.55f;

        private bool _exiting;

        private void Awake()
        {
            if (ui != null) ui.OnPlayRequested += HandlePlayRequested;
        }

        private void OnDestroy()
        {
            if (ui != null) ui.OnPlayRequested -= HandlePlayRequested;
        }

        private IEnumerator Start()
        {
            // Any transition into this scene leaves the fader opaque; clear it.
            yield return ScreenFader.Instance.FadeIn(openingFadeDuration).WaitDone();

            if (cinematic != null)
                yield return cinematic.PlayEnter().WaitDone();

            if (uiDelay > 0f) yield return new WaitForSeconds(uiDelay);

            if (ui != null) ui.PlayIntro();
        }

        private void HandlePlayRequested(GameMode mode)
        {
            if (_exiting) return;

            _exiting = true;
            StartCoroutine(ExitRoutine(mode));
        }

        private IEnumerator ExitRoutine(GameMode mode)
        {
            GameSession.SelectMode(mode);

            if (ui != null) ui.PlayOutro();

            if (cinematic != null)
            {
                // The kart clears the lens before the fade starts, so the drive-by reads fully.
                cinematic.PlayExit();
                yield return new WaitForSeconds(exitFadeLead);
            }

            if (loadDirector != null)
            {
                loadDirector.LoadGameplay(mode);
                yield break;
            }

            Debug.LogWarning("[MainMenuManager] No GameplayLoadDirector assigned; using a default load plan.");
            SceneFlow.Instance.TransitionTo(gameplaySceneName, new[]
            {
                new LoadStage("Unlocking the shutters", 1f, 0.8f),
                new LoadStage("Stocking the shelves", 1.4f, 1.1f),
                new LoadStage("Warming the tyres", 1.2f, 1f)
            });
        }
    }
}
