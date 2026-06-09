
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Attach to a GameObject in the GUI scene. Assign a Text component to display coin total.
/// The GUI scene must be loaded additively by the GameManager when entering Gameplay.
/// </summary>
public class GUICoinController : MonoBehaviour
{
	public Text coinText;
	// support TextMeshProUGUI if the project uses TMP for UI
	public TextMeshProUGUI coinTextTMP;

	// cached total so we can update from deltas if total events are not available
	private int cachedTotal = 0;

	// Subscribe as early as possible so we don't miss broadcasts from other Start() calls
	private void Awake()
	{
		PlayerObserverManager.OnCoinTotalChanged += OnTotalChanged;
		PlayerObserverManager.OnCoinCollectedDelta += OnCollectedDelta;
		Debug.Log($"GUICoinController: subscribed to PlayerObserverManager events on '{gameObject.name}'");
	}

	private void OnDestroy()
	{
		PlayerObserverManager.OnCoinTotalChanged -= OnTotalChanged;
		PlayerObserverManager.OnCoinCollectedDelta -= OnCollectedDelta;
	}

	private void Start()
	{
		// Initialize to 0 if text is assigned (keeps previous behaviour)
		if (coinText != null)
			coinText.text = "Coins = 0";
		if (coinTextTMP != null)
			coinTextTMP.text = "Coins = 0";

		// Try to initialize display from an existing PlayerCoinCollector in the scene
		// in case the collector published its initial total before we subscribed.
		// Use newer API when available to avoid deprecation warnings
#if UNITY_2023_1_OR_NEWER
		var collector = UnityEngine.Object.FindFirstObjectByType<PlayerCoinCollector>();
#else
		var collector = UnityEngine.Object.FindObjectOfType<PlayerCoinCollector>();
#endif
		if (collector != null)
		{
			try
			{
				int current = collector.GetTotal();
				cachedTotal = current;
				OnTotalChanged(current);
			}
			catch (System.Exception ex)
			{
				Debug.LogWarning($"GUICoinController: failed to read initial total from PlayerCoinCollector: {ex}");
			}
		}
		else
		{
			// Helpful debug hint when GUI isn't wired up or player collector isn't present
			Debug.Log("GUICoinController: PlayerCoinCollector not found in scene. Will poll briefly in case it's loaded shortly.");
			StartCoroutine(PollForCollector(3.0f));
		}
	}

	private IEnumerator PollForCollector(float timeoutSeconds)
	{
		float end = Time.realtimeSinceStartup + timeoutSeconds;
		while (Time.realtimeSinceStartup < end)
		{
			// try to find the collector
#if UNITY_2023_1_OR_NEWER
			var collector = UnityEngine.Object.FindFirstObjectByType<PlayerCoinCollector>();
#else
			var collector = UnityEngine.Object.FindObjectOfType<PlayerCoinCollector>();
#endif
			if (collector != null)
			{
				try
				{
					int current = collector.GetTotal();
					cachedTotal = current;
					OnTotalChanged(current);
					yield break;
				}
				catch { }
			}
			yield return new WaitForSecondsRealtime(0.25f);
		}
		Debug.Log("GUICoinController: stopped polling for PlayerCoinCollector; it was not found.");
	}

	// Fallback handler: update cached total when a delta is published
	private void OnCollectedDelta(int delta)
	{
		cachedTotal += delta;
		Debug.Log($"GUICoinController: OnCollectedDelta received delta={delta} new cachedTotal={cachedTotal} on '{gameObject.name}'");
		UpdateText(cachedTotal);
	}

	private void OnTotalChanged(int total)
	{
		Debug.Log($"GUICoinController: OnTotalChanged received total={total} on '{gameObject.name}'");
		cachedTotal = total;
		UpdateText(total);
	}

	private void UpdateText(int total)
	{
		if (coinText != null)
		{
			coinText.text = "Coins = " + total.ToString();
			return;
		}
		if (coinTextTMP != null)
		{
			coinTextTMP.text = "Coins = " + total.ToString();
			return;
		}
		// Try to find a TMP or UI Text on this object as a fallback
		var tmp = GetComponentInChildren<TextMeshProUGUI>();
		if (tmp != null)
		{
			tmp.text = "Coins = " + total.ToString();
			return;
		}
		var txt = GetComponentInChildren<Text>();
		if (txt != null)
		{
			txt.text = "Coins = " + total.ToString();
			return;
		}
		Debug.LogWarning($"GUICoinController: received coin total {total} but no UI text component assigned or found on '{gameObject.name}'");
	}
}





