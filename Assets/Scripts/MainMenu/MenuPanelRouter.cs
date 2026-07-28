using System.Collections.Generic;
using Flow;
using UI.Parent;
using UnityEngine;

namespace MainMenu
{
    /// Spawns menu panels from prefabs on demand and keeps one open at a time.
    ///
    /// Panels are resolved from Resources/MenuPanels/{PanelType} so that adding or editing a
    /// panel is a prefab-only change — the menu scene never has to be touched or committed.
    public class MenuPanelRouter : MonoBehaviour
    {
        private const string PanelFolder = "MenuPanels/";

        [Header("Hierarchy")]
        [Tooltip("Canvas child that spawned panels are parented under.")]
        [SerializeField] private RectTransform panelRoot;

        private readonly Dictionary<MenuPanelType, MenuPanel> _cache = new Dictionary<MenuPanelType, MenuPanel>();

        public MenuPanel CurrentPanel { get; private set; }
        public bool HasOpenPanel => CurrentPanel != null;

        public void Open(MenuPanelType type)
        {
            var panel = Resolve(type);
            if (panel == null) return;

            if (CurrentPanel == panel)
            {
                panel.Open();
                return;
            }

            if (CurrentPanel != null)
            {
                var outgoing = CurrentPanel;
                outgoing.Close(() => ShowPanel(panel));
                CurrentPanel = null;
                return;
            }

            ShowPanel(panel);
        }

        public void CloseCurrent()
        {
            if (CurrentPanel == null) return;

            CurrentPanel.Close();
            CurrentPanel = null;
        }

        private void ShowPanel(MenuPanel panel)
        {
            CurrentPanel = panel;
            panel.transform.SetAsLastSibling();
            panel.Open();
        }

        private MenuPanel Resolve(MenuPanelType type)
        {
            if (_cache.TryGetValue(type, out var cached) && cached != null) return cached;

            string path = PanelFolder + type;
            var prefab = Resources.Load<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[MenuPanelRouter] No panel prefab at Resources/{path}");
                return null;
            }

            var parent = panelRoot != null ? panelRoot : (RectTransform)transform;
            var instance = Instantiate(prefab, parent);
            instance.name = prefab.name;

            var panel = instance.GetComponent<MenuPanel>();
            if (panel == null)
            {
                Debug.LogError($"[MenuPanelRouter] Prefab at Resources/{path} has no MenuPanel component.");
                Destroy(instance);
                return null;
            }

            panel.OnCloseRequested += HandleCloseRequested;
            panel.gameObject.SetActive(false);
            _cache[type] = panel;
            return panel;
        }

        private void HandleCloseRequested(MenuPanel panel)
        {
            if (CurrentPanel != panel) return;

            CloseCurrent();
        }

        private void OnDestroy()
        {
            foreach (var kvp in _cache)
            {
                if (kvp.Value != null) kvp.Value.OnCloseRequested -= HandleCloseRequested;
            }
        }
    }
}
