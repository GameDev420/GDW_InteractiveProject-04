using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Data Types
    
    [System.Serializable]
    public struct PathPoint
    {
        public bool useRotation;
        public Transform point;
    }

    #endregion

    #region Variables

    const float SquareMarginOfPositionalError = 0.01f;

    [Header("Config")]
    [Tooltip("In meters per second.")]
    [SerializeField] float pathMoveSpeed = 2.0f;
    [SerializeField][Range(0.0f, 1.0f)] float playerRotationWeight = 0.2f;
    [SerializeField] bool zRotationLock = true;

    [Header("Pathing")]
    [SerializeField] PathPoint[] path = new PathPoint[0];

    [Header("Smoothing")]
    [SerializeField] SmoothedValueConfig positionSmoothConfig;
    [SerializeField] SmoothedValueConfig rotationSmoothConfig;
    SmoothedVector3 smoothedPosition;
    SmoothedQuaternion smoothedRotation;

    int currentPathPoint;
    Quaternion uninfluencedRot; // Rotation if players didn't exist (used to slerp between player and uninfluenced rotations)

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        uninfluencedRot = transform.rotation;

        smoothedPosition = new SmoothedVector3(positionSmoothConfig);
        smoothedPosition.SmoothedValue = smoothedPosition.TargetValue = transform.position;

        smoothedRotation = new SmoothedQuaternion(rotationSmoothConfig);
        smoothedRotation.SmoothedValue = smoothedRotation.TargetValue = transform.rotation;
    }

    private void LateUpdate()
    {
        UpdatePosition(Time.deltaTime);
        UpdateRotation(Time.deltaTime);
    }

    #endregion

    #region Private Methods

    void UpdatePosition(float deltaTime)
    {
        if (path == null || path.Length == 0) return;

        var p = path[currentPathPoint];

        // Check if we should move to next path point
        // Done before move logic to ensure it's consistent with rotation, even if it's technically delayed by a frame.
        if ((p.point.position - transform.position).sqrMagnitude <= SquareMarginOfPositionalError)
        {
            currentPathPoint = (currentPathPoint + 1) % path.Length;
            p = path[currentPathPoint];
            OnNewPathPoint(p);
        }

        smoothedPosition.TargetValue += pathMoveSpeed * deltaTime * (p.point.position - transform.position).normalized;

        smoothedPosition.Smooth(deltaTime);
        transform.position = smoothedPosition.SmoothedValue;
    }

    void UpdateRotation(float deltaTime)
    {
        if (PlayerManager.Players.Count == 0) return;

        // Update uninfluenced rotation
        // (i.e. if you want to apply interpolation between uninfluenced point rotations)
        //uninfluencedRot = ;

        // Get average player position
        var avgPos = Vector3.zero;
        var validCount = 0;
        for (int i = 0; i < PlayerManager.Players.Count; i++)
        {
            if (PlayerManager.Players[i] == null) continue;
            avgPos += PlayerManager.Players[i].transform.position;
            validCount++;
        }

        smoothedRotation.TargetValue = Quaternion.Lerp(uninfluencedRot, Quaternion.LookRotation((avgPos - transform.position).normalized, Vector3.up), playerRotationWeight);
        //smoothedRotation.TargetValue = Quaternion.Euler(Vector3.Lerp(smoothedRotation.TargetValue.eulerAngles, Quaternion.LookRotation((avgPos - transform.position).normalized, Vector3.up).eulerAngles, playerRotationWeight));

        // Not the most elegant solution, but does for a prototype
        // Helpful since Slerp can change the Z-Axis depending on the provided time.
        if (zRotationLock) smoothedRotation.TargetValue = Quaternion.Euler(new Vector3(smoothedRotation.TargetValue.eulerAngles.x, smoothedRotation.TargetValue.eulerAngles.y, 0.0f));

        // Finally, update and apply smoothed rotation
        smoothedRotation.Smooth(deltaTime);
        transform.rotation = smoothedRotation.SmoothedValue;

        if (zRotationLock) transform.rotation = Quaternion.Euler(smoothedRotation.TargetValue.eulerAngles.x, smoothedRotation.TargetValue.eulerAngles.y, 0.0f);
    }

    void OnNewPathPoint(PathPoint point)
    {
        if (point.useRotation && point.point != null) uninfluencedRot = point.point.rotation;
    }

    #endregion
}
