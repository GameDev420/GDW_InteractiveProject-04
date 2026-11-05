using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    Transform currentCheckpoint, nextCheckpoint;

    public float Progress;

    private void Start()
    {
        NextCheckpoint(0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && CheckpointManager.Instance.TryGetCheckpointIndex(other.attachedRigidbody, out var index))
            NextCheckpoint(index);
    }

    void NextCheckpoint(uint currentIndex)
    {
        currentCheckpoint = nextCheckpoint != null ? nextCheckpoint : CheckpointManager.Instance.GetCheckpointFromIndex(currentIndex);

        nextCheckpoint = CheckpointManager.Instance.GetCheckpointFromIndex(currentIndex + 1);
    }
}
