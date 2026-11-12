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
    [Tooltip("Adjust this to change how far/behind the camera must be from player progress in the current lap (in normalized time/progress).")]
    [SerializeField] [Range(-1.0f, 1.0f)] float camToPlayerProgressOffset = 0.0f;
    [Tooltip("In meters per second.")]
    [SerializeField] float pathMoveSpeed = 2.0f;
    [SerializeField][Range(0.0f, 1.0f)] float playerRotationWeight = 0.2f;
    [SerializeField] bool zRotationLock = true;

    [Header("Difficulty")]
    [SerializeField] float speedIncreasePerSecond = 0.018f;

    [SerializeField] bool useracespeed = true;

    //zoom out to put everyone 
    [SerializeField] float maxzoomout = 14f; 
    [SerializeField] float zoompadding = 2f; 
    [SerializeField] float zoomspeed = 3f;

    Camera cam;
    float zoomextra;

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

        float race = 1f;
        if (useracespeed) race = CheckpointManager.Current(1f);

        uninfluencedPosition += pathMoveSpeed * race * p.speedMultiplier * deltaTime * (p.point.position - uninfluencedPosition).normalized;

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

        transform.position -= transform.forward * ZoomOut(deltaTime);

    }

    float ZoomOut(float deltaTime)
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null) return 0f;

        // how much the camera sees per unit of distance, minus whatever the closing walls cover
        float tanup = Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float tanside = tanup * cam.aspect;
        if (ClosingWalls.Instance != null)
        {
            tanside *= 1f - 2f * ClosingWalls.Instance.SideCover;
            tanup *= 1f - 2f * ClosingWalls.Instance.TopBottomCover;
        }

        // for each living player, how far back the camera would need to be to fit them on screen
        float wanted = 0f;
        for (int i = 0; i < PlayerManager.Players.Count; i++)
        {
            PlayerController player = PlayerManager.Players[i];
            if (player == null || player.IsDead) continue;

            Vector3 fromcamera = player.transform.position + Vector3.up - transform.position;
            float ahead = Vector3.Dot(fromcamera, transform.forward); // how far in front of the camera
            float sideways = Mathf.Abs(Vector3.Dot(fromcamera, transform.right)) + zoompadding;
            float upways = Mathf.Abs(Vector3.Dot(fromcamera, transform.up)) + zoompadding;

            float neededforside = sideways / tanside - ahead;
            float neededforup = upways / tanup - ahead;
            wanted = Mathf.Max(wanted, neededforside, neededforup);
        }

        // the cap: past this, players too far away go off screen and DeathController knocks them out
        wanted = Mathf.Clamp(wanted, 0f, maxzoomout);

        zoomextra = Mathf.Lerp(zoomextra, wanted, 1f - Mathf.Exp(-zoomspeed * deltaTime));
        return zoomextra;
    }

    void UpdateRotation(float deltaTime)
    {
        // Return if there are no players, or if the game is over or hasn't started yet.
        if (PlayerManager.Players.Count == 0 || (GameManager.Instance != null && (GameManager.Instance.GameOver || !GameManager.Instance.GameStarted))) return;

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

        if (useracespeed) return;

        pathMoveSpeed += speedIncreasePerSecond * deltaTime;
    }

    void OnNewPathPoint(PathPoint point)
    {
        if (point.useRotation && point.point != null) uninfluencedRot = point.point.rotation;
    }

    #endregion
}
