using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

// Just to ensure event subscribers during OnEnable and OnDisable don't get reset on this component's Awake.
[DefaultExecutionOrder(-2)]
public class PlayerManager : MonoBehaviour
{
    // Events
    public delegate void PlayerEventHandler(PlayerController player);

    // Static Fields
    public static event PlayerEventHandler PlayerJoined;
    public static event PlayerEventHandler PlayerLeft;
    public static readonly List<PlayerController> Players = new();
    
    private void Awake() => ResetStaticFields();
    private void OnDestroy() => ResetStaticFields();

    public void OnPlayerJoined(PlayerInput player)
    {
        var controller = player.GetComponent<PlayerController>();
        Players.Add(controller);
        if (PlayerJoined != null) PlayerJoined.Invoke(controller);
    }
    public void OnPlayerLeft(PlayerInput player)
    {
        var controller = player.GetComponent<PlayerController>();
        if (Players.Contains(controller)) Players.Remove(controller);
        if (PlayerLeft != null) PlayerLeft.Invoke(controller);
    }

    // Ensures no data is kept between playmode sessions
    void ResetStaticFields()
    {
        PlayerJoined = null;
        PlayerLeft = null;
        Players.Clear();
    }
}
