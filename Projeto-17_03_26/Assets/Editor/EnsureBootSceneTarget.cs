using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal static class EnsureBootSceneTarget
{
    private const string ResourceAssetPath = "Assets/Resources/BootSceneTarget.asset";

    static EnsureBootSceneTarget()
    {
        EditorApplication.delayCall += EnsureAssetExists;
    }

    private static void EnsureAssetExists()
    {
        var asset = AssetDatabase.LoadAssetAtPath<BootSceneTarget>(ResourceAssetPath);
        if (asset == null)
        {
            var instance = ScriptableObject.CreateInstance<BootSceneTarget>();
            instance.TargetSceneName = "Splash";
            // create Resources folder if missing
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateAsset(instance, ResourceAssetPath);
            AssetDatabase.SaveAssets();
            Debug.Log("EnsureBootSceneTarget: Created Resources/BootSceneTarget.asset with TargetSceneName='Splash'.");
        }
        // Also ensure the _Boot scene contains a BootSceneLoader GameObject so the redirect will run in Play.
        EnsureBootSceneHasLoader();
    }

    private static void EnsureBootSceneHasLoader()
    {
        // Find boot scene path
        var guids = AssetDatabase.FindAssets("t:Scene");
        string bootPath = null;
        foreach (var g in guids)
        {
            var p = AssetDatabase.GUIDToAssetPath(g);
            var filename = System.IO.Path.GetFileNameWithoutExtension(p);
            if (string.Equals(filename, "_Boot", System.StringComparison.OrdinalIgnoreCase))
            {
                bootPath = p;
                break;
            }
        }
        if (string.IsNullOrEmpty(bootPath))
        {
            Debug.LogWarning("EnsureBootSceneTarget: _Boot scene not found in project.");
            return;
        }

        // Remember current scene
        var active = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        // Open boot scene additively to modify
        var scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(bootPath, UnityEditor.SceneManagement.OpenSceneMode.Additive);
        if (!scene.IsValid())
        {
            Debug.LogWarning($"EnsureBootSceneTarget: failed to open _Boot scene at '{bootPath}'.");
            return;
        }

        bool changed = false;
        // Check for existing BootSceneLoader
        var rootGOs = scene.GetRootGameObjects();
        bool hasLoader = false;
        foreach (var go in rootGOs)
        {
            if (go.GetComponent<BootSceneLoader>() != null)
            {
                hasLoader = true;
                break;
            }
        }

        if (!hasLoader)
        {
            var go = new GameObject("BootRunner");
            go.AddComponent<BootSceneLoader>();
            UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(go, scene);
            changed = true;
            Debug.Log("EnsureBootSceneTarget: Added BootSceneLoader to _Boot scene.");
        }

        // Optionally ensure there's a GameManagerCore/GameManager present
        bool hasGM = false;
        rootGOs = scene.GetRootGameObjects();
        foreach (var go in rootGOs)
        {
            if (go.GetComponent<GameManagerCore>() != null || go.GetComponent("GameManager") != null)
            {
                hasGM = true;
                break;
            }
        }
        if (!hasGM)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManagerCore>();
            UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(go, scene);
            changed = true;
            Debug.Log("EnsureBootSceneTarget: Added GameManagerCore to _Boot scene.");
        }

        if (changed)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
            Debug.Log("EnsureBootSceneTarget: _Boot scene updated and saved.");
        }

        // Close the additive scene and restore active
        UnityEditor.SceneManagement.EditorSceneManager.CloseScene(scene, true);
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(active.path, UnityEditor.SceneManagement.OpenSceneMode.Single);
    }
}


