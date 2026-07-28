using System;
using UnityEngine;

namespace Loading
{
    /// Persistent owner of the loading screen prefab. Survives scene loads so the screen can
    /// stay up across the menu → gameplay boundary. Holds no pacing logic: callers report
    /// progress into it (see GameplayLoadDirector).
    public class LoadingScreenService : MonoBehaviour
    {
        private const string PrefabPath = "LoadingScenePrefabs/LoadingScreen";

        private static LoadingScreenService _instance;

        public static LoadingScreenService Instance
        {
            get
            {
                if (_instance != null) return _instance;

                var host = new GameObject("[LoadingScreenService]");
                _instance = host.AddComponent<LoadingScreenService>();
                DontDestroyOnLoad(host);
                return _instance;
            }
        }

        public bool IsVisible { get; private set; }

        private LoadingScreenView _view;

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

        public void Show(string initialLabel = null)
        {
            EnsureView();
            if (_view == null) return;

            IsVisible = true;
            _view.Show();
            if (!string.IsNullOrEmpty(initialLabel)) _view.SetStageLabel(initialLabel);
            _view.SetProgress(0f);
        }

        public void Hide()
        {
            if (_view == null) return;

            IsVisible = false;
            _view.Hide();
        }

        public void ReportProgress(float normalised)
        {
            if (_view != null) _view.SetProgress(normalised);
        }

        public void ReportStage(string label)
        {
            if (_view != null) _view.SetStageLabel(label);
        }

        private void EnsureView()
        {
            if (_view != null) return;

            var prefab = Resources.Load<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[LoadingScreenService] Prefab missing at Resources/{PrefabPath}");
                return;
            }

            var instance = Instantiate(prefab, transform);
            _view = instance.GetComponent<LoadingScreenView>();

            if (_view == null)
                Debug.LogError($"[LoadingScreenService] Prefab at Resources/{PrefabPath} has no LoadingScreenView.");
        }
    }
}
