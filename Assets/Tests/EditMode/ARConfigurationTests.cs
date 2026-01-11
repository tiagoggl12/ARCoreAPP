using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ARCoreApp.Tests.EditMode
{
    /// <summary>
    /// Tests to validate AR project configuration and settings
    /// </summary>
    public class ARConfigurationTests
    {
        [Test]
        public void BuildScene_SampleScene_Exists()
        {
            // Verify SampleScene.unity exists in build settings
            string scenePath = "Assets/Scenes/SampleScene.unity";
            Assert.IsTrue(System.IO.File.Exists(scenePath),
                $"Build scene not found at {scenePath}");
        }

        [Test]
        public void BuildSettings_HasAtLeastOneScene()
        {
            // Verify at least one scene is configured in build settings
            var scenes = EditorBuildSettings.scenes;
            Assert.Greater(scenes.Length, 0, "No scenes configured in build settings");
            Assert.IsTrue(scenes[0].enabled, "First scene in build settings is not enabled");
        }

        [Test]
        public void ARFoundation_PackageInstalled()
        {
            // Verify AR Foundation package is present
            var packageList = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            bool hasARFoundation = packageList.Any(p => p.name == "com.unity.xr.arfoundation");
            Assert.IsTrue(hasARFoundation, "AR Foundation package (com.unity.xr.arfoundation) not installed");
        }

        [Test]
        public void ARCore_PackageInstalled()
        {
            // Verify ARCore package is present for Android support
            var packageList = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            bool hasARCore = packageList.Any(p => p.name == "com.unity.xr.arcore");
            Assert.IsTrue(hasARCore, "ARCore package (com.unity.xr.arcore) not installed");
        }

        [Test]
        public void ARKit_PackageInstalled()
        {
            // Verify ARKit package is present for iOS support
            var packageList = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            bool hasARKit = packageList.Any(p => p.name == "com.unity.xr.arkit");
            Assert.IsTrue(hasARKit, "ARKit package (com.unity.xr.arkit) not installed");
        }

        [Test]
        public void XRInteractionToolkit_PackageInstalled()
        {
            // Verify XR Interaction Toolkit package is present
            var packageList = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
            bool hasXRIT = packageList.Any(p => p.name == "com.unity.xr.interaction.toolkit");
            Assert.IsTrue(hasXRIT, "XR Interaction Toolkit package (com.unity.xr.interaction.toolkit) not installed");
        }

#if UNITY_ANDROID
        [Test]
        public void Android_MinSDKVersion_IsAtLeast30()
        {
            // Verify Android Min SDK is set to API 30 or higher (Android 11+)
            int minSdk = (int)PlayerSettings.Android.minSdkVersion;
            Assert.GreaterOrEqual(minSdk, 30,
                $"Android Min SDK should be 30 or higher (Android 11+), currently {minSdk}");
        }

        [Test]
        public void Android_PackageName_IsConfigured()
        {
            // Verify Android package name is set
            string packageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            Assert.IsNotEmpty(packageName, "Android package name is not configured");
            Assert.IsFalse(packageName.Contains("DefaultCompany"),
                "Android package name should not contain DefaultCompany");
        }
#endif

#if UNITY_IOS
        [Test]
        public void iOS_MinimumVersion_IsConfigured()
        {
            // Verify iOS minimum version is set appropriately for ARKit
            string minVersion = PlayerSettings.iOS.targetOSVersionString;
            Assert.IsNotEmpty(minVersion, "iOS minimum version is not configured");
        }

        [Test]
        public void iOS_BundleIdentifier_IsConfigured()
        {
            // Verify iOS bundle identifier is set
            string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
            Assert.IsNotEmpty(bundleId, "iOS bundle identifier is not configured");
            Assert.IsFalse(bundleId.Contains("DefaultCompany"),
                "iOS bundle identifier should not contain DefaultCompany");
        }
#endif

        [Test]
        public void ProductName_IsConfigured()
        {
            // Verify product name is set
            string productName = PlayerSettings.productName;
            Assert.IsNotEmpty(productName, "Product name is not configured");
        }

        [Test]
        public void CompanyName_IsConfigured()
        {
            // Verify company name is set
            string companyName = PlayerSettings.companyName;
            Assert.IsNotEmpty(companyName, "Company name is not configured");
        }
    }
}
