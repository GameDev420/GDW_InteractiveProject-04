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
        NextCheckpoint(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && CheckpointManager.Instance.TryGetCheckpointIndex(other.attachedRigidbody, out var index))
            NextCheckpoint(index);
    }

    void NextCheckpoint(uint currentIndex)
    {
        checkpointIndex = currentIndex;

        currentCheckpoint = nextCheckpoint != null ? nextCheckpoint : CheckpointManager.Instance.GetCheckpointFromIndex(currentIndex);

        nextCheckpoint = CheckpointManager.Instance.GetCheckpointFromIndex(currentIndex + 1);

        if (currentIndex == 0) lapsCompleted++;
    }
}
