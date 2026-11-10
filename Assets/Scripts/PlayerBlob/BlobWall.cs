using UnityEngine;

public partial class BlobController
{
    public float walljumpsidespeed = 10f;
    public float wallslidespeed = 4f;
    public float nosteertime = 0.14f; 
    int wallpush; 
    int lastwallpush;
    float walljumptimer;
    float nosteertimer;

    void CheckWalls()
    {
        wallpush = 0;
        if (TouchesSolid(Vector3.left, controller.radius + 0.15f)) wallpush = 1;
        if (TouchesSolid(Vector3.right, controller.radius + 0.15f)) wallpush = -1;

        if (wallpush != 0 && !IsGrounded)
        {
            walljumptimer = latejumptime;
            lastwallpush = wallpush;
        }
    }

    void WallJump()
    {
        velocity = new Vector3(lastwallpush * walljumpsidespeed, jumpspeed, 0f);
        facing = lastwallpush;
        nosteertimer = nosteertime;
    }

    bool CanSteer()
    {
        return nosteertimer <= 0f;
    }

    void SlideDownWall()
    {
        bool holdingintowall = heldx * wallpush < 0f;
        bool inair = !IsGrounded && !IsSwinging();

        if (holdingintowall && inair && velocity.y < -wallslidespeed)
        {
            velocity.y = -wallslidespeed;
        }
    }
}
