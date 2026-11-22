using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CheremoRunner.Editor
{
    public static class CheremoBuildScript
    {
        [MenuItem("Cheremo/Build Android APK")]
        public static void BuildAndroid()
        {
            Debug.Log("==================================================");
            Debug.Log("CHEREMO RUNNER — ANDROID BUILD PIPELINE STARTING");
            Debug.Log("==================================================");

            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Android");
            Directory.CreateDirectory(outputDir);
            string apkPath = Path.Combine(outputDir, "CHEREMO-RUNNER.apk");

            // Configure PlayerSettings
            PlayerSettings.companyName = "CHEREMO";
            PlayerSettings.productName = "CHEREMO RUNNER";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.cheremo.runner");

            // Portrait orientation
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            // Android SDK & Architecture targets
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24; // Android 7.0
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Android 14
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

            // Color space
            PlayerSettings.colorSpace = ColorSpace.Linear;

            string[] scenes = new string[] { "Assets/Scenes/MainScene.unity" };

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            Debug.Log($"Building APK to: {apkPath}");
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"SUCCESS: Android build completed! Size: {summary.totalSize} bytes");
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(0);
                }
            }
            else
            {
                Debug.LogError($"FAILED: Android build failed with result: {summary.result}");
                foreach (var step in report.steps)
                {
                    foreach (var message in step.messages)
                    {
                        if (message.type == LogType.Error || message.type == LogType.Exception)
                        {
                            Debug.LogError($"[BuildError] {message.content}");
                        }
                    }
                }
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
            }
        }
    }
}
