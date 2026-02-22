using UnityEditor;
using UnityEngine;

public class HeadlessBuild
{
    [MenuItem("Build/Build Headless")]
    static void Build()
    {
        // Define the build options
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/MOBAOptimized.unity" }, // Replace with your scene(s)
            locationPathName = "Builds/Headless/",
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.EnableHeadlessMode | BuildOptions.Development // or | BuildOptions.None for a release build
        };

        // Execute the build
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log("Headless build complete.");
    }
}
