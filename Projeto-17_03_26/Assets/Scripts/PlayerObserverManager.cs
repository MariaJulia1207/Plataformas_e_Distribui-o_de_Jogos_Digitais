using System;

// Static event manager (observer) for player-related events such as coin collection.
// Provides two channels:
// - OnCoinCollectedDelta: publishes when a coin is picked (delta amount)
// - OnCoinTotalChanged: publishes when the player's total coin count changes
public static class PlayerObserverManager
{
    public static event Action<int> OnCoinCollectedDelta;
    public static event Action<int> OnCoinTotalChanged;

    public static void PublishCoinCollected(int delta)
    {
        OnCoinCollectedDelta?.Invoke(delta);
    }

    public static void PublishCoinTotal(int total)
    {
        OnCoinTotalChanged?.Invoke(total);
    }
}


