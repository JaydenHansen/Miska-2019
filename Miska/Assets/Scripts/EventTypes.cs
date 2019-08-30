using UnityEngine.Events;

[System.Serializable]
public class VoidEvent : UnityEvent { }

[System.Serializable]
public class PlayerEvent : UnityEvent<Player> { }
