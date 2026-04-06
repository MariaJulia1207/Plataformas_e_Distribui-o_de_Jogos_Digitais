using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        // Load the BootSceneTarget from Resources
        var target = Resources.Load<BootSceneTarget>("BootSceneTarget");
        if (target == null || string.IsNullOrEmpty(target.TargetSceneName))
        {
            Debug.LogError("BootSceneLoader: No BootSceneTarget found in Resources or target scene name is empty. Ensure the editor created Assets/Resources/BootSceneTarget.asset before Play.");
            yield break;
        }

        // If the target is the Boot scene itself, do nothing
        var currentScene = SceneManager.GetActiveScene();
        if (string.Equals(currentScene.name, target.TargetSceneName, System.StringComparison.OrdinalIgnoreCase))
        {
            Debug.Log("BootSceneLoader: Target scene is the Boot scene itself. Nothing to load.");
            yield break;
        }

        // Load the target scene additively
        var loadOp = SceneManager.LoadSceneAsync(target.TargetSceneName, LoadSceneMode.Additive);
        if (loadOp == null)
        {
            Debug.LogError($"BootSceneLoader: Failed to start loading scene '{target.TargetSceneName}'. Ensure it is added to Build Settings and the name is correct.");
            yield break;
        }

        while (!loadOp.isDone)
            yield return null;

        // Optionally, set the newly loaded scene as active
        var loadedScene = SceneManager.GetSceneByName(target.TargetSceneName);
        if (loadedScene.IsValid())
            SceneManager.SetActiveScene(loadedScene);

        // Unload the Boot scene
        var unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        if (unloadOp == null)
        {
            Debug.LogWarning("BootSceneLoader: Failed to unload Boot scene.");
            yield break;
        }

        while (!unloadOp.isDone)
            yield return null;

        Debug.Log($"BootSceneLoader: Loaded '{target.TargetSceneName}' and unloaded Boot scene.");
    }
}
