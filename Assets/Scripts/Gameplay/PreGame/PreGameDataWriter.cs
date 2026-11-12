using UnityEngine;

public class PreGameDataWriter : MonoBehaviour
{
    [SerializeField] bool resetOnAwake = true;
    [SerializeField] PreGameData preGameData;

    private void Awake()
    {
        if (resetOnAwake && preGameData != null) preGameData.ResetData();
    }

    public void WritePreGameData()
    {
        if (preGameData == null) return;

        preGameData.Players = new PreGameData.PlayerData[PlayerManager.Players.Count];

        for (int i = 0; i < PlayerManager.Players.Count; i++)
        {
            var player = PlayerManager.Players[i];

            var colorComponent = player.GetComponentInChildren<MaterialColorRandomizer>();

            preGameData.Players[i] = new PreGameData.PlayerData
            {
                color = colorComponent != null ? colorComponent.Color : Color.white,
                playerIndex = i,
                controlScheme = player.Input.currentControlScheme,
                // Honestly not sure how the devices array works, hopefully this is fine
                pairDevices = player.Input.devices.ToArray()
            };
        }
    }
}
