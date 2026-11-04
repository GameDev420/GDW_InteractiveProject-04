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

    [SerializeField] Transform[] spawnPoints = new Transform[0];
    
    private void Awake() => ResetStaticFields();
    private void OnDestroy() => ResetStaticFields();

    public void RemovePlayer(int index)
    {
        if (index < 0 || index >= Players.Count) return;
        Players[index].Remove();
    }

    public void OnPlayerJoined(PlayerInput player)
    {
        if (spawnPoints.Length > 0)
        {
            var point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            player.transform.SetPositionAndRotation(point.position, point.rotation);
        }

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
