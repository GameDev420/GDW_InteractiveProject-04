using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Data Types
    
    [System.Serializable]
    public struct PathPoint
    {
        public bool useRotation;
        public float speedMultiplier;
        public Transform point;
    }

    #endregion

    #region Variables

    const float SquareMarginOfPositionalError = 0.01f;


    [Header("Config")]
    [SerializeField] bool allowLeadingPlayerToControlPosition = false;
    [SerializeField] [Min(0.0f)] float leadingPlayerWeightMultiplier = 2.0f;
    [Tooltip("Adjust this to change how far/behind the camera must be from player progress in the current lap (in normalized time/progress).")]
    [SerializeField] [Range(-1.0f, 1.0f)] float camToPlayerProgressOffset = 0.0f;
    [Tooltip("In meters per second.")]
    [SerializeField] float pathMoveSpeed = 2.0f;
    [SerializeField][Range(0.0f, 1.0f)] float playerRotationWeight = 0.2f;
    //[Tooltip("Determines how much of the playerRotationWeight is influenced by the leading player. For example, a rotation weight (above variable) of 0.5 means the camera's rotation will be evenly split between the path and players, and a leading weight value of, say, 0.3 means the leading player will take up 30% of the initial 50% from the base player weight, whereas all other players will fill the rest. Note that this does round up when there's less players (e.g. leading weight of 0.3 will be considered 0.5 if there's only two players).")]
    //[SerializeField][Range(0.0f, 1.0f)] float leadingPlayerRotationWeight = 0.3f;
    //[Tooltip("Multiplier to determine how much the leading player's position affects the average player position to look toward.")]
    //[SerializeField][Range(1.0f, 10.0f)] float leadingPlayerImportance = 2.0f;
    [SerializeField] bool zRotationLock = true;

    [Header("Difficulty")]
    [SerializeField] float speedIncreasePerSecond = 0.018f;

    [Header("Pathing")]
    [SerializeField] Vector3 worldSpacePlayerOffset = new(0.0f, 5.0f, -10.0f);
    [SerializeField] PathPoint[] path = new PathPoint[0];

    [Header("Smoothing")]
    [SerializeField] SmoothedValueConfig positionSmoothConfig;
    [SerializeField] SmoothedValueConfig rotationSmoothConfig;
    SmoothedVector3 smoothedPosition;
    SmoothedQuaternion smoothedRotation;

    bool movementEnabled = false;
    int currentPathPoint; // the index of the DESIRED path point (the one we're moving toward)
    Vector3 uninfluencedPosition;
    Quaternion uninfluencedRot; // Rotation if players didn't exist (used to slerp between player and uninfluenced rotations)

    float maxProgress;

    uint numLaps = 0;
    
    float LapProgress
    {
        get
        {
            var step = 1.0f / Mathf.Max(path.Length, 1.0f);

            // Only doing this because modulo operation doesn't work properly with negatives (iirc)
            var prevDesiredPointIndex = currentPathPoint == 0 ? path.Length - 1 : currentPathPoint - 1;

            var currentPointProgress = Vector3Extensions.InverseLerp(
                path[prevDesiredPointIndex].point.position,
                path[currentPathPoint % path.Length].point.position,
                uninfluencedPosition
            );
            return Mathf.Clamp01((currentPathPoint - 1) * step + currentPointProgress * step);
        }
    }
    float OverallProgress => numLaps + LapProgress;

    // Stores camera's position on the path WITHOUT following the leading player.

    #endregion

    #region MonoBehaviour Methods

    private void Awake()
    {
        uninfluencedRot = transform.rotation;

        smoothedPosition = new SmoothedVector3(positionSmoothConfig);
        smoothedPosition.SmoothedValue = smoothedPosition.TargetValue = uninfluencedPosition = transform.position;

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
        UpdatePosition(Time.deltaTime);
        UpdateRotation(Time.deltaTime);
        UpdateDifficulty(Time.deltaTime);
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
        if ((p.point.position - uninfluencedPosition).sqrMagnitude <= SquareMarginOfPositionalError)
        {
            currentPathPoint = (currentPathPoint + 1) % path.Length;
            if (currentPathPoint == 0) numLaps++;
            p = path[currentPathPoint];
            OnNewPathPoint(p);
        }

        uninfluencedPosition += pathMoveSpeed * p.speedMultiplier * deltaTime * (p.point.position - uninfluencedPosition).normalized;

        // HOLY INEFFICIENCY BTW IM SO SORRY (this whole section, esp. running each frame)
        // Calculate leading player
        var leadingPlayer = CheckpointManager.Instance.GetLeadingPlayer();
        var leadingProgress = CheckpointManager.Instance.GetOverallProgress(leadingPlayer);
        if (leadingProgress > maxProgress) maxProgress = leadingProgress;

        // Uninfluenced Camera movement
        if (!allowLeadingPlayerToControlPosition || OverallProgress + camToPlayerProgressOffset > maxProgress)
            smoothedPosition.TargetValue = uninfluencedPosition;
        // Follow leading player (if they're ahead of camera)
        else
        {
            /*var validPlayers = 0;

            // Different from the average rotation position since the leading player may be weighted differently in each
            // Still could definitely be optimized
            var avgPos = Vector3.zero;
            for (int i = 0; i < PlayerManager.Players.Count; i++)
            {
                var player = PlayerManager.Players[i];
                if (player == null || player.IsDead) continue;

                if (player.gameObject == leadingPlayer.gameObject) avgPos += player.transform.position * leadingPlayerWeightMultiplier;
                else avgPos += player.transform.position;

                validPlayers++;
            }*/

            //smoothedPosition.TargetValue = (avgPos / Mathf.Max(validPlayers + leadingPlayerWeightMultiplier - 1, 1)) + worldSpacePlayerOffset;

            smoothedPosition.TargetValue = leadingPlayer.transform.position + worldSpacePlayerOffset;
        }


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
            if (PlayerManager.Players[i] == null || PlayerManager.Players[i].IsDead) continue;
            avgPos += PlayerManager.Players[i].transform.position;

            validCount++;
        }

        avgPos /= validCount;

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

    void UpdateDifficulty(float deltaTime)
    {
        if (!movementEnabled) return;

        pathMoveSpeed += speedIncreasePerSecond * deltaTime;
    }

    void OnNewPathPoint(PathPoint point)
    {
        if (point.useRotation && point.point != null) uninfluencedRot = point.point.rotation;
    }

    #endregion
}
