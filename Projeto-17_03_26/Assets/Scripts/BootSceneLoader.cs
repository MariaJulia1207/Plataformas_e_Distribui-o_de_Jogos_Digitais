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
        // Delegate the boot loading to GameManager which centralizes SceneManager access
        var gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("BootSceneLoader: GameManager instance not found. Ensure GameManager exists in the project or in _Boot scene.");
            yield break;
        }

        gm.StartBootLoad(target.TargetSceneName);
        // GameManager will perform loading/unloading. We can stop this coroutine immediately.
        yield break;
    }
}
