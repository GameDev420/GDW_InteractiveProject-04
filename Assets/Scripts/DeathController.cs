using UnityEngine;
using UnityEngine.Events;

public class DeathController : MonoBehaviour
{
    [Header("General")]
    [Tooltip("Determines how far a player must travel from the camera center to die. Note that this is in viewport coordinates, such that (1, 1) will kill players as soon as they step out of the camera's bounds.")]
    [SerializeField] Vector2 deathViewportBounds = new(1.08f, 1.08f);
    [Tooltip("Offset from the player's position to check for death conditions.")]
    [SerializeField] Vector3 localPlayerOffset = new(0.0f, 0.5f, 0.0f);
    [SerializeField] PlayableSFXObject deathSFX;
    Camera mainCamera;

    [Header("Game Over Config")]
    [SerializeField] bool openMenuOnGameOver = true;
    [SerializeField] GameOverMenuController gameOverMenu;
    [SerializeField] UnityEvent onGameOver;
    public UnityEvent OnGameOver => onGameOver;

    [Header("Debugging")]
    [SerializeField] bool debugMode = false;
    [SerializeField] int numAlivePlayersToWin = 1;

    bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    private void Start()
    {
        mainCamera = Camera.main != null ? Camera.main : FindAnyObjectByType<Camera>();
    }

    private void Update()
    {
        // Return if game hasn't started.
        if (GameManager.Instance != null && !GameManager.Instance.GameStarted) return;

        var numAlivePlayers = 0;

        for (int i = 0; i < PlayerManager.Players.Count; i++)
        {
            var player = PlayerManager.Players[i];

            if (player.IsDead) continue;

            // Bottom-left is (0, 0), top-right is (1, 1)
            var viewportPoint = (Vector2)mainCamera.WorldToViewportPoint(player.transform.TransformPoint(localPlayerOffset));
            var offsetFromCenter = viewportPoint - new Vector2(0.5f, 0.5f);

            if (Mathf.Abs(offsetFromCenter.x) > deathViewportBounds.x * 0.5f || Mathf.Abs(offsetFromCenter.y) > deathViewportBounds.y * 0.5f)
                KillPlayer(player);
            else
                numAlivePlayers++;
        }

        if (!isGameOver && numAlivePlayers <= (debugMode ? numAlivePlayersToWin : 1))
        {
            if (openMenuOnGameOver && gameOverMenu != null) gameOverMenu.Open();
            if (onGameOver != null) onGameOver.Invoke();

            isGameOver = true;
        }
    }

    void KillPlayer(int index) => KillPlayer(PlayerManager.Players[index]);
    void KillPlayer(PlayerController player)
    {
        player.Kill();

        if (deathSFX != null && deathSFX.SFX != null) deathSFX.SFX.TryPlay(this);
    }
}
