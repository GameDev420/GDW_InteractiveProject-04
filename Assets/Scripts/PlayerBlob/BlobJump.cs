using UnityEngine;

public partial class BlobController
{
    public float jumpspeed = 14f;
    public float gravity = 38f;
    public float fallgravitymultiplier = 1.3f;
    public float maxfallspeed = 26f;
    public float shorthopmultiplier = 0.5f; 
    public float latejumptime = 0.1f; 
    public float earlyjumptime = 0.1f;

    public PlayableSFXObject jumpSFX; // SFX stuff - can be replaced later

    bool canshortenjump;
    float latejumptimer;
    float earlyjumptimer;

    public bool IsGrounded
    {
        get 
        {
            return controller.isGrounded && TouchesSolid(Vector3.down, controller.height * 0.5f + 0.1f); 
        }
    }

    void CheckGround()
    {
        if ((IsGrounded))
        {
            latejumptimer = latejumptime;

        }
    }

    void Jump()
    {
        if (earlyjumptimer <= 0f && walljumppresstimer <= 0f)
            return;

        if (IsGrounded && jumpSFX != null && jumpSFX.SFX != null) jumpSFX.SFX.TryPlay(this);

        if (latejumptimer > 0f && earlyjumptimer > 0f)

            if (latejumptimer > 0f)
        {
            velocity.y = jumpspeed;
            UseUpJump();
        }
        else if (walljumptimer > 0f && !hook.IsAttached)
        {
            WallJump();
            UseUpJump();
            canshortenjump = walljumpshorthop;
        }
    }

    void UseUpJump()
    {
        canshortenjump = true;
        earlyjumptimer = 0f;
        latejumptimer = 0f;
        walljumptimer = 0f;
        walljumppresstimer = 0f;
    }

    void ShortenJump(bool jumpheld)
    {
        if (canshortenjump && !jumpheld && velocity.y > 0f)
        {
            velocity.y *= shorthopmultiplier;

            canshortenjump = false;
        }
    }

    void ApplyGravity(float seconds)
    {
        float gravitynow = gravity;
        
        if (velocity.y < 0f && !IsSwinging())
        {
            gravitynow *= fallgravitymultiplier;
        }


        velocity.y -= gravitynow * seconds;
        
        
        if (velocity.y < -maxfallspeed)
        {
            velocity.y = -maxfallspeed;
        }
    }

    void StayOnGround()
    {
        if (IsGrounded && velocity.y < 0f) velocity.y = -2f;
    }
}
