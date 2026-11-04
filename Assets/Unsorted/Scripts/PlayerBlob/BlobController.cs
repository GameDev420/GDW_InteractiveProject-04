using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(CharacterController), typeof(PlayerController), typeof(GrappleHook))]
public partial class BlobController : MonoBehaviour
{
    public float sideslowdown; 

    CharacterController controller;
    PlayerController player;
    GrappleHook hook;
    TempMovement walking;

    int solidlayers; 
    Vector3 velocity;
    Vector3 lastposition; 
    int facing = 1; 
    float heldx; 

    public Vector3 Velocity { get { return velocity; } }
    public int Facing { get { return facing; } }

    // finds the other parts this script uses
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        player = GetComponent<PlayerController>();
        hook = GetComponent<GrappleHook>();
        walking = GetComponent<TempMovement>();
        solidlayers = Physics.DefaultRaycastLayers & ~LayerMask.GetMask("Player");
    }

    // remembers where we are so the first move starts from the right spot
    void OnEnable()
    {
        lastposition = transform.position;
    }

    
    void OnDisable()
    {
        hook.Cancel();
        if (walking != null) walking.enabled = true;
    }

    // runs after TempMovement each frame, using the buttons from PlayerController
    void Update()
    {
        Tick(player.ReadButtons(), Time.deltaTime);
    }

    
    public void Tick(BlobButtons buttons, float seconds)
    {
        CountDownTimers(buttons, seconds);
        CheckGround();
        CheckWalls();
        LookWhereMoving(buttons.movex);
        UseHook(buttons, seconds);

        if (IsSwinging()) PushAlongSwing(seconds);
        else if (CanSteer()) FadeSideSpeed(seconds);

        Jump();
        ShortenJump(buttons.jumpheld);
        ApplyGravity(seconds);
        SlideDownWall();
        StayOnGround();
        MoveBlob(seconds);
        LetWalkingRun();
    }

    // puts the player somewhere new and clears their speed
    public void ResetTo(Vector3 position)
    {
        controller.enabled = false;
        transform.position = position;
        controller.enabled = true;
        Physics.SyncTransforms();

        lastposition = position;
        velocity = Vector3.zero;
        hook.Cancel();
    }

    // runs down the jump timers, a jump press starts the early jump timer
    void CountDownTimers(BlobButtons buttons, float seconds)
    {
        latejumptimer -= seconds;
        walljumptimer -= seconds;
        nosteertimer -= seconds;
        earlyjumptimer -= seconds;

        if (buttons.jumppressed) earlyjumptimer = earlyjumptime;
    }

    // remembers which way the player is facing, the hook fires that way
    void LookWhereMoving(float movex)
    {
        heldx = movex;

        if (movex > 0f) facing = 1;
        if (movex < 0f) facing = -1;
    }

    // slows down side speed left over from swings and wall jumps
    void FadeSideSpeed(float seconds)
    {
        velocity.x = Mathf.MoveTowards(velocity.x, 0f, sideslowdown * seconds);
    }

    void LetWalkingRun()
    {
        if (walking != null) walking.enabled = CanSteer() && !IsSwinging();
    }

    // your player's position is at its feet, this is the middle of its body
    Vector3 Middle()
    {
        return transform.position + controller.center;
    }

    bool TouchesSolid(Vector3 direction, float reach)
    {
        return Physics.Raycast(Middle(), direction, reach, solidlayers, QueryTriggerInteraction.Ignore);
    }
}
