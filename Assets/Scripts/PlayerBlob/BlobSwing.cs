using UnityEngine;

public partial class BlobController
{
    public float swingdrive;
    public float maxswingspeed;
    public float swingreleaseboost;

    public float grapplehop = 6f;

    public float swingfloorgap = 0.5f;

    bool IsSwinging()
    {
        return hook.IsAttached && !IsGrounded;
    }


    void UseHook(BlobButtons buttons, float seconds)
    {
        bool washooked = hook.IsAttached;
        hook.Tick(buttons.grapplepressed, buttons.grappleheld, Facing, seconds);

        bool letgo = washooked && !hook.IsAttached;
        if (letgo && !IsGrounded) velocity *= swingreleaseboost;

        bool justcaught = !washooked && hook.IsAttached;
        if (justcaught && IsGrounded && grapplehop > 0f)
        {
            velocity.y = grapplehop;

            // the rope was as long as the gap from the floor to the hook, so the bottom of
            // the swing went into the floor and stopped you. reel it in so the bottom of
            // the swing passes swingfloorgap above where you're standing
            float heightbelowhook = hook.Tip.y - Middle().y;
            float safelength = heightbelowhook - swingfloorgap;
            hook.ReelInTo(Mathf.Max(safelength, 1f));
        }
    }

    void PushAlongSwing(float seconds)
    {
        Vector3 tohook = (hook.Tip - Middle()).normalized;
        Vector3 alongswing = new Vector3(tohook.y, -tohook.x, 0f); // sideways to the rope

        velocity += alongswing * (facing * swingdrive * RaceSpeedNow() * seconds);
    }

    Vector3 KeepOnRope(Vector3 target)
    {
        Vector3 fromhook = target + controller.center - hook.Tip;
        if (fromhook.magnitude <= hook.RopeLength) return target;

        Vector3 awayfromhook = fromhook.normalized;
        float speedaway = Vector3.Dot(velocity, awayfromhook);
        if (speedaway > 0f) velocity -= awayfromhook * speedaway;

        velocity = Vector3.ClampMagnitude(velocity, maxswingspeed * RaceSpeedNow());
        return hook.Tip + awayfromhook * hook.RopeLength - controller.center;
    }
}
