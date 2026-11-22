using UnityEngine;
using UnityEngine.UI;
using CheremoRunner.Core;
using CheremoRunner.Audio;

namespace CheremoRunner.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("Toggles")]
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Toggle vibrationToggle;

        [Header("Quality Buttons")]
        [SerializeField] private Button lowQualityBtn;
        [SerializeField] private Button medQualityBtn;
        [SerializeField] private Button highQualityBtn;

        private void OnEnable()
        {
            if (SaveManager.Instance == null) return;

            if (musicToggle != null) musicToggle.isOn = SaveManager.Instance.MusicEnabled;
            if (sfxToggle != null) sfxToggle.isOn = SaveManager.Instance.SfxEnabled;
            if (vibrationToggle != null) vibrationToggle.isOn = SaveManager.Instance.VibrationEnabled;
        }

        public void OnToggleMusic(bool value)
        {
            SaveManager.Instance?.SetMusicEnabled(value);
            AudioManager.Instance?.SetMusicMute(!value);
        }

        public void OnToggleSfx(bool value)
        {
            SaveManager.Instance?.SetSfxEnabled(value);
        }

        public void OnToggleVibration(bool value)
        {
            SaveManager.Instance?.SetVibrationEnabled(value);
        }

        public void OnSelectQuality(int level)
        {
            SaveManager.Instance?.SetQualityLevel(level);
            AudioManager.Instance?.PlayClickSound();
        }

        public void OnClickBack()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
