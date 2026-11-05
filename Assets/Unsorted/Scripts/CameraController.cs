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

    public float progress;
    public int laps;

    [Header("Config")]
    [Tooltip("WORK IN PROGRESS: LIKELY BROKEN.")]
    [SerializeField] bool allowLeadingPlayerToControlPosition = false;
    [Tooltip("In meters per second.")]
    [SerializeField] float pathMoveSpeed = 2.0f;
    [SerializeField][Range(0.0f, 1.0f)] float playerRotationWeight = 0.2f;
    //[Tooltip("Determines how much of the playerRotationWeight is influenced by the leading player. For example, a rotation weight (above variable) of 0.5 means the camera's rotation will be evenly split between the path and players, and a leading weight value of, say, 0.3 means the leading player will take up 30% of the initial 50% from the base player weight, whereas all other players will fill the rest. Note that this does round up when there's less players (e.g. leading weight of 0.3 will be considered 0.5 if there's only two players).")]
    //[SerializeField][Range(0.0f, 1.0f)] float leadingPlayerRotationWeight = 0.3f;
    //[Tooltip("Multiplier to determine how much the leading player's position affects the average player position to look toward.")]
    //[SerializeField][Range(1.0f, 10.0f)] float leadingPlayerImportance = 2.0f;
    [SerializeField] bool zRotationLock = true;

    [Header("Pathing")]
    [SerializeField] Vector3 worldSpacePlayerOffset = new(0.0f, 5.0f, -10.0f);
    [SerializeField] PathPoint[] path = new PathPoint[0];

    [Header("Smoothing")]
    [SerializeField] SmoothedValueConfig positionSmoothConfig;
    [SerializeField] SmoothedValueConfig rotationSmoothConfig;
    SmoothedVector3 smoothedPosition;
    SmoothedQuaternion smoothedRotation;

    bool movementEnabled = false;
    int currentPathPoint;
    Quaternion uninfluencedRot; // Rotation if players didn't exist (used to slerp between player and uninfluenced rotations)

    int numLaps = 0;
    
    // Holy inefficiency btw
    // ^ update: AND READABILITY
    float LapProgress
        => Mathf.Clamp01(
            currentPathPoint / Mathf.Max(path.Length, 1.0f)
            + Vector3Extensions.InverseLerp(
                path[currentPathPoint].point.position,
                path[(currentPathPoint + 1) % path.Length].point.position,
                Vector3.Project(
                    path[(currentPathPoint + 1) % path.Length].point.position - regularPosition,
                    path[(currentPathPoint + 1) % path.Length].point.position - path[currentPathPoint].point.position) + path[currentPathPoint].point.position
                )
            * (1.0f / Mathf.Max(path.Length, 1.0f)));
    float OverallProgress => numLaps + LapProgress;

    // Stores camera's position on the path WITHOUT following the leading player.
    Vector3 regularPosition;

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        uninfluencedRot = transform.rotation;

        smoothedPosition = new SmoothedVector3(positionSmoothConfig);
        smoothedPosition.SmoothedValue = smoothedPosition.TargetValue = regularPosition = transform.position;

        smoothedRotation = new SmoothedQuaternion(rotationSmoothConfig);
        smoothedRotation.SmoothedValue = smoothedRotation.TargetValue = transform.rotation;
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart += OnGameStart;
            GameManager.Instance.OnGameOver += OnGameOver;
        }
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStart -= OnGameStart;
            GameManager.Instance.OnGameOver -= OnGameOver;
        }
    }

    private void LateUpdate()
    {
        laps = numLaps;
        progress = OverallProgress;

        UpdatePosition(Time.deltaTime);
        UpdateRotation(Time.deltaTime);
    }

    #endregion

    #region Private Methods

    void OnGameStart() => movementEnabled = true;
    void OnGameOver(PlayerController player) => movementEnabled = false;

    void UpdatePosition(float deltaTime)
    {
        if (path == null || path.Length == 0 || !movementEnabled) return;

        var p = path[currentPathPoint];

        // Check if we should move to next path point
        // Done before move logic to ensure it's consistent with rotation, even if it's technically delayed by a frame.
        if ((p.point.position - regularPosition).sqrMagnitude <= SquareMarginOfPositionalError)
        {
            currentPathPoint = (currentPathPoint + 1) % path.Length;
            if (currentPathPoint == 0) numLaps++;
            p = path[currentPathPoint];
            OnNewPathPoint(p);
        }

        regularPosition += pathMoveSpeed * deltaTime * (p.point.position - regularPosition).normalized;

        if (allowLeadingPlayerToControlPosition)
        {
            // HOLY INEFFICIENCY BTW IM SO SORRY
            var leadingPlayer = CheckpointManager.Instance.GetLeadingPlayer();
            if (OverallProgress > CheckpointManager.Instance.GetOverallProgress(leadingPlayer)) smoothedPosition.TargetValue = regularPosition;
            else smoothedPosition.TargetValue = leadingPlayer.transform.position + worldSpacePlayerOffset;
        }
        else smoothedPosition.TargetValue = regularPosition;

        smoothedPosition.Smooth(deltaTime);
        transform.position = smoothedPosition.SmoothedValue;
    }

    void UpdateRotation(float deltaTime)
    {
        if (PlayerManager.Players.Count == 0) return;

        // Get average player position
        var avgPos = Vector3.zero;
        var validCount = 0;

        for (int i = 0; i < PlayerManager.Players.Count; i++)
        {
            if (PlayerManager.Players[i] == null) continue;
            avgPos += PlayerManager.Players[i].transform.position;

            validCount++;
        }

        avgPos /= validCount; // TODO: divide by total weight

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
