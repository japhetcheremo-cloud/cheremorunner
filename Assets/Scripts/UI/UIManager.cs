using UnityEngine;
using UnityEngine.UI;
using CheremoRunner.Core;
using CheremoRunner.Audio;

namespace CheremoRunner.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Screens")]
        public GameObject splashScreen;
        public GameObject loadingScreen;
        public GameObject mainMenu;
        public GameObject characterSelectScreen;
        public GameObject shopScreen;
        public GameObject gameplayHUD;
        public GameObject pauseMenu;
        public GameObject gameOverScreen;
        public GameObject settingsScreen;
        public GameObject achievementsScreen;

        [Header("HUD Elements")]
        public Text hudScoreText;
        public Text hudCoinsText;
        public Text hudDistanceText;
        public Text hudMultiplierText;

        [Header("Game Over Elements")]
        public Text gameOverScoreText;
        public Text gameOverBestScoreText;
        public Text gameOverCoinsText;
        public Text gameOverDistanceText;

        [Header("Main Menu Elements")]
        public Text menuHighScoreText;
        public Text menuTotalCoinsText;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }
        }

        private void Start()
        {
            GameManager.OnGameStateChanged += HandleGameStateChanged;
            GameManager.OnScoreUpdated += UpdateHUDScore;
            GameManager.OnCoinsUpdated += UpdateHUDCoins;
            GameManager.OnDistanceUpdated += UpdateHUDDistance;

            UpdateMenuStats();
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= HandleGameStateChanged;
            GameManager.OnScoreUpdated -= UpdateHUDScore;
            GameManager.OnCoinsUpdated -= UpdateHUDCoins;
            GameManager.OnDistanceUpdated -= UpdateHUDDistance;
        }

        private void HandleGameStateChanged(GameState state)
        {
            HideAllScreens();

            switch (state)
            {
                case GameState.SplashScreen:
                    if (splashScreen != null) splashScreen.SetActive(true);
                    break;
                case GameState.LoadingScreen:
                    if (loadingScreen != null) loadingScreen.SetActive(true);
                    break;
                case GameState.MainMenu:
                    if (mainMenu != null) mainMenu.SetActive(true);
                    UpdateMenuStats();
                    break;
                case GameState.Playing:
                    if (gameplayHUD != null) gameplayHUD.SetActive(true);
                    break;
                case GameState.Paused:
                    if (gameplayHUD != null) gameplayHUD.SetActive(true);
                    if (pauseMenu != null) pauseMenu.SetActive(true);
                    break;
                case GameState.GameOver:
                    if (gameOverScreen != null) gameOverScreen.SetActive(true);
                    UpdateGameOverStats();
                    break;
                case GameState.CharacterSelect:
                    if (characterSelectScreen != null) characterSelectScreen.SetActive(true);
                    break;
                case GameState.Shop:
                    if (shopScreen != null) shopScreen.SetActive(true);
                    break;
                case GameState.Settings:
                    if (settingsScreen != null) settingsScreen.SetActive(true);
                    break;
                case GameState.Achievements:
                    if (achievementsScreen != null) achievementsScreen.SetActive(true);
                    break;
            }
        }

        private void HideAllScreens()
        {
            if (splashScreen != null) splashScreen.SetActive(false);
            if (loadingScreen != null) loadingScreen.SetActive(false);
            if (mainMenu != null) mainMenu.SetActive(false);
            if (characterSelectScreen != null) characterSelectScreen.SetActive(false);
            if (shopScreen != null) shopScreen.SetActive(false);
            if (gameplayHUD != null) gameplayHUD.SetActive(false);
            if (pauseMenu != null) pauseMenu.SetActive(false);
            if (gameOverScreen != null) gameOverScreen.SetActive(false);
            if (settingsScreen != null) settingsScreen.SetActive(false);
            if (achievementsScreen != null) achievementsScreen.SetActive(false);
        }

        private void UpdateHUDScore(int score)
        {
            if (hudScoreText != null) hudScoreText.text = score.ToString();
        }

        private void UpdateHUDCoins(int coins)
        {
            if (hudCoinsText != null) hudCoinsText.text = coins.ToString();
        }

        private void UpdateHUDDistance(float distance)
        {
            if (hudDistanceText != null) hudDistanceText.text = Mathf.FloorToInt(distance) + "m";
        }

        public void UpdateMenuStats()
        {
            if (SaveManager.Instance != null)
            {
                if (menuHighScoreText != null)
                    menuHighScoreText.text = "BEST: " + SaveManager.Instance.HighScore;
                if (menuTotalCoinsText != null)
                    menuTotalCoinsText.text = SaveManager.Instance.TotalCoins.ToString();
            }
        }

        private void UpdateGameOverStats()
        {
            if (GameManager.Instance != null)
            {
                if (gameOverScoreText != null)
                    gameOverScoreText.text = GameManager.Instance.CurrentScore.ToString();
                if (gameOverCoinsText != null)
                    gameOverCoinsText.text = "+" + GameManager.Instance.CoinsThisRun;
                if (gameOverDistanceText != null)
                    gameOverDistanceText.text = Mathf.FloorToInt(GameManager.Instance.DistanceRun) + "m";
            }
            if (SaveManager.Instance != null && gameOverBestScoreText != null)
            {
                gameOverBestScoreText.text = "BEST: " + SaveManager.Instance.HighScore;
            }
        }

        #region UI Button Handlers

        public void OnClickPlay()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.StartGame();
        }

        public void OnClickCharacters()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.OpenCharacterSelect();
        }

        public void OnClickShop()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.OpenShop();
        }

        public void OnClickAchievements()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.OpenAchievements();
        }

        public void OnClickSettings()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.OpenSettings();
        }

        public void OnClickPause()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.PauseGame();
        }

        public void OnClickResume()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.ResumeGame();
        }

        public void OnClickRestart()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.RestartGame();
        }

        public void OnClickMainMenu()
        {
            AudioManager.Instance?.PlayClickSound();
            GameManager.Instance?.GoToMainMenu();
        }

        #endregion
    }
}
