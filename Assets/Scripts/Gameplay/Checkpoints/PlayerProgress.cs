using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    Transform currentCheckpoint, nextCheckpoint;

    public float Progress => currentCheckpoint == null || nextCheckpoint == null ? 0.0f : Vector3Extensions.InverseLerp(currentCheckpoint.position, nextCheckpoint.position, transform.position);
    int lapsCompleted;
    public int LapsCompleted => lapsCompleted;

    uint checkpointIndex;
    public uint CheckpointIndex => checkpointIndex;

    private void Start()
    {
        lapsCompleted = -1;

        if (CheckpointManager.Instance == null || CheckpointManager.Instance.NumCheckpoints == 0) return;

        NextCheckpoint(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // before touching the start checkpoint always added a lap, so walking back and forth
        // over the start line could speed the whole race W
        if (CheckpointManager.Instance == null || CheckpointManager.Instance.NumCheckpoints == 0) return;

        if (other.attachedRigidbody != null && CheckpointManager.Instance.TryGetCheckpointIndex(other.attachedRigidbody, out var index))
        {
            uint nextindex = (checkpointIndex + 1) % (uint)CheckpointManager.Instance.NumCheckpoints;
            if (index == nextindex) NextCheckpoint(index);
        }
    }

    void NextCheckpoint(uint currentIndex)
    {
        checkpointIndex = currentIndex;

        currentCheckpoint = CheckpointManager.Instance.GetCheckpointFromIndex(currentIndex);

        nextCheckpoint = CheckpointManager.Instance.GetCheckpointFromIndex(currentIndex + 1);

        if (currentIndex == 0) lapsCompleted++;
    }
}
