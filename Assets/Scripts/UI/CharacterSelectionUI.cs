using UnityEngine;
using UnityEngine.UI;
using CheremoRunner.Core;
using CheremoRunner.Player;
using CheremoRunner.Audio;

namespace CheremoRunner.UI
{
    public class CharacterSelectionUI : MonoBehaviour
    {
        [Header("Preview Target")]
        [SerializeField] private Transform previewPlatform;
        [SerializeField] private float rotationSpeed = 35f;

        [Header("UI Text")]
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterTaglineText;
        [SerializeField] private Text perkText;
        [SerializeField] private Text coinBalanceText;
        [SerializeField] private Button actionButton;
        [SerializeField] private Text actionButtonText;

        private int currentIndex = 0;
        private GameObject currentPreviewModel;

        private void OnEnable()
        {
            if (SaveManager.Instance != null)
            {
                currentIndex = SaveManager.Instance.SelectedSkinId;
            }
            UpdateDisplay();
        }

        private void Update()
        {
            if (currentPreviewModel != null)
            {
                currentPreviewModel.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }

        public void OnClickNext()
        {
            AudioManager.Instance?.PlayClickSound();
            if (CharacterModelSwitcher.Instance == null) return;
            currentIndex = (currentIndex + 1) % CharacterModelSwitcher.Instance.skins.Count;
            UpdateDisplay();
        }

        public void OnClickPrevious()
        {
            AudioManager.Instance?.PlayClickSound();
            if (CharacterModelSwitcher.Instance == null) return;
            currentIndex = (currentIndex - 1 + CharacterModelSwitcher.Instance.skins.Count) % CharacterModelSwitcher.Instance.skins.Count;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (CharacterModelSwitcher.Instance == null || SaveManager.Instance == null) return;

            CharacterSkinData skin = CharacterModelSwitcher.Instance.GetSkin(currentIndex);
            if (skin == null) return;

            if (characterNameText != null) characterNameText.text = skin.name;
            if (characterTaglineText != null) characterTaglineText.text = skin.tagline;
            if (perkText != null) perkText.text = $"+{skin.speedBonusPercent}% SPEED  |  +{skin.coinBonusPercent}% COINS";
            if (coinBalanceText != null) coinBalanceText.text = SaveManager.Instance.TotalCoins.ToString();

            bool isUnlocked = SaveManager.Instance.IsSkinUnlocked(skin.id);
            bool isEquipped = SaveManager.Instance.SelectedSkinId == skin.id;

            if (actionButtonText != null)
            {
                if (isEquipped)
                {
                    actionButtonText.text = "EQUIPPED";
                    if (actionButton != null) actionButton.interactable = false;
                }
                else if (isUnlocked)
                {
                    actionButtonText.text = "SELECT";
                    if (actionButton != null) actionButton.interactable = true;
                }
                else
                {
                    actionButtonText.text = $"UNLOCK ({skin.coinPrice} COINS)";
                    if (actionButton != null) actionButton.interactable = SaveManager.Instance.TotalCoins >= skin.coinPrice;
                }
            }

            // Spawn or update preview 3D model
            UpdatePreviewModel(skin);
        }

        private void UpdatePreviewModel(CharacterSkinData skin)
        {
            if (previewPlatform == null) return;

            if (currentPreviewModel != null)
            {
                Destroy(currentPreviewModel);
            }

            currentPreviewModel = Gameplay.ProceduralMeshBuilder.CreateRunnerCharacter(skin.primaryColor, skin.accentColor, skin.glowColor);
            currentPreviewModel.transform.SetParent(previewPlatform);
            currentPreviewModel.transform.localPosition = Vector3.zero;
            currentPreviewModel.transform.localRotation = Quaternion.Euler(0, 180, 0);

            // Disable player control on preview model
            var cc = currentPreviewModel.GetComponent<CharacterController>();
            if (cc != null) Destroy(cc);
        }

        public void OnClickAction()
        {
            if (CharacterModelSwitcher.Instance == null || SaveManager.Instance == null) return;

            CharacterSkinData skin = CharacterModelSwitcher.Instance.GetSkin(currentIndex);
            if (skin == null) return;

            bool isUnlocked = SaveManager.Instance.IsSkinUnlocked(skin.id);

            if (isUnlocked)
            {
                SaveManager.Instance.SelectSkin(skin.id);
                AudioManager.Instance?.PlayClickSound();
            }
            else
            {
                if (SaveManager.Instance.SpendCoins(skin.coinPrice))
                {
                    SaveManager.Instance.UnlockSkin(skin.id);
                    SaveManager.Instance.SelectSkin(skin.id);
                    AudioManager.Instance?.PlayPowerupSound();
                }
            }

            UpdateDisplay();
        }

        public void OnClickBack()
        {
            AudioManager.Instance?.PlayClickSound();
            if (currentPreviewModel != null)
            {
                Destroy(currentPreviewModel);
            }
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
