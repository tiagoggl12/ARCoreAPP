using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Build command for CI/CD automation
/// </summary>
public class BuildCommand
{
    private static readonly string BUILD_PATH = "build";

    /// <summary>
    /// Main build method called from command line
    /// Usage: Unity -quit -batchmode -executeMethod BuildCommand.PerformBuild -buildTarget [iOS|Android]
    /// </summary>
    public static void PerformBuild()
    {
        // Get build target from command line args
        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;

        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-buildTarget" && i + 1 < args.Length)
            {
                string targetName = args[i + 1];
                Debug.Log($"[BuildCommand] Requested build target: {targetName}");

                if (Enum.TryParse(targetName, true, out BuildTarget parsedTarget))
                {
                    buildTarget = parsedTarget;
                }
            }
        }

        Debug.Log($"[BuildCommand] Starting build for {buildTarget}...");

        // Determine build path and output name
        string platformPath = "";
        string outputName = "";
        BuildOptions buildOptions = BuildOptions.None;

        switch (buildTarget)
        {
            case BuildTarget.iOS:
                platformPath = Path.Combine(BUILD_PATH, "iOS");
                outputName = ""; // iOS builds to a folder
                buildOptions = BuildOptions.None;
                break;

            case BuildTarget.Android:
                platformPath = Path.Combine(BUILD_PATH, "Android");
                outputName = PlayerSettings.productName + ".apk";
                buildOptions = BuildOptions.None;
                break;

            default:
                Debug.LogError($"[BuildCommand] Unsupported build target: {buildTarget}");
                EditorApplication.Exit(1);
                return;
        }

        // Create build directory if it doesn't exist
        if (!Directory.Exists(platformPath))
        {
            Directory.CreateDirectory(platformPath);
            Debug.Log($"[BuildCommand] Created directory: {platformPath}");
        }

        // Get all scenes in build settings
        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("[BuildCommand] No scenes found in build settings!");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"[BuildCommand] Building {scenes.Length} scenes:");
        foreach (string scene in scenes)
        {
            Debug.Log($"  - {scene}");
        }

        // Full output path
        string fullPath = buildTarget == BuildTarget.iOS
            ? platformPath
            : Path.Combine(platformPath, outputName);

        Debug.Log($"[BuildCommand] Output path: {fullPath}");
        Debug.Log($"[BuildCommand] Build options: {buildOptions}");

        // Perform build
        try
        {
            BuildReport report = BuildPipeline.BuildPlayer(scenes, fullPath, buildTarget, buildOptions);
            BuildSummary summary = report.summary;

            Debug.Log($"[BuildCommand] Build finished in {summary.totalTime}");
            Debug.Log($"[BuildCommand] Build result: {summary.result}");
            Debug.Log($"[BuildCommand] Build size: {summary.totalSize} bytes");

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"[BuildCommand] ✅ Build succeeded! Output: {fullPath}");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[BuildCommand] ❌ Build failed with result: {summary.result}");
                Debug.LogError($"[BuildCommand] Total errors: {summary.totalErrors}");
                Debug.LogError($"[BuildCommand] Total warnings: {summary.totalWarnings}");
                EditorApplication.Exit(1);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[BuildCommand] ❌ Build exception: {e.Message}");
            Debug.LogError(e.StackTrace);
            EditorApplication.Exit(1);
        }
    }

    /// <summary>
    /// Get all enabled scenes from build settings
    /// </summary>
    private static string[] GetEnabledScenes()
    {
        var scenes = new System.Collections.Generic.List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }

        return scenes.ToArray();
    }

    /// <summary>
    /// Build iOS - can be called directly
    /// </summary>
    [MenuItem("Build/Build iOS")]
    public static void BuildiOS()
    {
        Debug.Log("[BuildCommand] Building iOS from menu...");
        string[] args = Environment.GetCommandLineArgs();
        string[] newArgs = new string[args.Length + 2];
        args.CopyTo(newArgs, 0);
        newArgs[args.Length] = "-buildTarget";
        newArgs[args.Length + 1] = "iOS";
        Environment.SetEnvironmentVariable("COMMAND_LINE_ARGS", string.Join(" ", newArgs));
        PerformBuild();
    }

    /// <summary>
    /// Build Android - can be called directly
    /// </summary>
    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        Debug.Log("[BuildCommand] Building Android from menu...");
        string[] args = Environment.GetCommandLineArgs();
        string[] newArgs = new string[args.Length + 2];
        args.CopyTo(newArgs, 0);
        newArgs[args.Length] = "-buildTarget";
        newArgs[args.Length + 1] = "Android";
        Environment.SetEnvironmentVariable("COMMAND_LINE_ARGS", string.Join(" ", newArgs));
        PerformBuild();
    }
}
