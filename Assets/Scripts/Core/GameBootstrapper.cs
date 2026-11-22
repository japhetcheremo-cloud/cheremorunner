using UnityEngine;
using CheremoRunner.Player;
using CheremoRunner.Environment;
using CheremoRunner.Audio;
using CheremoRunner.Gameplay;
using CheremoRunner.UI;
using CheremoRunner.Camera;

namespace CheremoRunner.Core
{
    public class GameBootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            // Set target framerate to 60 FPS for smooth mobile gameplay
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            EnsureCoreSystems();
        }

        private void EnsureCoreSystems()
        {
            // 1. Save Manager
            if (SaveManager.Instance == null)
            {
                GameObject saveObj = new GameObject("SaveManager");
                saveObj.AddComponent<SaveManager>();
            }

            // 2. Audio Manager
            if (AudioManager.Instance == null)
            {
                GameObject audioObj = new GameObject("AudioManager");
                audioObj.AddComponent<AudioManager>();
            }

            // 3. Object Pool
            if (ObjectPool.Instance == null)
            {
                GameObject poolObj = new GameObject("ObjectPool");
                poolObj.AddComponent<ObjectPool>();
            }

            // 4. Character Model Switcher
            if (CharacterModelSwitcher.Instance == null)
            {
                GameObject switcherObj = new GameObject("CharacterModelSwitcher");
                switcherObj.AddComponent<CharacterModelSwitcher>();
            }

            // 5. Player Character
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                int skinId = SaveManager.Instance != null ? SaveManager.Instance.SelectedSkinId : 0;
                var skinData = CharacterModelSwitcher.Instance != null ? CharacterModelSwitcher.Instance.GetSkin(skinId) : null;
                Color bodyCol = skinData != null ? skinData.primaryColor : new Color(0.08f, 0.38f, 0.74f);
                Color accCol = skinData != null ? skinData.accentColor : Color.white;
                Color glowCol = skinData != null ? skinData.glowColor : Color.cyan;

                GameObject playerObj = ProceduralMeshBuilder.CreateRunnerCharacter(bodyCol, accCol, glowCol);
                playerObj.transform.position = new Vector3(GameConstants.LANE_MIDDLE, 0, 0);
                player = playerObj.AddComponent<PlayerController>();
                playerObj.AddComponent<SwipeDetector>();
                playerObj.AddComponent<PlayerAnimationHelper>();
            }

            // 6. Camera Follow
            CameraFollow camFollow = FindObjectOfType<CameraFollow>();
            if (camFollow == null)
            {
                UnityEngine.Camera mainCam = UnityEngine.Camera.main;
                if (mainCam == null)
                {
                    GameObject camObj = new GameObject("Main Camera");
                    mainCam = camObj.AddComponent<UnityEngine.Camera>();
                    camObj.tag = "MainCamera";
                }
                camFollow = mainCam.gameObject.AddComponent<CameraFollow>();
            }
            camFollow.SetTarget(player.transform);

            // 7. Directional Light
            if (FindObjectOfType<Light>() == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                Light sun = lightObj.AddComponent<Light>();
                sun.type = LightType.Directional;
                sun.intensity = 1.15f;
                sun.color = new Color(1.0f, 0.96f, 0.90f);
                lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            }

            // 8. Track Spawner
            TrackSpawner spawner = FindObjectOfType<TrackSpawner>();
            if (spawner == null)
            {
                GameObject spawnerObj = new GameObject("TrackSpawner");
                spawner = spawnerObj.AddComponent<TrackSpawner>();
            }

            // 9. UI Builder
            UIBuilder uiBuilder = FindObjectOfType<UIBuilder>();
            if (uiBuilder == null)
            {
                GameObject uiObj = new GameObject("UI");
                uiBuilder = uiObj.AddComponent<UIBuilder>();
            }

            // 10. Game Manager
            if (GameManager.Instance == null)
            {
                GameObject gmObj = new GameObject("GameManager");
                GameManager gm = gmObj.AddComponent<GameManager>();
            }
        }
    }
}
