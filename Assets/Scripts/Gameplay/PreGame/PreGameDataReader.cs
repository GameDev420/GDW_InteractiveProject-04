using UnityEngine;
using UnityEngine.InputSystem;

public class PreGameDataReader : MonoBehaviour
{
    // False in case players wanna play again (with same player configuration)
    [SerializeField] bool resetOnDisable = false;
    [SerializeField] PreGameData preGameData;
    [SerializeField] PlayerInputManager inputManager;

    private void Awake()
    {
        if (preGameData == null || inputManager == null || preGameData.Players == null || preGameData.Players.Length == 0) return;

        inputManager.DisableJoining();

        for (int i = 0; i < preGameData.Players.Length; i++)
        {
            var playerData = preGameData.Players[i];
            var player = inputManager.JoinPlayer(i, -1, playerData.controlScheme, playerData.pairDevices);

            var colorComponent = player.GetComponentInChildren<MaterialColorRandomizer>();
            if (colorComponent != null) colorComponent.SetColor(playerData.color);
        }

        if (GameManager.Instance != null) GameManager.Instance.StartGame();
    }

    private void OnDisable()
    {
        if (resetOnDisable && preGameData != null) preGameData.ResetData();
    }
}
