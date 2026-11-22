using UnityEngine;
using UnityEngine.UI;
using CheremoRunner.Core;

namespace CheremoRunner.UI
{
    public class UIBuilder : MonoBehaviour
    {
        [Header("Auto Construct UI if Missing")]
        [SerializeField] private bool autoConstruct = true;

        private void Awake()
        {
            if (autoConstruct && GetComponentInChildren<Canvas>() == null)
            {
                ConstructCompleteUI();
            }
        }

        public void ConstructCompleteUI()
        {
            // Create EventSystem if missing
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // Create Canvas
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(transform);
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f; // Match portrait width

            canvasObj.AddComponent<GraphicRaycaster>();

            UIManager uiMgr = GetComponent<UIManager>();
            if (uiMgr == null) uiMgr = gameObject.AddComponent<UIManager>();

            Font defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");

            // 1. Splash Screen
            GameObject splash = CreatePanel(canvasObj.transform, "SplashScreen", new Color(0.04f, 0.07f, 0.17f, 1f));
            CreateText(splash.transform, "Title", "CHEREMO\nRUNNER", 72, Color.cyan, new Vector2(0, 200), defaultFont);
            CreateText(splash.transform, "Tagline", "RUN . DODGE . COLLECT . CONQUER", 28, Color.white, new Vector2(0, 60), defaultFont);
            CreateText(splash.transform, "Brand", "DEVELOPED BY CHEREMO", 26, new Color(1f, 0.84f, 0f), new Vector2(0, -500), defaultFont);
            uiMgr.splashScreen = splash;

            // 2. Loading Screen
            GameObject loading = CreatePanel(canvasObj.transform, "LoadingScreen", new Color(0.06f, 0.09f, 0.18f, 1f));
            CreateText(loading.transform, "LoadText", "PREPARING 3D CITY TRACK...", 36, Color.white, new Vector2(0, 100), defaultFont);
            uiMgr.loadingScreen = loading;

            // 3. Main Menu
            GameObject menu = CreatePanel(canvasObj.transform, "MainMenu", new Color(0.04f, 0.06f, 0.14f, 0.95f));
            CreateText(menu.transform, "MenuTitle", "CHEREMO RUNNER", 64, new Color(1f, 0.84f, 0f), new Vector2(0, 550), defaultFont);
            uiMgr.menuHighScoreText = CreateText(menu.transform, "HighScore", "BEST: 0", 38, Color.white, new Vector2(0, 430), defaultFont);
            uiMgr.menuTotalCoinsText = CreateText(menu.transform, "Coins", "0 COINS", 34, new Color(1f, 0.84f, 0f), new Vector2(0, 360), defaultFont);

            CreateButton(menu.transform, "PlayBtn", "PLAY", new Vector2(0, 140), new Vector2(480, 110), new Color(0f, 0.75f, 0.95f), () => uiMgr.OnClickPlay(), defaultFont);
            CreateButton(menu.transform, "CharBtn", "CHARACTERS", new Vector2(0, 0), new Vector2(480, 95), new Color(0.18f, 0.22f, 0.35f), () => uiMgr.OnClickCharacters(), defaultFont);
            CreateButton(menu.transform, "ShopBtn", "SHOP", new Vector2(0, -120), new Vector2(480, 95), new Color(0.18f, 0.22f, 0.35f), () => uiMgr.OnClickShop(), defaultFont);
            CreateButton(menu.transform, "AchBtn", "ACHIEVEMENTS", new Vector2(0, -240), new Vector2(480, 95), new Color(0.18f, 0.22f, 0.35f), () => uiMgr.OnClickAchievements(), defaultFont);
            CreateButton(menu.transform, "SetBtn", "SETTINGS", new Vector2(0, -360), new Vector2(480, 95), new Color(0.18f, 0.22f, 0.35f), () => uiMgr.OnClickSettings(), defaultFont);
            uiMgr.mainMenu = menu;

            // 4. Gameplay HUD
            GameObject hud = CreatePanel(canvasObj.transform, "GameplayHUD", Color.clear);
            uiMgr.hudScoreText = CreateText(hud.transform, "Score", "0", 68, Color.white, new Vector2(0, 800), defaultFont);
            uiMgr.hudCoinsText = CreateText(hud.transform, "Coins", "0", 42, new Color(1f, 0.84f, 0f), new Vector2(360, 810), defaultFont);
            uiMgr.hudDistanceText = CreateText(hud.transform, "Distance", "0m", 38, Color.cyan, new Vector2(-360, 810), defaultFont);
            CreateButton(hud.transform, "PauseBtn", "||", new Vector2(440, 880), new Vector2(90, 90), new Color(0.2f, 0.2f, 0.3f, 0.8f), () => uiMgr.OnClickPause(), defaultFont);
            uiMgr.gameplayHUD = hud;

            // 5. Pause Menu
            GameObject pause = CreatePanel(canvasObj.transform, "PauseMenu", new Color(0, 0, 0, 0.85f));
            CreateText(pause.transform, "PauseTitle", "GAME PAUSED", 58, Color.white, new Vector2(0, 400), defaultFont);
            CreateButton(pause.transform, "ResumeBtn", "RESUME", new Vector2(0, 160), new Vector2(440, 100), Color.cyan, () => uiMgr.OnClickResume(), defaultFont);
            CreateButton(pause.transform, "RestartBtn", "RESTART", new Vector2(0, 30), new Vector2(440, 100), new Color(0.2f, 0.3f, 0.5f), () => uiMgr.OnClickRestart(), defaultFont);
            CreateButton(pause.transform, "MenuBtn", "MAIN MENU", new Vector2(0, -100), new Vector2(440, 100), new Color(0.2f, 0.3f, 0.5f), () => uiMgr.OnClickMainMenu(), defaultFont);
            uiMgr.pauseMenu = pause;

            // 6. Game Over Screen
            GameObject gameOver = CreatePanel(canvasObj.transform, "GameOverScreen", new Color(0.08f, 0.02f, 0.05f, 0.95f));
            CreateText(gameOver.transform, "OverTitle", "RUN TERMINATED", 64, new Color(1f, 0.2f, 0.25f), new Vector2(0, 500), defaultFont);
            uiMgr.gameOverScoreText = CreateText(gameOver.transform, "FinalScore", "0", 72, Color.white, new Vector2(0, 340), defaultFont);
            uiMgr.gameOverBestScoreText = CreateText(gameOver.transform, "BestScore", "BEST: 0", 38, new Color(1f, 0.84f, 0f), new Vector2(0, 240), defaultFont);
            uiMgr.gameOverCoinsText = CreateText(gameOver.transform, "CoinsCollected", "+0", 42, new Color(1f, 0.84f, 0f), new Vector2(0, 150), defaultFont);
            CreateButton(gameOver.transform, "ReplayBtn", "PLAY AGAIN", new Vector2(0, -120), new Vector2(480, 110), Color.cyan, () => uiMgr.OnClickRestart(), defaultFont);
            CreateButton(gameOver.transform, "OverMenuBtn", "MAIN MENU", new Vector2(0, -260), new Vector2(480, 95), new Color(0.2f, 0.3f, 0.5f), () => uiMgr.OnClickMainMenu(), defaultFont);
            uiMgr.gameOverScreen = gameOver;

            // 7. Character Selection Screen
            GameObject charSel = CreatePanel(canvasObj.transform, "CharacterSelection", new Color(0.04f, 0.06f, 0.14f, 0.95f));
            CreateText(charSel.transform, "Title", "SELECT RUNNER", 54, Color.white, new Vector2(0, 680), defaultFont);
            var charSelComp = charSel.AddComponent<CharacterSelectionUI>();
            CreateButton(charSel.transform, "BackBtn", "< BACK", new Vector2(-400, 850), new Vector2(180, 80), new Color(0.2f, 0.25f, 0.4f), () => charSelComp.OnClickBack(), defaultFont);
            uiMgr.characterSelectScreen = charSel;

            // 8. Shop Screen
            GameObject shop = CreatePanel(canvasObj.transform, "ShopScreen", new Color(0.04f, 0.06f, 0.14f, 0.95f));
            CreateText(shop.transform, "Title", "UPGRADE SHOP", 54, new Color(1f, 0.84f, 0f), new Vector2(0, 680), defaultFont);
            var shopComp = shop.AddComponent<ShopUI>();
            CreateButton(shop.transform, "BackBtn", "< BACK", new Vector2(-400, 850), new Vector2(180, 80), new Color(0.2f, 0.25f, 0.4f), () => shopComp.OnClickBack(), defaultFont);
            uiMgr.shopScreen = shop;

            // 9. Settings Screen
            GameObject settings = CreatePanel(canvasObj.transform, "SettingsScreen", new Color(0.04f, 0.06f, 0.14f, 0.95f));
            CreateText(settings.transform, "Title", "SETTINGS", 54, Color.white, new Vector2(0, 680), defaultFont);
            var settingsComp = settings.AddComponent<SettingsUI>();
            CreateButton(settings.transform, "BackBtn", "< BACK", new Vector2(-400, 850), new Vector2(180, 80), new Color(0.2f, 0.25f, 0.4f), () => settingsComp.OnClickBack(), defaultFont);
            uiMgr.settingsScreen = settings;

            // 10. Achievements Screen
            GameObject ach = CreatePanel(canvasObj.transform, "AchievementsScreen", new Color(0.04f, 0.06f, 0.14f, 0.95f));
            CreateText(ach.transform, "Title", "ACHIEVEMENTS", 54, Color.white, new Vector2(0, 680), defaultFont);
            var achComp = ach.AddComponent<AchievementsUI>();
            CreateButton(ach.transform, "BackBtn", "< BACK", new Vector2(-400, 850), new Vector2(180, 80), new Color(0.2f, 0.25f, 0.4f), () => achComp.OnClickBack(), defaultFont);
            uiMgr.achievementsScreen = ach;
        }

