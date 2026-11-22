using UnityEngine;
using UnityEngine.UI;
using CheremoRunner.Core;
using CheremoRunner.Audio;

namespace CheremoRunner.UI
{
    public class ShopUI : MonoBehaviour
    {
        [Header("Coin Balance")]
        [SerializeField] private Text coinBalanceText;

        [Header("Powerup Upgrades")]
        [SerializeField] private Text magnetLevelText;
        [SerializeField] private Text magnetCostText;
        [SerializeField] private Text shieldLevelText;
        [SerializeField] private Text shieldCostText;
        [SerializeField] private Text multiplierLevelText;
        [SerializeField] private Text multiplierCostText;

        private void OnEnable()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            if (SaveManager.Instance == null) return;

            if (coinBalanceText != null)
                coinBalanceText.text = SaveManager.Instance.TotalCoins.ToString();

            // Magnet
            int mLevel = SaveManager.Instance.MagnetLevel;
            if (magnetLevelText != null) magnetLevelText.text = "LVL " + mLevel;
            if (magnetCostText != null) magnetCostText.text = (mLevel * 300) + " COINS";

            // Shield
            int sLevel = SaveManager.Instance.ShieldLevel;
            if (shieldLevelText != null) shieldLevelText.text = "LVL " + sLevel;
            if (shieldCostText != null) shieldCostText.text = (sLevel * 350) + " COINS";

            // Multiplier
            int multLevel = SaveManager.Instance.MultiplierLevel;
            if (multiplierLevelText != null) multiplierLevelText.text = "LVL " + multLevel;
            if (multiplierCostText != null) multiplierCostText.text = (multLevel * 400) + " COINS";
        }

        public void OnClickUpgradeMagnet()
        {
            AudioManager.Instance?.PlayClickSound();
            if (SaveManager.Instance != null && SaveManager.Instance.UpgradeMagnet())
            {
                AudioManager.Instance?.PlayPowerupSound();
                RefreshUI();
            }
        }

        public void OnClickUpgradeShield()
        {
            AudioManager.Instance?.PlayClickSound();
            if (SaveManager.Instance != null && SaveManager.Instance.UpgradeShield())
            {
                AudioManager.Instance?.PlayPowerupSound();
                RefreshUI();
            }
        }

        public void OnClickUpgradeMultiplier()
        {
            AudioManager.Instance?.PlayClickSound();
            if (SaveManager.Instance != null && SaveManager.Instance.UpgradeMultiplier())
            {
                AudioManager.Instance?.PlayPowerupSound();
                RefreshUI();
            }
        }

        public void OnClickBack()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
