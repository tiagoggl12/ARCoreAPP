using UnityEditor;
using UnityEngine;
using System.Linq;

namespace ARCoreApp.Editor
{
    /// <summary>
    /// Validates Unity project configuration for CI/CD and production builds
    /// </summary>
    public static class ProjectValidator
    {
        private static bool _hasErrors = false;

        [MenuItem("Tools/Validate Project Configuration")]
        public static void ValidateProject()
        {
            _hasErrors = false;
            Debug.Log("=== Starting Project Validation ===");

            ValidateBuildScenes();
            ValidateARPackages();
            ValidateAndroidSettings();
            ValidateiOSSettings();
            ValidatePlayerSettings();

            if (_hasErrors)
            {
                Debug.LogError("❌ Project validation FAILED! Please fix the errors above.");
                EditorApplication.Exit(1);
            }
            else
            {
                Debug.Log("✅ Project validation PASSED! All checks successful.");
            }
        }

        /// <summary>
        /// Validates that required scenes are configured in build settings
        /// </summary>
        private static void ValidateBuildScenes()
        {
            Debug.Log("--- Validating Build Scenes ---");

            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0)
            {
                LogError("No scenes configured in build settings!");
                return;
            }

            Debug.Log($"Found {scenes.Length} scene(s) in build settings:");

            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    if (System.IO.File.Exists(scene.path))
                    {
                        Debug.Log($"  ✓ {scene.path}");
                    }
                    else
                    {
                        LogError($"  ✗ Scene file not found: {scene.path}");
                    }
                }
                else
                {
                    Debug.LogWarning($"  ⚠ Scene disabled: {scene.path}");
                }
            }
        }

        /// <summary>
        /// Validates that required AR packages are installed
        /// </summary>
        private static void ValidateARPackages()
        {
            Debug.Log("--- Validating AR Packages ---");

            var requiredPackages = new[]
            {
                "com.unity.xr.arfoundation",
                "com.unity.xr.arcore",
                "com.unity.xr.arkit",
                "com.unity.xr.interaction.toolkit",
                "com.unity.xr.management"
            };

            var installedPackages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages()
                .Select(p => p.name)
                .ToHashSet();

            foreach (var package in requiredPackages)
            {
                if (installedPackages.Contains(package))
                {
                    var packageInfo = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages()
                        .FirstOrDefault(p => p.name == package);
                    Debug.Log($"  ✓ {package} @ {packageInfo?.version}");
                }
                else
                {
                    LogError($"  ✗ Required package not installed: {package}");
                }
            }
        }

        /// <summary>
        /// Validates Android-specific settings
        /// </summary>
        private static void ValidateAndroidSettings()
        {
            Debug.Log("--- Validating Android Settings ---");

            // Min SDK Version
            int minSdk = (int)PlayerSettings.Android.minSdkVersion;
            if (minSdk >= 30)
            {
                Debug.Log($"  ✓ Min SDK Version: API {minSdk} (Android 11+)");
            }
            else
            {
                LogError($"  ✗ Min SDK should be API 30 or higher (Android 11+), currently API {minSdk}");
            }

            // Package Name
            string packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            if (!string.IsNullOrEmpty(packageName))
            {
                if (packageName.Contains("DefaultCompany"))
                {
                    LogWarning($"  ⚠ Package name contains 'DefaultCompany': {packageName}");
                }
                else
                {
                    Debug.Log($"  ✓ Package Name: {packageName}");
                }
            }
            else
            {
                LogError("  ✗ Android package name is not configured");
            }

            // Target Architecture
            var targetArch = PlayerSettings.Android.targetArchitectures;
            Debug.Log($"  ✓ Target Architectures: {targetArch}");

            // Scripting Backend
            var scriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            Debug.Log($"  ✓ Scripting Backend: {scriptingBackend}");
        }

        /// <summary>
        /// Validates iOS-specific settings
        /// </summary>
        private static void ValidateiOSSettings()
        {
            Debug.Log("--- Validating iOS Settings ---");

            // Bundle Identifier
            string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            if (!string.IsNullOrEmpty(bundleId))
            {
                if (bundleId.Contains("DefaultCompany"))
                {
                    LogWarning($"  ⚠ Bundle ID contains 'DefaultCompany': {bundleId}");
                }
                else
                {
                    Debug.Log($"  ✓ Bundle Identifier: {bundleId}");
                }
            }
            else
            {
                LogError("  ✗ iOS bundle identifier is not configured");
            }

            // Target iOS Version
            string targetVersion = PlayerSettings.iOS.targetOSVersionString;
            Debug.Log($"  ✓ Target iOS Version: {targetVersion}");

            // Camera Usage Description
            string cameraUsage = PlayerSettings.iOS.cameraUsageDescription;
            if (string.IsNullOrEmpty(cameraUsage))
            {
                LogWarning("  ⚠ Camera usage description is not set (required for ARKit)");
            }
            else
            {
                Debug.Log($"  ✓ Camera Usage Description: {cameraUsage}");
            }
        }

        /// <summary>
        /// Validates general player settings
        /// </summary>
        private static void ValidatePlayerSettings()
        {
            Debug.Log("--- Validating Player Settings ---");

            // Product Name
            string productName = PlayerSettings.productName;
            if (!string.IsNullOrEmpty(productName))
            {
                Debug.Log($"  ✓ Product Name: {productName}");
            }
            else
            {
                LogError("  ✗ Product name is not configured");
            }

            // Company Name
            string companyName = PlayerSettings.companyName;
            if (!string.IsNullOrEmpty(companyName))
            {
                if (companyName == "DefaultCompany")
                {
                    LogWarning($"  ⚠ Company name is 'DefaultCompany'");
                }
                else
                {
                    Debug.Log($"  ✓ Company Name: {companyName}");
                }
            }
            else
            {
                LogError("  ✗ Company name is not configured");
            }

            // Version
            string version = PlayerSettings.bundleVersion;
            Debug.Log($"  ✓ Version: {version}");

            // Graphics API
            var graphicsAPIs = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            Debug.Log($"  ✓ Graphics APIs (Android): {string.Join(", ", graphicsAPIs)}");
        }

        private static void LogError(string message)
        {
            _hasErrors = true;
            Debug.LogError(message);
        }

        private static void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// Validates project configuration and exits with error code if validation fails
        /// Called by CI/CD workflows
        /// </summary>
        public static void ValidateForCI()
        {
            Debug.Log("=== CI/CD Project Validation ===");
            ValidateProject();
        }
    }
}
