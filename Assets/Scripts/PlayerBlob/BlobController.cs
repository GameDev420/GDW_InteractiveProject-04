using UnityEngine;

[DefaultExecutionOrder(100)]
[RequireComponent(typeof(CharacterController), typeof(PlayerController), typeof(GrappleHook))]
public partial class BlobController : MonoBehaviour
{
    public float sideslowdown;


    public Transform model;

    // degrees per second 1800 spins all the way round in .1 seconds
    public float turnspeed = 1800f; 
    public bool modelfacesright = true;

    // the models rotation when facing right
    Quaternion facingright; 

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

        if (model != null)
        {
            facingright = model.localRotation;
            if (!modelfacesright) facingright = Quaternion.Euler(0f, 180f, 0f) * facingright;
        }
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
        ReturnWalkControl(seconds);
        CountDownTimers(buttons, seconds);
        CheckGround();
        CheckWalls();
        LookWhereMoving(buttons.movex);
        UseHook(buttons, seconds);

        if (IsSwinging()) PushAlongSwing(seconds);
        else if (CanSteer()) FadeSideSpeed(seconds);


        SlideDownWall(seconds);
        Jump();
        ShortenJump(buttons.jumpheld);
        ApplyGravity(seconds);
        StayOnGround();
        MoveBlob(seconds);
        LetWalkingRun();

        TurnModel(seconds);

    }


    void TurnModel(float seconds)
    {
        if (model == null) return;

        Quaternion wanted = facingright;
        if (facing < 0) wanted = Quaternion.Euler(0f, 180f, 0f) * facingright;

        // turn a little each frame, so it spins instead of snapping
        model.localRotation = Quaternion.RotateTowards(model.localRotation, wanted, turnspeed * seconds);
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

        walkcontrol = 1f;
        walljumppush = false;
    }

    // runs down the jump timers, a jump press starts the early jump timer
    void CountDownTimers(BlobButtons buttons, float seconds)
    {
        latejumptimer -= seconds;
        walljumptimer -= seconds;
        nosteertimer -= seconds;
        earlyjumptimer -= seconds;
        walljumppresstimer -= seconds;

        if (buttons.jumppressed) earlyjumptimer = earlyjumptime;

        if (buttons.jumppressed) walljumppresstimer = walljumppresswindow;
    }

    // remembers which way the player is facing, the hook fires that way
    void LookWhereMoving(float movex)
    {
        heldx = movex;

        if (movex > 0f) facing = 1;
        if (movex < 0f) facing = -1;

        if (alwaysrun) PickRunDirection(movex);
    }

    // slows down side speed left over from swings and wall jumps
    void FadeSideSpeed(float seconds)
    {
        float fade = sideslowdown;

        if (walljumppush && IsGrounded) fade = walljumpfade;

        velocity.x = Mathf.MoveTowards(velocity.x, 0f, fade * seconds);

        if (velocity.x == 0f) walljumppush = false;
       
    }

    void LetWalkingRun()
    {
        if (alwaysrun)
        {
            if (walking != null) walking.enabled = false;
            return;
        }
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
