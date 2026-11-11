using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [SerializeField] Rigidbody[] checkpoints = new Rigidbody[0];
    readonly List<PlayerProgress> progression = new();

    public int NumCheckpoints => checkpoints.Length;

    public float lapspeedgain = 0.15f; 
    public float speedeasetime = 3f;

    float racespeed = 1f; 
    int mostlaps;

    public float RaceSpeed { get { return racespeed; } }
    public int MostLaps { get { return mostlaps; } }


    public static float Current(float fallback)
    {
        if (Instance == null) return fallback;
        return Instance.racespeed;
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // remember the most laps anyone has finished, even if that player dies later
        foreach (PlayerProgress p in progression)
        {
            if (p != null && p.LapsCompleted > mostlaps) mostlaps = p.LapsCompleted;
        }

        // ease up towards the speed for that many laps, one lap's worth every speedeasetime seconds
        float wanted = 1f + lapspeedgain * mostlaps;
        float rate = 1000f;
        if (speedeasetime > 0f) rate = lapspeedgain / speedeasetime;
        racespeed = Mathf.MoveTowards(racespeed, wanted, rate * Time.deltaTime);
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
    public Transform GetCheckpointFromIndex(uint index)
    {
        return checkpoints[index % checkpoints.Length].transform;
    }

    public PlayerProgress GetLeadingPlayer()
    {
        if (progression.Count == 0) return null;

        var highestLap = -1;
        uint highestCheckpoint = 0;
        var highestProgress = -1.0f;
        PlayerProgress leadingPlayer = null;

        for (int i = 0; i < progression.Count; i++)
        {
            var p = progression[i];
            if (p == null) continue;

            //a dead player can't be the leader or the camera would follow them
            if (p.TryGetComponent(out PlayerController owner) && owner.IsDead) continue;

            var progress = p.Progress;

            //a checkpoint ahead could lose to someone behind who was further between their checkpoints

            bool morelaps = p.LapsCompleted > highestLap;
            bool samelap = p.LapsCompleted == highestLap;
            bool laterpoint = samelap && p.CheckpointIndex > highestCheckpoint;
            bool furtheralong = samelap && p.CheckpointIndex == highestCheckpoint && progress > highestProgress;

            if (morelaps || laterpoint || furtheralong)
            {
                highestLap = p.LapsCompleted;
                highestCheckpoint = p.CheckpointIndex;
                highestProgress = progress;
                leadingPlayer = p;
            }
        }

        return leadingPlayer;
    }

    public float GetLapProgress(PlayerProgress player)
    {
        var step = 1.0f / Mathf.Max(checkpoints.Length, 1);
        return Mathf.Clamp01(player.CheckpointIndex * step + player.Progress * step);
    }

    // This would return negative during the first checkpoint of the first lap, thus .
    public float GetOverallProgress(PlayerProgress player) => player.LapsCompleted + GetLapProgress(player);
}
