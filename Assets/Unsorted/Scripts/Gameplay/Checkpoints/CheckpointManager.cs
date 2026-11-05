using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;

    [SerializeField] Rigidbody[] checkpoints = new Rigidbody[0];

    private void Awake()
    {
        Instance = this;
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
}
