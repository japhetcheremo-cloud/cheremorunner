using UnityEngine;
using System;
using System.Collections;
using CheremoRunner.Player;
using CheremoRunner.Audio;

namespace CheremoRunner.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event Action<GameState> OnGameStateChanged;
        public static event Action<int> OnScoreUpdated;
        public static event Action<int> OnCoinsUpdated;
        public static event Action<float> OnDistanceUpdated;
        public static event Action<PowerupType, float> OnPowerupTimerUpdated;
        public static event Action<PowerupType> OnPowerupDeactivated;

        [Header("Scene References")]
        [SerializeField] private PlayerController player;
        [SerializeField] private Camera.CameraFollow cameraFollow;
        [SerializeField] private Environment.TrackSpawner trackSpawner;

        public GameState CurrentState { get; private set; } = GameState.SplashScreen;
        public int CurrentScore { get; private set; }
        public int CoinsThisRun { get; private set; }
        public float DistanceRun { get; private set; }
        public int ScoreMultiplier { get; private set; } = 1;

        // Powerup timers
        private float magnetTimer = 0f;
        private float shieldTimer = 0f;
        private float multiplierTimer = 0f;

        public bool IsMagnetActive => magnetTimer > 0f;
        public bool IsShieldActive => shieldTimer > 0f;
        public bool IsMultiplierActive => multiplierTimer > 0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            SetState(GameState.SplashScreen);
            StartCoroutine(SplashScreenSequence());
        }

        private IEnumerator SplashScreenSequence()
        {
            yield return new WaitForSeconds(1.8f);
            SetState(GameState.LoadingScreen);
            yield return new WaitForSeconds(1.2f);
            SetState(GameState.MainMenu);
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 1.0f;
                    break;
                default:
                    Time.timeScale = 1.0f;
                    break;
            }
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing && player != null && !player.IsDead)
            {
                DistanceRun = player.DistanceTravelled;
                OnDistanceUpdated?.Invoke(DistanceRun);

                // Score formula: Distance * Multiplier + (Coins * 10)
                int calculatedScore = Mathf.FloorToInt(DistanceRun * (IsMultiplierActive ? 2 : 1)) + (CoinsThisRun * 10);
                if (calculatedScore != CurrentScore)
                {
                    CurrentScore = calculatedScore;
                    OnScoreUpdated?.Invoke(CurrentScore);
                }

                UpdatePowerups(Time.deltaTime);
            }
        }

        private void UpdatePowerups(float dt)
        {
            if (magnetTimer > 0f)
            {
                magnetTimer -= dt;
                OnPowerupTimerUpdated?.Invoke(PowerupType.CoinMagnet, magnetTimer);
                if (magnetTimer <= 0f)
                {
                    OnPowerupDeactivated?.Invoke(PowerupType.CoinMagnet);
                }
            }

            if (shieldTimer > 0f)
            {
                shieldTimer -= dt;
                OnPowerupTimerUpdated?.Invoke(PowerupType.Shield, shieldTimer);
                if (shieldTimer <= 0f)
                {
                    player?.DeactivateShield();
                    OnPowerupDeactivated?.Invoke(PowerupType.Shield);
                }
            }

            if (multiplierTimer > 0f)
            {
                multiplierTimer -= dt;
                OnPowerupTimerUpdated?.Invoke(PowerupType.ScoreMultiplier2X, multiplierTimer);
                if (multiplierTimer <= 0f)
                {
                    ScoreMultiplier = 1;
                    OnPowerupDeactivated?.Invoke(PowerupType.ScoreMultiplier2X);
                }
            }
        }

        public void StartGame()
        {
            CurrentScore = 0;
            CoinsThisRun = 0;
            DistanceRun = 0f;
            ScoreMultiplier = 1;
            magnetTimer = 0f;
            shieldTimer = 0f;
            multiplierTimer = 0f;

            if (player != null)
            {
                player.ResetPlayer();
            }

            if (trackSpawner != null)
            {
                trackSpawner.ResetTrack();
            }

            SetState(GameState.Playing);
            AudioManager.Instance?.PlayBGM();
        }

        public void PauseGame()
        {
            if (CurrentState == GameState.Playing)
            {
                SetState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            if (CurrentState == GameState.Paused)
            {
                SetState(GameState.Playing);
            }
        }

        public void RestartGame()
        {
            StartGame();
        }

        public void GoToMainMenu()
        {
            SetState(GameState.MainMenu);
        }

        public void OpenCharacterSelect()
        {
            SetState(GameState.CharacterSelect);
        }

        public void OpenShop()
        {
            SetState(GameState.Shop);
        }

        public void OpenAchievements()
        {
            SetState(GameState.Achievements);
        }

        public void OpenSettings()
        {
            SetState(GameState.Settings);
        }

        public void AddCoin()
        {
            int earned = IsMultiplierActive ? 2 : 1;
            CoinsThisRun += earned;
            SaveManager.Instance?.AddCoins(earned);
            OnCoinsUpdated?.Invoke(CoinsThisRun);
        }

        public void ActivatePowerup(PowerupType type)
        {
            float duration = 8.0f;
            switch (type)
            {
                case PowerupType.CoinMagnet:
                    int magLevel = SaveManager.Instance != null ? SaveManager.Instance.MagnetLevel : 1;
                    duration = GameConstants.BASE_MAGNET_DURATION + (magLevel - 1) * GameConstants.DURATION_PER_UPGRADE_LEVEL;
                    magnetTimer = duration;
                    break;

                case PowerupType.Shield:
                    int shldLevel = SaveManager.Instance != null ? SaveManager.Instance.ShieldLevel : 1;
                    duration = GameConstants.BASE_SHIELD_DURATION + (shldLevel - 1) * GameConstants.DURATION_PER_UPGRADE_LEVEL;
                    shieldTimer = duration;
                    player?.ActivateShield();
                    break;

                case PowerupType.ScoreMultiplier2X:
                    int multLevel = SaveManager.Instance != null ? SaveManager.Instance.MultiplierLevel : 1;
                    duration = GameConstants.BASE_MULTIPLIER_DURATION + (multLevel - 1) * GameConstants.DURATION_PER_UPGRADE_LEVEL;
                    multiplierTimer = duration;
                    ScoreMultiplier = 2;
                    break;
            }
        }

        public void OnPlayerDied()
        {
            SaveManager.Instance?.SaveScoreIfHigh(CurrentScore);
            StartCoroutine(GameOverSequence());
        }

        private IEnumerator GameOverSequence()
        {
            cameraFollow?.TriggerShake(0.4f, 0.5f);
            yield return new WaitForSeconds(1.2f);
            AudioManager.Instance?.PlayGameOverSound();
            SetState(GameState.GameOver);
        }
    }
}
