using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [Tooltip("The index of the first checkpoint at the half-way point, at the top-right of the level.")]
    [SerializeField] int upperHalfIndex; // really bad solution but I'm rushing a bit
    [SerializeField] Rigidbody[] checkpoints = new Rigidbody[0];

    readonly List<PlayerProgress> progression = new();

    private void Awake()
    {
        Instance = this;
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

    void OnPlayerJoined(PlayerController player)
    {
        if (player.TryGetComponent(out PlayerProgress prog) && !progression.Contains(prog)) progression.Add(prog);
    }
    void OnPlayerLeft(PlayerController player)
    {
        if (player.TryGetComponent(out PlayerProgress prog) && progression.Contains(prog)) progression.Remove(prog);
    }

    public bool TryGetCheckpointIndex(Rigidbody point, out uint index)
    {
        index = 0;

        if (point == null) return false;

        for (uint i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == point)
            {
                index = i;
                return true;
            }
        }

        return false;
    }
    public Transform GetCheckpointFromIndex(uint index) => checkpoints[index % checkpoints.Length].transform;

    public PlayerController GetLeadingPlayer()
    {
        var highestLap = -1;
        uint highestCheckpoint = 0;
        var highestProgress = -1.0f;
        PlayerController leadingPlayer = null;

        for (int i = 0; i < progression.Count; i++)
        {
            var p = progression[i];
            if (p == null) continue;

            var progress = p.Progress;
            if (p.LapsCompleted > highestLap || (p.LapsCompleted == highestLap && p.CheckpointIndex >= highestCheckpoint && progress > highestProgress))
            {
                highestLap = p.LapsCompleted;
                highestCheckpoint = p.CheckpointIndex;
                highestProgress = progress;
                leadingPlayer = PlayerManager.Players[i];
            }
        }

        return leadingPlayer;
    }
}
