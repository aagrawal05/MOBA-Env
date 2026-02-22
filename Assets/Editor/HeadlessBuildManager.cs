using UnityEngine;
using UnityEditor;
using System.IO;

public class HeadlessBuildManager : EditorWindow
{
    private const string SourceScenePath = "Assets/Scenes/MOBA.unity";

    [MenuItem("Windows/Headless Build")]
    public static void ShowWindow()
    {
        GetWindow<HeadlessBuildManager>("Headless Build Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("Headless Scene", EditorStyles.boldLabel);
        GUILayout.Label($"Source Scene: {SourceScenePath}", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Generate Headless Scene"))
            GenerateScene();
    }

    private void GenerateScene()
    {
        if (!File.Exists(SourceScenePath))
        {
            Debug.LogError($"Source scene not found at path: {SourceScenePath}");
            return;
        }

        string scenesFolder = "Assets/Scenes";
        string sceneName = Path.GetFileNameWithoutExtension(SourceScenePath);
        
        if (!AssetDatabase.IsValidFolder(scenesFolder))
            AssetDatabase.CreateFolder("Assets", "Scenes");

        string newScenePath = AssetDatabase.GenerateUniqueAssetPath($"{scenesFolder}/{sceneName}Headless.unity");

        AssetDatabase.CopyAsset(SourceScenePath, newScenePath);
        AssetDatabase.Refresh();

        Debug.Log($"Headless scene generated @ {newScenePath}");
    }
}
