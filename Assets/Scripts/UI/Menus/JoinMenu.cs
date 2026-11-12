using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class JoinMenu : Menu
{
    [SerializeField] string startGameInputAction = "StartGame";
    [Space]
    [SerializeField] string playerCountPrefix = "Players: ";
    [SerializeField] string playerCountSuffix = "";
    [SerializeField] TMP_Text playerCountText;
    [SerializeField] TMP_Text startText;
    [SerializeField] PlayerInputManager inputManager;
    [Header("Events")]
    [SerializeField] UnityEvent onGameStart;

    private void Awake()
    {
        if (inputManager != null && inputManager.enabled) inputManager.enabled = false;
    }

    private void OnEnable()
    {
        PlayerManager.PlayerJoined += OnPlayerJoined;
        PlayerManager.PlayerLeft += OnPlayerLeft;
    }
    private void OnDisable()
    {
        PlayerManager.PlayerJoined -= OnPlayerJoined;
        PlayerManager.PlayerLeft -= OnPlayerLeft;
    }

    public override void Open()
    {
        base.Open();
        if (inputManager != null && !inputManager.enabled) inputManager.enabled = true;

        UpdatePlayerCount();
    }
    public override void Close()
    {
        base.Close();
        if (inputManager != null && inputManager.enabled) inputManager.enabled = false;
        PlayerManager.RemoveAllPlayers();
    }

    public void OnPlayerJoined(PlayerController player)
    {
        UpdatePlayerCount();
        player.Input.actions.FindAction(startGameInputAction).performed += OnStartGameInput;
    }
    void OnPlayerLeft(PlayerController player)
    {
        UpdatePlayerCount();
        player.Input.actions.FindAction(startGameInputAction).performed -= OnStartGameInput;
    }

    void OnStartGameInput(InputAction.CallbackContext context)
    {
        if (onGameStart != null) onGameStart.Invoke();
    }

    void UpdatePlayerCount()
    {
        if (playerCountText != null) playerCountText.text = $"{playerCountPrefix}{PlayerManager.Players.Count}{playerCountSuffix}";

        if (startText != null)
        {
            var active = PlayerManager.Players.Count > 0;
            if (startText.gameObject.activeSelf != active) startText.gameObject.SetActive(active);
        }
    }
}
