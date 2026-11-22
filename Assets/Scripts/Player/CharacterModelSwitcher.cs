using UnityEngine;
using System.Collections.Generic;
using CheremoRunner.Core;
using CheremoRunner.Gameplay;

namespace CheremoRunner.Player
{
    public class CharacterModelSwitcher : MonoBehaviour
    {
        public static CharacterModelSwitcher Instance { get; private set; }

        public List<CharacterSkinData> skins = new List<CharacterSkinData>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else { Destroy(gameObject); return; }

            InitializeSkins();
        }

        private void InitializeSkins()
        {
            skins.Clear();

            // Skin 0: Cheremo Classic (Default)
            skins.Add(new CharacterSkinData
            {
                id = 0,
                name = "CHEREMO CLASSIC",
                tagline = "The original champion runner.",
                coinPrice = 0,
                isUnlockedByDefault = true,
                primaryColor = new Color(0.08f, 0.38f, 0.74f), // Electric Blue
                accentColor = new Color(0.92f, 0.94f, 0.98f), // Crisp White
                glowColor = new Color(1.0f, 0.55f, 0.0f),    // Blaze Orange Visor
                speedBonusPercent = 0.0f,
                coinBonusPercent = 0.0f
            });

            // Skin 1: Cyber Phantom
            skins.Add(new CharacterSkinData
            {
                id = 1,
                name = "CYBER PHANTOM",
                tagline = "Neon-infused high-speed cyber suit.",
                coinPrice = 500,
                isUnlockedByDefault = false,
                primaryColor = new Color(0.12f, 0.12f, 0.15f), // Carbon Black
                accentColor = new Color(0.0f, 0.95f, 1.0f),    // Cyan Neon
                glowColor = new Color(0.0f, 1.0f, 0.85f),    // Mint Glow
                speedBonusPercent = 5.0f,
                coinBonusPercent = 5.0f
            });

            // Skin 2: Gold Rush
            skins.Add(new CharacterSkinData
            {
                id = 2,
                name = "GOLD RUSH",
                tagline = "Pure gilded prestige for elite runners.",
                coinPrice = 1500,
                isUnlockedByDefault = false,
                primaryColor = new Color(0.95f, 0.78f, 0.15f), // Gold
                accentColor = new Color(1.0f, 0.92f, 0.6f),    // Light Gold
                glowColor = new Color(1.0f, 0.84f, 0.0f),    // Radiant Gold
                speedBonusPercent = 5.0f,
                coinBonusPercent = 15.0f
            });

            // Skin 3: Urban Shadow
            skins.Add(new CharacterSkinData
            {
                id = 3,
                name = "URBAN SHADOW",
                tagline = "Silent street ninja with crimson optics.",
                coinPrice = 3000,
                isUnlockedByDefault = false,
                primaryColor = new Color(0.05f, 0.06f, 0.08f), // Deep Stealth
                accentColor = new Color(0.25f, 0.27f, 0.30f), // Gunmetal
                glowColor = new Color(1.0f, 0.15f, 0.20f),    // Crimson Visor
                speedBonusPercent = 10.0f,
                coinBonusPercent = 10.0f
            });
        }

        public CharacterSkinData GetSkin(int id)
        {
            if (id >= 0 && id < skins.Count) return skins[id];
            return skins[0];
        }

        public void ApplySkinToCharacter(GameObject characterObj, int skinId)
        {
            CharacterSkinData skin = GetSkin(skinId);
            if (characterObj == null || skin == null) return;

            Transform torso = characterObj.transform.Find("Torso");
            if (torso != null) ProceduralMeshBuilder.SetMeshColor(torso.gameObject, skin.primaryColor);

            Transform leftArm = characterObj.transform.Find("LeftArm");
            if (leftArm != null) ProceduralMeshBuilder.SetMeshColor(leftArm.gameObject, skin.accentColor);

            Transform rightArm = characterObj.transform.Find("RightArm");
            if (rightArm != null) ProceduralMeshBuilder.SetMeshColor(rightArm.gameObject, skin.accentColor);

            Transform leftLeg = characterObj.transform.Find("LeftLeg");
            if (leftLeg != null) ProceduralMeshBuilder.SetMeshColor(leftLeg.gameObject, skin.primaryColor * 0.75f);

            Transform rightLeg = characterObj.transform.Find("RightLeg");
            if (rightLeg != null) ProceduralMeshBuilder.SetMeshColor(rightLeg.gameObject, skin.primaryColor * 0.75f);

            Transform head = characterObj.transform.Find("Head");
            if (head != null)
            {
                Transform visor = head.Find("Visor");
                if (visor != null)
                {
                    ProceduralMeshBuilder.SetMeshColor(visor.gameObject, skin.glowColor);
                    ProceduralMeshBuilder.SetEmission(visor.gameObject, skin.glowColor, 2.0f);
                }
            }

            Transform leftShoe = characterObj.transform.Find("LeftShoe");
            if (leftShoe != null) ProceduralMeshBuilder.SetMeshColor(leftShoe.gameObject, skin.glowColor);

            Transform rightShoe = characterObj.transform.Find("RightShoe");
            if (rightShoe != null) ProceduralMeshBuilder.SetMeshColor(rightShoe.gameObject, skin.glowColor);
        }
    }
}
