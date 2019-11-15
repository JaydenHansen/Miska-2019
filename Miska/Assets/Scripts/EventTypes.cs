using UnityEngine.Events;

/// <summary>
/// Unity event with no parameters
/// </summary>
[System.Serializable]
public class VoidEvent : UnityEvent { }

/// <summary>
/// Unity event that takes in a player
/// </summary>
[System.Serializable]
public class PlayerEvent : UnityEvent<Player> { }
