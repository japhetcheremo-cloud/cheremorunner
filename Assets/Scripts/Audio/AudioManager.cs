using UnityEngine;

namespace CheremoRunner.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip bgmClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip slideClip;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip crashClip;
        [SerializeField] private AudioClip clickClip;
        [SerializeField] private AudioClip powerupClip;
        [SerializeField] private AudioClip gameOverClip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = 0.65f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = 0.85f;
            }

            // Load audio clips from Resources or Assets if not assigned
            if (bgmClip == null) bgmClip = Resources.Load<AudioClip>("Audio/bgm_runner");
            if (jumpClip == null) jumpClip = Resources.Load<AudioClip>("Audio/sfx_jump");
            if (slideClip == null) slideClip = Resources.Load<AudioClip>("Audio/sfx_slide");
            if (coinClip == null) coinClip = Resources.Load<AudioClip>("Audio/sfx_coin");
            if (crashClip == null) crashClip = Resources.Load<AudioClip>("Audio/sfx_crash");
            if (clickClip == null) clickClip = Resources.Load<AudioClip>("Audio/sfx_click");
            if (powerupClip == null) powerupClip = Resources.Load<AudioClip>("Audio/sfx_powerup");
            if (gameOverClip == null) gameOverClip = Resources.Load<AudioClip>("Audio/sfx_gameover");
        }

        private void Start()
        {
            PlayBGM();
        }

        public void PlayBGM()
        {
            if (musicSource != null && bgmClip != null)
            {
                musicSource.clip = bgmClip;
                bool isEnabled = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.MusicEnabled : true;
                musicSource.mute = !isEnabled;
                if (!musicSource.isPlaying && isEnabled)
                {
                    musicSource.Play();
                }
            }
        }

        public void SetMusicMute(bool mute)
        {
            if (musicSource != null)
            {
                musicSource.mute = mute;
                if (!mute && !musicSource.isPlaying)
                {
                    musicSource.Play();
                }
            }
        }

        public void PlaySFX(AudioClip clip, float pitch = 1.0f)
        {
            if (clip == null || sfxSource == null) return;
            bool isEnabled = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.SfxEnabled : true;
            if (!isEnabled) return;

            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip);
        }

        public void PlayJumpSound() => PlaySFX(jumpClip);
        public void PlaySlideSound() => PlaySFX(slideClip);
        public void PlayCoinSound()
        {
            // Subtle pitch variation for rapid coin pickups
            float p = Random.Range(0.96f, 1.08f);
            PlaySFX(coinClip, p);
        }
        public void PlayCrashSound() => PlaySFX(crashClip);
        public void PlayClickSound() => PlaySFX(clickClip);
        public void PlayPowerupSound() => PlaySFX(powerupClip);
        public void PlayGameOverSound() => PlaySFX(gameOverClip);

        public void TriggerHaptic(long milliseconds = 40)
        {
            bool vibEnabled = Core.SaveManager.Instance != null ? Core.SaveManager.Instance.VibrationEnabled : true;
            if (!vibEnabled) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                Handheld.Vibrate();
            }
            catch {}
#endif
        }
    }
}
