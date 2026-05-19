
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to a GameObject in the GUI scene. Assign a Text component to display coin total.
/// The GUI scene must be loaded additively by the GameManager when entering Gameplay.
/// </summary>
public class GUICoinController : MonoBehaviour
{
	public Text coinText;

	private void OnEnable()
	{
		PlayerObserverManager.OnCoinTotalChanged += OnTotalChanged;
	}

	private void OnDisable()
	{
		PlayerObserverManager.OnCoinTotalChanged -= OnTotalChanged;
	}

	private void Start()
	{
		if (coinText != null) coinText.text = "0";
	}

	private void OnTotalChanged(int total)
	{
		if (coinText != null)
			coinText.text = total.ToString();
	}
}