        private GameObject CreatePanel(Transform parent, string name, Color bgColor)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image img = panel.AddComponent<Image>();
            img.color = bgColor;
            return panel;
        }

        private Text CreateText(Transform parent, string name, string text, int fontSize, Color color, Vector2 pos, Font font)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rt = obj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(900, fontSize * 2.2f);

            Text t = obj.AddComponent<Text>();
            t.text = text;
            t.fontSize = fontSize;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = font;
            return t;
        }

        private GameObject CreateButton(Transform parent, string name, string text, Vector2 pos, Vector2 size, Color bgColor, UnityEngine.Events.UnityAction onClick, Font font)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.color = bgColor;

            Button btn = btnObj.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(onClick);

            // Button label
            GameObject lblObj = new GameObject("Label");
            lblObj.transform.SetParent(btnObj.transform, false);
            RectTransform lblRt = lblObj.AddComponent<RectTransform>();
            lblRt.anchorMin = Vector2.zero;
            lblRt.anchorMax = Vector2.one;
            lblRt.offsetMin = Vector2.zero;
            lblRt.offsetMax = Vector2.zero;

            Text t = lblObj.AddComponent<Text>();
            t.text = text;
            t.fontSize = 32;
            t.color = Color.white;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = font;

            return btnObj;
        }
    }
}
