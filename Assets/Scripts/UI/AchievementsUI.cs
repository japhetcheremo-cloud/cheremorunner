using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CheremoRunner.Core;
using CheremoRunner.Audio;

namespace CheremoRunner.UI
{
    public class AchievementsUI : MonoBehaviour
    {
        [System.Serializable]
        public class Achievement
        {
            public string id;
            public string title;
            public string description;
            public int targetValue;
            public int rewardCoins;
        }

        [Header("Achievements Data")]
        public List<Achievement> achievements = new List<Achievement>();

        [Header("UI Container")]
        [SerializeField] private Transform listContainer;
        [SerializeField] private Text statusSummaryText;

        private void Awake()
        {
            InitializeAchievements();
        }

        private void InitializeAchievements()
        {
            achievements.Clear();
            achievements.Add(new Achievement { id = "ach_first_step", title = "FIRST SPRINT", description = "Complete your first run.", targetValue = 1, rewardCoins = 50 });
            achievements.Add(new Achievement { id = "ach_dist_100", title = "STREET ROOKIE", description = "Run 100 meters in a single run.", targetValue = 100, rewardCoins = 100 });
            achievements.Add(new Achievement { id = "ach_dist_1000", title = "CITY EXPLORER", description = "Run 1,000 meters in a single run.", targetValue = 1000, rewardCoins = 300 });
            achievements.Add(new Achievement { id = "ach_dist_5000", title = "MARATHON CHAMPION", description = "Run 5,000 meters in a single run.", targetValue = 5000, rewardCoins = 1000 });
            achievements.Add(new Achievement { id = "ach_coins_100", title = "COIN COLLECTOR", description = "Accumulate 100 total coins.", targetValue = 100, rewardCoins = 150 });
            achievements.Add(new Achievement { id = "ach_coins_1000", title = "GOLD HOARDER", description = "Accumulate 1,000 total coins.", targetValue = 1000, rewardCoins = 500 });
            achievements.Add(new Achievement { id = "ach_skins_2", title = "FASHION RUNNER", description = "Unlock at least 2 character skins.", targetValue = 2, rewardCoins = 400 });
        }

        private void OnEnable()
        {
            RefreshAchievements();
        }

        public void RefreshAchievements()
        {
            if (SaveManager.Instance == null) return;

            int completedCount = 0;
            for (int i = 0; i < achievements.Count; i++)
            {
                if (IsAchievementCompleted(achievements[i]))
                {
                    completedCount++;
                }
            }

            if (statusSummaryText != null)
            {
                statusSummaryText.text = $"COMPLETED: {completedCount} / {achievements.Count}";
            }
        }

        public bool IsAchievementCompleted(Achievement ach)
        {
            if (SaveManager.Instance == null) return false;

            switch (ach.id)
            {
                case "ach_first_step":
                    return SaveManager.Instance.HighScore > 0;
                case "ach_dist_100":
                    return SaveManager.Instance.HighScore >= 100;
                case "ach_dist_1000":
                    return SaveManager.Instance.HighScore >= 1000;
                case "ach_dist_5000":
                    return SaveManager.Instance.HighScore >= 5000;
                case "ach_coins_100":
                    return SaveManager.Instance.TotalCoins >= 100;
                case "ach_coins_1000":
                    return SaveManager.Instance.TotalCoins >= 1000;
                case "ach_skins_2":
                    return SaveManager.Instance.IsSkinUnlocked(1) || SaveManager.Instance.IsSkinUnlocked(2) || SaveManager.Instance.IsSkinUnlocked(3);
                default:
                    return false;
            }
        }

        public void OnClickBack()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
