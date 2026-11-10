using UnityEngine;

public partial class BlobController
{
    // does the real move for the frame, and stops at ceilings and walls
    void MoveBlob(float seconds)
    {
        Vector3 walked = transform.position - lastposition;
        transform.position = lastposition;
        Physics.SyncTransforms();

        Vector3 start = lastposition;
        Vector3 target = start + walked + velocity * seconds;

        if (hook.IsAttached)
        {
            target = KeepOnRope(target);
        }

        CollisionFlags hits = controller.Move(target - start);

        if (hits.HasFlag(CollisionFlags.Above) && velocity.y > 0f) 
                           velocity.y = 0f;

        bool goingintowall = velocity.x * wallpush < 0f;

        if (hits.HasFlag(CollisionFlags.Sides) && goingintowall) 
                           velocity.x = 0f;

        PopOverCorner(start, target);
        lastposition = transform.position;
    }

    void PopOverCorner(Vector3 start, Vector3 target)
    {
        float wanted = Mathf.Abs(target.x - start.x);
        float moved = Mathf.Abs(transform.position.x - start.x);
        bool stuck = moved < wanted * 0.25f;

        if (controller.isGrounded && !IsGrounded && stuck)
        {
            velocity.y = 0f;
            controller.Move(Vector3.up * 0.12f);
        }
    }
}
