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

    public float walljumpupspeed = 12f;
    public float walljumpforgiveness = 0.15f; 
    public float walljumpfade = 20f;
    public float controlreturntime = 0.25f; 

    public bool walljumpshorthop = false;
    

    public float walljumppresswindow = 0.2f; 
    public float wallgriptime = 0.4f; 
    public float wallslidegain = 8f; 

    float walljumppresstimer; 
    float walltouchtime; 
    bool cameoffwalljump; 

    float walkcontrol = 1f; 
    bool walljumppush; 

    void CheckWalls()
    {
        wallpush = 0;

        if (WallOnSide(Vector3.left)) wallpush = 1;
        if (WallOnSide(Vector3.right)) wallpush = -1;
        

        if (wallpush != 0 && !IsGrounded)
        {
            walljumptimer = walljumpforgiveness;
            lastwallpush = wallpush;
        }
    }

    #region wall jump upgrade: three wall checks instead of one
    bool WallOnSide(Vector3 direction)
    {
        float feet = controller.radius;
        float middle = controller.center.y;
        float head = controller.height - controller.radius;

        return WallAt(feet, direction) || WallAt(middle, direction) || WallAt(head, direction);
    }

    bool WallAt(float height, Vector3 direction)
    {
        Vector3 from = transform.position + Vector3.up * height;
        float reach = controller.radius + 0.15f;
        return Physics.Raycast(from, direction, reach, solidlayers, QueryTriggerInteraction.Ignore);
    }

    void WallJump()
    {
        velocity = new Vector3(lastwallpush * walljumpsidespeed, walljumpupspeed, 0f);
        walkcontrol = 0f;
        walljumppush = true;

        walltouchtime = 0f;
        cameoffwalljump = true;
        walljumppresstimer = 0f;
        facing = lastwallpush;
        nosteertimer = nosteertime;

        if (alwaysrun) rundirection = lastwallpush;
    }

    void ReturnWalkControl(float seconds)
    {
        if (IsSwinging()) walljumppush = false;

        if (CanSteer() && controlreturntime > 0f)
        {
            walkcontrol = Mathf.MoveTowards(walkcontrol, 1f, seconds / controlreturntime);
        }
        else if (CanSteer())
        {
            walkcontrol = 1f;
        }
    }

    bool CanSteer()
    {
        return nosteertimer <= 0f;
    }

    void SlideDownWall(float seconds)
    {
        bool holdingintowall = heldx * wallpush < 0f;
        bool inair = !IsGrounded && !IsSwinging();
        bool onwall = wallpush != 0 && inair;

        if (!inair) cameoffwalljump = false;

        bool holdingon = onwall && (holdingintowall || cameoffwalljump);

        if (!holdingon)
        {
            walltouchtime = 0f;
            return;
        }

        walltouchtime += seconds;

        if (velocity.y > 0f) return;

        if (walltouchtime < wallgriptime)
        {
            velocity.y = 0f; 
        }
        else
        {
            float slidetime = walltouchtime - wallgriptime;
            float slidespeed = Mathf.Min(slidetime * wallslidegain, wallslidespeed);
            if (velocity.y < -slidespeed) velocity.y = -slidespeed;
        }
    }
    #endregion
}
