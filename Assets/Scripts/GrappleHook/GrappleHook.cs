using UnityEngine;

public enum HookState { Idle, Flying, Attached }

[RequireComponent(typeof(LineRenderer))]
public class GrappleHook : MonoBehaviour
{

    public float fireangle = 60f;
    public float hookspeed = 70f;
    public float maxrange = 22f;

    public PlayableSFXObject grappleSFX;

    public float reelspeed = 10f;

    float wantedropelength;

    public void ReelInTo(float length)
    {
        wantedropelength = Mathf.Min(length, RopeLength);
    }

    public HookState State { get; private set; }
    public Vector3 Tip { get; private set; }
    public float RopeLength { get; private set; }
    public bool IsAttached { get { return State == HookState.Attached; } }

    LineRenderer line;
    CharacterController body;
    int solidlayers; 
    Vector3 firedfrom;
    Vector3 flydirection;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        body = GetComponent<CharacterController>();
        line.positionCount = 2;
        line.enabled = false;
        solidlayers = Physics.DefaultRaycastLayers & ~LayerMask.GetMask("Player");
    }

    public void Tick(bool pressed, bool held, int facing, float seconds)
    {
        if (pressed) Fire(facing);

        if (!held) State = HookState.Idle;
        else if (State == HookState.Flying) Fly(seconds);

        if (State == HookState.Attached)
        {
            RopeLength = Mathf.MoveTowards(RopeLength, wantedropelength, reelspeed * seconds);
        }

        DrawLine();
    }
    public void Cancel()
    {
        State = HookState.Idle;
        line.enabled = false;
    }

    Vector3 Middle()
    {
        return transform.position + body.center;
    }

    void Fire(int facing)
    {
        float radians = fireangle * Mathf.Deg2Rad;
        flydirection = new Vector3(facing * Mathf.Cos(radians), Mathf.Sin(radians), 0f);
        Tip = firedfrom = Middle();
        State = HookState.Flying;

        if (grappleSFX != null && grappleSFX.SFX != null) grappleSFX.SFX.TryPlay(this);
    }

    void Fly(float seconds)
    {
        float step = hookspeed * seconds;

        if (Physics.Raycast(Tip, flydirection, out RaycastHit hit, step, solidlayers, QueryTriggerInteraction.Ignore))
        {
            Land(hit);
            return;
        }

        Tip += flydirection * step;
        if (Vector3.Distance(firedfrom, Tip) >= maxrange) State = HookState.Idle;
    }

    void Land(RaycastHit hit)
    {
        bool hitstrip = hit.collider.GetComponent<GrappleStrip>() != null;

        Tip = hit.point;
        RopeLength = Vector3.Distance(Middle(), hit.point);

        wantedropelength = RopeLength;
        State = hitstrip ? HookState.Attached : HookState.Idle;
    }

    void DrawLine()
    {
        line.enabled = State != HookState.Idle;
        line.SetPosition(0, Middle());
        line.SetPosition(1, Tip);
    }
}
