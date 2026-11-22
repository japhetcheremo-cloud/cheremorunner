using UnityEngine;
using System.Collections.Generic;

namespace CheremoRunner.Core
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        public int HighScore { get; private set; }
        public int TotalCoins { get; private set; }
        public int SelectedSkinId { get; private set; }
        public bool MusicEnabled { get; private set; }
        public bool SfxEnabled { get; private set; }
        public bool VibrationEnabled { get; private set; }
        public int QualityLevel { get; private set; }
        public int MagnetLevel { get; private set; }
        public int ShieldLevel { get; private set; }
        public int MultiplierLevel { get; private set; }

        private HashSet<int> unlockedSkins = new HashSet<int>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadAllData()
        {
            HighScore = PlayerPrefs.GetInt(GameConstants.PREF_HIGH_SCORE, 0);
            TotalCoins = PlayerPrefs.GetInt(GameConstants.PREF_TOTAL_COINS, 0);
            SelectedSkinId = PlayerPrefs.GetInt(GameConstants.PREF_SELECTED_SKIN, 0);
            MusicEnabled = PlayerPrefs.GetInt(GameConstants.PREF_MUSIC_ENABLED, 1) == 1;
            SfxEnabled = PlayerPrefs.GetInt(GameConstants.PREF_SFX_ENABLED, 1) == 1;
            VibrationEnabled = PlayerPrefs.GetInt(GameConstants.PREF_VIBRATION_ENABLED, 1) == 1;
            QualityLevel = PlayerPrefs.GetInt(GameConstants.PREF_QUALITY_LEVEL, 2); // 0=Low, 1=Med, 2=High
            MagnetLevel = PlayerPrefs.GetInt(GameConstants.PREF_MAGNET_LEVEL, 1);
            ShieldLevel = PlayerPrefs.GetInt(GameConstants.PREF_SHIELD_LEVEL, 1);
            MultiplierLevel = PlayerPrefs.GetInt(GameConstants.PREF_MULTIPLIER_LEVEL, 1);

            // Load unlocked skins
            unlockedSkins.Clear();
            unlockedSkins.Add(0); // Default skin always unlocked
            string skinsStr = PlayerPrefs.GetString(GameConstants.PREF_UNLOCKED_SKINS, "0");
            string[] parts = skinsStr.Split(',');
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int id))
                {
                    unlockedSkins.Add(id);
                }
            }

            QualitySettings.SetQualityLevel(QualityLevel);
        }

        public void SaveScoreIfHigh(int score)
        {
            if (score > HighScore)
            {
                HighScore = score;
                PlayerPrefs.SetInt(GameConstants.PREF_HIGH_SCORE, HighScore);
                PlayerPrefs.Save();
            }
        }

        public void AddCoins(int amount)
        {
            TotalCoins += amount;
            PlayerPrefs.SetInt(GameConstants.PREF_TOTAL_COINS, TotalCoins);
            PlayerPrefs.Save();
        }

        public bool SpendCoins(int amount)
        {
            if (TotalCoins >= amount)
            {
                TotalCoins -= amount;
                PlayerPrefs.SetInt(GameConstants.PREF_TOTAL_COINS, TotalCoins);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        public bool IsSkinUnlocked(int skinId)
        {
            return unlockedSkins.Contains(skinId);
        }

        public void UnlockSkin(int skinId)
        {
            unlockedSkins.Add(skinId);
            string serialized = string.Join(",", unlockedSkins);
            PlayerPrefs.SetString(GameConstants.PREF_UNLOCKED_SKINS, serialized);
            PlayerPrefs.Save();
        }

        public void SelectSkin(int skinId)
        {
            if (IsSkinUnlocked(skinId))
            {
                SelectedSkinId = skinId;
                PlayerPrefs.SetInt(GameConstants.PREF_SELECTED_SKIN, SelectedSkinId);
                PlayerPrefs.Save();
            }
        }

        public void SetMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;
            PlayerPrefs.SetInt(GameConstants.PREF_MUSIC_ENABLED, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetSfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
            PlayerPrefs.SetInt(GameConstants.PREF_SFX_ENABLED, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetVibrationEnabled(bool enabled)
        {
            VibrationEnabled = enabled;
            PlayerPrefs.SetInt(GameConstants.PREF_VIBRATION_ENABLED, enabled ? 1 : 0);
            PlayerPrefs.Save();
        }

        public void SetQualityLevel(int level)
        {
            QualityLevel = Mathf.Clamp(level, 0, 2);
            PlayerPrefs.SetInt(GameConstants.PREF_QUALITY_LEVEL, QualityLevel);
            QualitySettings.SetQualityLevel(QualityLevel);
            PlayerPrefs.Save();
        }

        public bool UpgradeMagnet()
        {
            int cost = MagnetLevel * 300;
            if (SpendCoins(cost))
            {
                MagnetLevel++;
                PlayerPrefs.SetInt(GameConstants.PREF_MAGNET_LEVEL, MagnetLevel);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        public bool UpgradeShield()
        {
            int cost = ShieldLevel * 350;
            if (SpendCoins(cost))
            {
                ShieldLevel++;
                PlayerPrefs.SetInt(GameConstants.PREF_SHIELD_LEVEL, ShieldLevel);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }

        public bool UpgradeMultiplier()
        {
            int cost = MultiplierLevel * 400;
            if (SpendCoins(cost))
            {
                MultiplierLevel++;
                PlayerPrefs.SetInt(GameConstants.PREF_MULTIPLIER_LEVEL, MultiplierLevel);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }
    }
}
