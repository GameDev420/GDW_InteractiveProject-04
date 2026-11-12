using UnityEngine;
using UnityEngine.Events;

// Not really needed, but nice to have in case another script needs it during Awake/OnEnable
[DefaultExecutionOrder(-5)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameOverMenuController gameOverMenu;

    // Could also do the same with a GameState enum, but this is simpler for now.
    bool gameStarted = false;
    bool gameOver = false;
    public bool GameStarted => gameStarted;
    public bool GameOver => gameOver;

    public delegate void GameStartEventHandler();
    public delegate void GameOverEventHandler(PlayerController winner);
    public event GameStartEventHandler OnGameStart;
    public event GameOverEventHandler OnGameOver;

    // TODO: convert the above C# events to UnityEvents so they can be set in the inspector (just for better consistency)
    [Header("Events")]
    [SerializeField] UnityEvent onGameStart;
    [SerializeField] UnityEvent<PlayerController> onGameOver;

    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
    }

    private void OnEnable()
    {
        // Not ideal, but I'm too lazy rn.
        var deathController = FindAnyObjectByType<DeathController>();
        if (deathController != null) deathController.OnGameOver.AddListener(EndGame);
        else Debug.LogWarning("No DeathController found in scene. GameManager will not be able to handle game over events.", this);
    }
    private void OnDisable()
    {
        var deathController = FindAnyObjectByType<DeathController>();
        if (deathController != null) deathController.OnGameOver.RemoveListener(EndGame);
    }

    // Called from a button's OnClick event in game scene.
    public void StartGame()
    {
        gameStarted = true;

        foreach (var player in PlayerManager.Players)
        {
            player.Respawn();
        }
        
        gameOver = false;

        if (OnGameStart != null) OnGameStart.Invoke();
        if (onGameStart != null) onGameStart.Invoke();
    }

    void EndGame()
    {
        var winner = PlayerManager.Players.Find((p) => !p.IsDead);
        
        if (gameOverMenu != null)
        {
            var winnerIndex = winner != null ? PlayerManager.Players.IndexOf(winner) : 0; // Assume player 1 wins if no winner is found (e.g. in singleplayer).
            gameOverMenu.Init(new((winnerIndex + 1).ToString()));
            gameOverMenu.Open();
        }

        gameOver = true;

        if (OnGameOver != null) OnGameOver.Invoke(winner);
        if (onGameOver != null) onGameOver.Invoke(winner);
    }
}
