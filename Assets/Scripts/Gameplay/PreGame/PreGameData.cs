using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Stores pre-game info like player count and color that needs to be preserved between scenes.
/// </summary>
[CreateAssetMenu(fileName = "PreGameData", menuName = "Scriptable Objects/PreGameData")]
public class PreGameData : ScriptableObject
{
    [System.Serializable]
    public struct PlayerData
    {
        public Color color;
        public int playerIndex;
        public string controlScheme;
        public InputDevice[] pairDevices;

        public PlayerData(Color color, int playerIndex, string controlScheme, InputDevice[] pairDevices)
        {
            this.color = color;
            this.playerIndex = playerIndex;
            this.controlScheme = controlScheme;
            this.pairDevices = pairDevices;
        }
    }

    public PlayerData[] Players;

    internal void ResetData()
    {
        Players = new PlayerData[0];
    }
}
