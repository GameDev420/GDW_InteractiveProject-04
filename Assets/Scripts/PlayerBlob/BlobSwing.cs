using UnityEngine;

public partial class BlobController
{
    public float swingdrive;
    public float maxswingspeed;
    public float swingreleaseboost;

    // true when hanging from the hook in the air
    bool IsSwinging()
    {
        return hook.IsAttached && !IsGrounded;
    }

    // runs the hook and letting go mid swing gives a speed boost
    void UseHook(BlobButtons buttons, float seconds)
    {
        bool washooked = hook.IsAttached;
        hook.Tick(buttons.grapplepressed, buttons.grappleheld, Facing, seconds);

        bool letgo = washooked && !hook.IsAttached;
        if (letgo && !IsGrounded) velocity *= swingreleaseboost;
    }

    void PushAlongSwing(float seconds)
    {
        Vector3 tohook = (hook.Tip - Middle()).normalized;
        Vector3 alongswing = new Vector3(tohook.y, -tohook.x, 0f); // sideways to the rope

        velocity += alongswing * (facing * swingdrive * seconds);
    }

    Vector3 KeepOnRope(Vector3 target)
    {
        Vector3 fromhook = target + controller.center - hook.Tip;
        if (fromhook.magnitude <= hook.RopeLength) return target;

        Vector3 awayfromhook = fromhook.normalized;
        float speedaway = Vector3.Dot(velocity, awayfromhook);
        if (speedaway > 0f) velocity -= awayfromhook * speedaway;

        velocity = Vector3.ClampMagnitude(velocity, maxswingspeed);
        return hook.Tip + awayfromhook * hook.RopeLength - controller.center;
    }
}
