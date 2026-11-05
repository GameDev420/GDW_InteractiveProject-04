using UnityEngine;

// Not really needed, but nice to have in case another script needs it during Awake/OnEnable
[DefaultExecutionOrder(-5)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] GameOverMenuController gameOverMenu;
    bool gameStarted = false;
    public bool GameStarted => gameStarted;

    public delegate void GameStartEventHandler();
    public delegate void GameOverEventHandler(PlayerController winner);
    public event GameStartEventHandler OnGameStart;
    public event GameOverEventHandler OnGameOver;

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

        if (OnGameStart != null) OnGameStart.Invoke();
    }

    void EndGame()
    {
        var winner = PlayerManager.Players.Find((p) => !p.IsDead);
        Debug.Log("Winner is: " + winner.name + ", who is dead = " + winner.IsDead, winner);

        if (winner != null) gameOverMenu.Init(new((PlayerManager.Players.IndexOf(winner) + 1).ToString()));
        else Debug.LogWarning("Winner could not be found. Game Over screen results will be incorrect.", this);

        gameOverMenu.Open();

        if (OnGameOver != null) OnGameOver.Invoke(winner);
    }
}
