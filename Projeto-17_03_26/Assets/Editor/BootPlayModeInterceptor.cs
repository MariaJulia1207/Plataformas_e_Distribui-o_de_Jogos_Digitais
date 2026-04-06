using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class BootPlayModeInterceptor
{
    private const string BootSceneName = "_Boot";
    private const string ResourcesFolder = "Assets/Resources";
    private const string ResourceAssetPath = "Assets/Resources/BootSceneTarget.asset";

    static BootPlayModeInterceptor()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // Before entering Play, find the _Boot scene asset
            var bootGuid = AssetDatabase.FindAssets("t:Scene " + BootSceneName);
            if (bootGuid == null || bootGuid.Length == 0)
            {
                if (!EditorUtility.DisplayDialog("_Boot scene not found", "No scene named '_Boot' was found in the project. Cancel Play or create an _Boot scene.", "Cancel Play", "Continue Anyway"))
                {
                    // Cancel play: abort by returning to Edit mode
                    EditorApplication.isPlaying = false;
                    return;
                }
                else
                {
                    // Continue without setting a start scene
                    return;
                }
            }

            // Use the first match
            var bootPath = AssetDatabase.GUIDToAssetPath(bootGuid[0]);
            var bootSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootPath);
            if (bootSceneAsset == null)
            {
                Debug.LogError("BootPlayModeInterceptor: Failed to load SceneAsset for _Boot.");
                return;
            }

            // Save the currently active scene path into a ScriptableObject under Resources
            var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            var targetPath = activeScene.path; // e.g. Assets/Scenes/Level1.unity
            var targetName = Path.GetFileNameWithoutExtension(targetPath);

            // Ensure Resources folder exists
            if (!AssetDatabase.IsValidFolder(ResourcesFolder))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            BootSceneTarget asset = AssetDatabase.LoadAssetAtPath<BootSceneTarget>(ResourceAssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<BootSceneTarget>();
                AssetDatabase.CreateAsset(asset, ResourceAssetPath);
            }

            asset.TargetScenePath = targetPath;
            asset.TargetSceneName = targetName;
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            // Set the playModeStartScene to _Boot
            UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene = bootSceneAsset;
            Debug.Log($"BootPlayModeInterceptor: Starting Play from '{bootPath}', saved target '{targetPath}'.");
        }

        if (state == PlayModeStateChange.EnteredEditMode)
        {
            // Clear the custom start scene after exiting Play
            UnityEditor.SceneManagement.EditorSceneManager.playModeStartScene = null;
            // Optionally, remove or clear the resources asset
            var asset = AssetDatabase.LoadAssetAtPath<BootSceneTarget>(ResourceAssetPath);
            if (asset != null)
            {
                asset.TargetScenePath = string.Empty;
                asset.TargetSceneName = string.Empty;
                EditorUtility.SetDirty(asset);
                AssetDatabase.SaveAssets();
            }
        }
    }
}


