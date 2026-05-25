
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Coin : MonoBehaviour
{
	public int value = 1;
	public AudioClip pickupSound;

	private void Reset()
	{
		var col = GetComponent<Collider>();
		if (col != null) col.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log($"Coin: '{gameObject.name}' collected by '{other.gameObject.name}' (tag={other.gameObject.tag}) value={value}");
			var pc = other.GetComponent<PlayerCoinCollector>();
			if (pc != null)
			{
				pc.AddCoins(value);
			}
			else
			{
				// publish delta so any UI can respond; total won't be known
				PlayerObserverManager.PublishCoinCollected(value);
			}

			if (pickupSound != null) AudioSource.PlayClipAtPoint(pickupSound, transform.position);
			Destroy(gameObject);
		}
	}
}


