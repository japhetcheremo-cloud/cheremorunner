# 🏃 CHEREMO RUNNER

[![Android Build & Release](https://github.com/japhetcheremo-cloud/cheremorunner/actions/workflows/android-build.yml/badge.svg)](https://github.com/japhetcheremo-cloud/cheremorunner/actions/workflows/android-build.yml)
[![Platform](https://img.shields.io/badge/Platform-Android%207.0%2B-green.svg)](https://www.android.com)
[![Engine](https://img.shields.io/badge/Unity-2022.3%20LTS-blue.svg)](https://unity.com)
[![License](https://img.shields.io/badge/Developer-CHEREMO-orange.svg)](#)

> **TAGLINE:** *RUN. DODGE. COLLECT. CONQUER.*  
> **DEVELOPER / BRAND:** CHEREMO  
> **PLATFORM:** Android Mobile Application (Portrait Orientation)  
> **GAME TYPE:** 3D Endless Runner  

---

## 🌟 Overview

**CHEREMO RUNNER** is a high-octane 3D endless runner mobile game built from the ground up with Unity and C#. Players navigate an energetic cyberpunk-styled metropolis, swiping across three lanes to dodge barriers, leap over hurdles, slide beneath laser signs, and outmaneuver moving drones while harvesting coins and supercharging abilities with powerups.

---

## 🎮 Key Features

- **⚡ Third-Person 3D Endless Runner**: Smooth portrait gameplay with auto-forward running, 3-lane lateral motion, jumping, and sliding.
- **📱 Responsive Touch & Swipe Controls**:
  - **Swipe Left / Right**: Switch lanes instantly with smooth easing.
  - **Swipe Up**: Leap over barriers and road hazards.
  - **Swipe Down**: Slide beneath overhead bridges/signs, or perform an instant dive from mid-air.
  - **Keyboard Testing Support**: `A`/`D` (Lanes), `W`/`Space` (Jump), `S` (Slide).
- **🌆 Procedural 3D City Track**:
  - Endless streaming track segments generated with zero garbage collection via custom **Object Pooling**.
  - Dynamic skyscrapers with glowing windows, street lamps, sidewalks, and neon lane dividers.
  - Fair obstacle generation: guarantees at least one viable lane or jump/slide path.
- **🛑 Diverse Obstacles**:
  - **Low Barriers**: Require jumping.
  - **Overhead Signs & Lasers**: Require sliding.
  - **Roadblocks**: Require lane switches.
  - **Moving Drones / Traffic**: Dynamically patrol across lanes.
- **✨ Collectible Economy & Powerups**:
  - **Gold Coins**: Placed in arcs, lines, and patterns.
  - **Coin Magnet**: Draws nearby coins towards the runner.
  - **Energy Shield**: Absorbs one obstacle impact, protecting the player.
  - **2X Score Multiplier**: Doubles score accumulation rate and coin value.
- **🥋 4 Unlockable Character Skins**:
  1. **Cheremo Classic**: Signature electric blue & white suit with blaze orange visor (Default).
  2. **Cyber Phantom**: Carbon-fiber black chassis with bright cyan neon highlights (+5% Speed, +5% Coins).
  3. **Gold Rush**: Gilded champion armor for high-rollers (+5% Speed, +15% Coins).
  4. **Urban Shadow**: Deep stealth shinobi suit with crimson optics (+10% Speed, +10% Coins).
- **🛒 Functional Shop & Upgrades**:
  - Upgrade Magnet, Shield, and Multiplier durations up to Level 5.
  - Purchase skins with earned coins.
- **📊 10 Complete Functional Screens**:
  1. Splash Screen (Cheremo branding & logo)
  2. Loading Screen
  3. Main Menu (Play, Characters, Shop, Achievements, Settings)
  4. Character Selection (Interactive 3D turntable preview)
  5. Shop Screen (Upgrades & items)
  6. Gameplay HUD (Live score, distance meters, coins, pause button)
  7. Pause Menu (Resume, Restart, Main Menu)
  8. Game-Over Screen (Run score, personal best, coins earned, replay)
  9. Settings Screen (Music, SFX, Haptic Vibration, Graphic Quality)
  10. Achievements Screen (Milestone badges & coin rewards)
- **🔊 Dynamic Audio & Haptics**:
  - Synthesized upbeat 128 BPM electronic runner soundtrack.
  - High-impact audio effects for jumps, slides, coin chimes, crashes, and powerups.
  - Android haptic feedback on obstacle collision.
- **💾 100% Offline Persistence**:
  - Automatically saves high scores, wallet balance, unlocked skins, and settings locally using reliable serialization.

---

## 🛠️ Technology Stack

- **Game Engine**: Unity 3D (2022.3 LTS)
- **Programming Language**: C# (.NET Standard 2.1)
- **Render Pipeline**: Universal Render Pipeline (URP) / Standard
- **Target OS**: Android 7.0+ (API Level 24 to 34)
- **Architecture**: ARM64 (arm64-v8a) & ARMv7 (armeabi-v7a)
- **CI/CD Automation**: GitHub Actions & GameCI

---

## 📦 Project Structure

```
cheremorunner/
├── .github/
│   └── workflows/
│       └── android-build.yml       # GitHub Actions CI/CD for APK generation & releases
├── Assets/
│   ├── Audio/                      # 16-bit 44.1kHz WAV sound effects and BGM
│   ├── Editor/
│   │   └── CheremoBuildScript.cs   # Headless Android build automation script
│   ├── Scenes/
│   │   ├── MainScene.unity         # Core game scene
│   │   └── MainScene.unity.meta
│   ├── Scripts/
│   │   ├── Audio/                  # AudioManager and sound triggers
│   │   ├── Camera/                 # CameraFollow with dynamic FOV & shake
│   │   ├── Core/                   # GameManager, SaveManager, GameConstants, Bootstrapper
│   │   ├── Environment/            # TrackSpawner, TrackSegment, ObjectPool
│   │   ├── Gameplay/               # ProceduralMeshBuilder, Coins, Powerups, Obstacles
│   │   ├── Player/                 # PlayerController, SwipeDetector, AnimationHelper, Skins
│   │   └── UI/                     # UIManager, UIBuilder, ShopUI, SettingsUI, AchievementsUI
│   └── Textures/
│       ├── Environment/            # Road asphalt, building facades, sidewalks
│       ├── Icons/                  # 512x512 Android App Icon
│       └── UI/                     # Splash background, title logo, coin icon, buttons
├── Packages/
│   └── manifest.json               # Unity Package Manager manifest
├── ProjectSettings/
│   ├── EditorBuildSettings.asset
│   ├── ProjectSettings.asset       # Android package 'com.cheremo.runner', orientation, SDKs
│   ├── ProjectVersion.txt          # Unity 2022.3.45f1 LTS
│   └── TagManager.asset            # Tags: Player, Obstacle, Coin, Powerup, Ground
├── .gitignore                      # Standard Unity Git ignore rules
└── README.md                       # Complete documentation
```

---

## 📲 Download & Installation (Android)

### Direct APK Download
Once GitHub Actions completes the build:
- **Download Latest APK**: [CHEREMO-RUNNER.apk](https://github.com/japhetcheremo-cloud/cheremorunner/releases/latest/download/CHEREMO-RUNNER.apk)
- **All Releases**: [GitHub Releases](https://github.com/japhetcheremo-cloud/cheremorunner/releases)
- **Build Status**: [GitHub Actions](https://github.com/japhetcheremo-cloud/cheremorunner/actions)

### How to Install on Android Devices:
1. Open the direct APK download link on your Android device or download it to your PC and transfer it via USB.
2. Tap `CHEREMO-RUNNER.apk` in your Downloads folder.
3. If Android displays *"Install unknown apps"*, toggle **Allow from this source**.
4. Tap **Install**.
5. Launch **CHEREMO RUNNER** and play!

---

## ⚙️ Building from Source

### Prerequisites
- **Unity Hub & Unity Editor 2022.3.x LTS** (with Android Build Support, Android SDK & NDK tools, OpenJDK installed).

### Building in Unity Editor:
1. Open **Unity Hub** and click **Add** -> Select the `cheremorunner` folder.
2. Open the project in Unity Editor (2022.3 LTS).
3. Switch build target to **Android**:
   - `File` -> `Build Settings...` -> Select `Android` -> Click `Switch Platform`.
4. Run the automated build:
   - Click the top menu: `Cheremo` -> `Build Android APK`.
   - The compiled APK will be generated at `Builds/Android/CHEREMO-RUNNER.apk`.

### Building via Command Line:
```bash
Unity.exe -quit -batchmode -projectPath "C:\path\to\cheremorunner" -executeMethod CheremoRunner.Editor.CheremoBuildScript.BuildAndroid -logFile "build.log"
```

---

## 🤖 GitHub Actions CI/CD Configuration

The repository includes an automated workflow at `.github/workflows/android-build.yml` powered by **GameCI**.

### Required Secrets for Automated CI Building:
To enable Unity licensing in GitHub Actions, configure the following secrets in your repository (`Settings` -> `Secrets and variables` -> `Actions`):

1. **`UNITY_LICENSE`** (Recommended for Personal License):
   - Export your Unity license file (`.ulf`) content and paste it as `UNITY_LICENSE`.
   - *Guide*: Follow [GameCI Activation Guide](https://game.ci/docs/github/activation).
2. Or use **`UNITY_EMAIL`**, **`UNITY_PASSWORD`**, and **`UNITY_SERIAL`** (for Unity Plus/Pro).

When pushed to `main` or triggered manually via `workflow_dispatch`, the workflow will:
1. Check out the project.
2. Set up Unity 2022.3 with Android SDK/NDK.
3. Execute `CheremoRunner.Editor.CheremoBuildScript.BuildAndroid`.
4. Verify the generated APK.
5. Upload `CHEREMO-RUNNER.apk` as a build artifact.
6. Publish a new GitHub Release with the attached APK!

---

## 👤 Developer & Brand

- **Brand**: CHEREMO
- **Repository**: [japhetcheremo-cloud/cheremorunner](https://github.com/japhetcheremo-cloud/cheremorunner)
- **Copyright**: © 2026 CHEREMO. All rights reserved.
