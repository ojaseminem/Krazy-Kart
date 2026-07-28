using UI.Parent;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace UI.Panels
{
    /// Example modular panel. Lives entirely inside its own prefab
    /// (Resources/MenuPanels/Settings) — editing it never dirties the menu scene.
    public class SettingsPanel : MenuPanel
    {
        private const string MasterKey = "settings.volume.master";
        private const string MusicKey = "settings.volume.music";
        private const string SfxKey = "settings.volume.sfx";
        private const string InvertKey = "settings.camera.invert";

        [Header("Audio")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("Gameplay")]
        [SerializeField] private Toggle invertCameraToggle;

        protected override void Awake()
        {
            base.Awake();
            LoadValues();

            if (masterSlider != null) masterSlider.onValueChanged.AddListener(v => ApplyVolume("MasterVolume", v));
            if (musicSlider != null) musicSlider.onValueChanged.AddListener(v => ApplyVolume("MusicVolume", v));
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(v => ApplyVolume("SfxVolume", v));
        }

        protected override void OnClosing()
        {
            SaveValues();
        }

        private void LoadValues()
        {
            if (masterSlider != null) masterSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MasterKey, 0.8f));
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MusicKey, 0.7f));
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SfxKey, 0.9f));
            if (invertCameraToggle != null) invertCameraToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(InvertKey, 0) == 1);

            if (masterSlider != null) ApplyVolume("MasterVolume", masterSlider.value);
            if (musicSlider != null) ApplyVolume("MusicVolume", musicSlider.value);
            if (sfxSlider != null) ApplyVolume("SfxVolume", sfxSlider.value);
        }

        private void SaveValues()
        {
            if (masterSlider != null) PlayerPrefs.SetFloat(MasterKey, masterSlider.value);
            if (musicSlider != null) PlayerPrefs.SetFloat(MusicKey, musicSlider.value);
            if (sfxSlider != null) PlayerPrefs.SetFloat(SfxKey, sfxSlider.value);
            if (invertCameraToggle != null) PlayerPrefs.SetInt(InvertKey, invertCameraToggle.isOn ? 1 : 0);

            PlayerPrefs.Save();
        }

        /// Sliders run 0..1; mixers want decibels, and log10(0) is undefined.
        private void ApplyVolume(string exposedParam, float normalised)
        {
            if (mixer == null) return;

            float db = normalised <= 0.0001f ? -80f : Mathf.Log10(normalised) * 20f;
            mixer.SetFloat(exposedParam, db);
        }
    }
}
