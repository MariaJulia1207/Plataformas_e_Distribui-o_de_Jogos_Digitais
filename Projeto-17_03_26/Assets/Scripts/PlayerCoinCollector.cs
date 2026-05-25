
using UnityEngine;

/// <summary>
/// Component to attach to the player to maintain a coin total and publish
/// updates through the static PlayerObserverManager.
/// </summary>
public class PlayerCoinCollector : MonoBehaviour
{
	private int totalCoins = 0;

	private void Start()
	{
		// publish initial total
		PlayerObserverManager.PublishCoinTotal(totalCoins);
	}

	public void AddCoins(int amount)
	{
		if (amount == 0) return;
		totalCoins += amount;
		Debug.Log($"PlayerCoinCollector: Player '{gameObject.name}' collected {amount} coins. Total now={totalCoins}");
		PlayerObserverManager.PublishCoinCollected(amount);
		PlayerObserverManager.PublishCoinTotal(totalCoins);
	}

	public int GetTotal() => totalCoins;
}


