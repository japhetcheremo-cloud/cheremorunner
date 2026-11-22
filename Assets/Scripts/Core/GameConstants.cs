using UnityEngine;

namespace CheremoRunner.Core
{
    public static class GameConstants
    {
        // Lane positioning
        public const float LANE_DISTANCE = 2.4f;
        public const float LANE_LEFT = -LANE_DISTANCE;
        public const float LANE_MIDDLE = 0.0f;
        public const float LANE_RIGHT = LANE_DISTANCE;

        // Player physics & speed
        public const float INITIAL_FORWARD_SPEED = 11.0f;
        public const float MAX_FORWARD_SPEED = 26.0f;
        public const float SPEED_ACCELERATION_RATE = 0.12f;
        public const float LANE_CHANGE_SPEED = 16.0f;
        public const float JUMP_FORCE = 10.5f;
        public const float GRAVITY = 30.0f;
        public const float SLIDE_DURATION = 0.75f;
        public const float FAST_FALL_GRAVITY_MULTIPLIER = 2.5f;

        // Track and spawning
        public const float TRACK_SEGMENT_LENGTH = 32.0f;
        public const int INITIAL_TRACK_SEGMENTS = 7;
        public const float DESPAWN_DISTANCE_BEHIND_PLAYER = 40.0f;

        // Tags
        public const string TAG_PLAYER = "Player";
        public const string TAG_OBSTACLE = "Obstacle";
        public const string TAG_COIN = "Coin";
        public const string TAG_POWERUP = "Powerup";
        public const string TAG_GROUND = "Ground";

        // Save keys
        public const string PREF_HIGH_SCORE = "Cheremo_HighScore";
        public const string PREF_TOTAL_COINS = "Cheremo_TotalCoins";
        public const string PREF_SELECTED_SKIN = "Cheremo_SelectedSkin";
        public const string PREF_UNLOCKED_SKINS = "Cheremo_UnlockedSkins";
        public const string PREF_MUSIC_ENABLED = "Cheremo_MusicEnabled";
        public const string PREF_SFX_ENABLED = "Cheremo_SfxEnabled";
        public const string PREF_VIBRATION_ENABLED = "Cheremo_VibrationEnabled";
        public const string PREF_QUALITY_LEVEL = "Cheremo_QualityLevel";
        public const string PREF_MAGNET_LEVEL = "Cheremo_MagnetLevel";
        public const string PREF_SHIELD_LEVEL = "Cheremo_ShieldLevel";
        public const string PREF_MULTIPLIER_LEVEL = "Cheremo_MultiplierLevel";
        public const string PREF_ACHIEVEMENTS = "Cheremo_Achievements";

        // Powerup durations (Base seconds)
        public const float BASE_MAGNET_DURATION = 8.0f;
        public const float BASE_SHIELD_DURATION = 10.0f;
        public const float BASE_MULTIPLIER_DURATION = 10.0f;
        public const float DURATION_PER_UPGRADE_LEVEL = 2.0f;
        public const float MAGNET_ATTRACT_RADIUS = 12.0f;
        public const float MAGNET_ATTRACT_SPEED = 18.0f;
    }

    public enum LaneIndex
    {
        Left = 0,
        Middle = 1,
        Right = 2
    }

    public enum ObstacleType
    {
        BarrierJump,     // Low barrier: player can jump over
        BarrierSlide,    // Overhead sign/laser: player can slide under
        Roadblock,       // Tall barrier: must switch lanes
        MovingDrone      // Moving vehicle/drone: moves towards player
    }

    public enum PowerupType
    {
        CoinMagnet,
        Shield,
        ScoreMultiplier2X
    }

    public enum GameState
    {
        SplashScreen,
        LoadingScreen,
        MainMenu,
        CharacterSelect,
        Shop,
        Achievements,
        Settings,
        Playing,
        Paused,
        GameOver
    }

    [System.Serializable]
    public class CharacterSkinData
    {
        public int id;
        public string name;
        public string tagline;
        public int coinPrice;
        public bool isUnlockedByDefault;
        public Color primaryColor;
        public Color accentColor;
        public Color glowColor;
        public float speedBonusPercent;
        public float coinBonusPercent;
    }
}
