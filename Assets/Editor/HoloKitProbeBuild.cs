using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Builds a development iOS player with the SDK feature probe (HOLOKIT_SDK_TESTS).
// Project settings are not modified; the scene list, define and optional bundle id apply to this build only.
//   unity build . --target iOS --execute-method HoloKitProbeBuild.Build -o Build_Probe \
//     --args "-probeBundleId org.example.app"
// See Packages/io.holokit.unity-sdk/Tests/Probe/README.md for running the probe on a device.
public static class HoloKitProbeBuild
{
    static readonly string[] Scenes =
    {
        "Assets/Samples/StereoscopicRendering/Scenes/StereoscopicRendering.unity",
        "Assets/Samples/HandTracking/Scenes/HandTracking.unity",
        "Assets/Samples/HandGestureRecognition/Scenes/HandGestureRecognition.unity",
        "Assets/Samples/GazeGestureInteraction/Scenes/GazeGestureInteraction.unity",
        "Assets/Samples/GazeInteraction/Scenes/GazeInteraction.unity",
        "Assets/Samples/ResetWorldOrigin/Scenes/ResetWorldOrigin.unity",
        "Assets/Samples/ARBackgroundVideoEnhancement/Scenes/ARBackgroundVideoEnhancement.unity",
        "Assets/Samples/RecordingAudio/Scenes/RecordingEnvironment.unity",
        "Assets/Samples/RecordingAudio/Scenes/RecordingGameAndMicrophone.unity",
    };

    public static void Build()
    {
        string output = Argument("-buildOutput") ?? "Build_Probe";
        string bundleId = Argument("-probeBundleId");
        string previousId = PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS);
        if (bundleId != null)
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS, bundleId);
        try
        {
            var options = new BuildPlayerOptions
            {
                scenes = Scenes.Where(File.Exists).ToArray(),
                locationPathName = output,
                target = BuildTarget.iOS,
                targetGroup = BuildTargetGroup.iOS,
                options = BuildOptions.Development,
                extraScriptingDefines = new[] { "HOLOKIT_SDK_TESTS" },
            };
            Debug.Log($"[HoloKitProbeBuild] scenes={string.Join(",", options.scenes)} bundle={bundleId ?? previousId}");
            BuildReport report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new Exception($"Probe build failed: {report.summary.result}");
        }
        finally
        {
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS, previousId);
            AssetDatabase.SaveAssets();
        }
    }

    static string Argument(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }
}
