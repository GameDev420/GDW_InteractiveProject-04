using UnityEngine;

// picking this up gives you one throw grapple, you keep it until you use it
public class ThrowGrapplePowerUpInstance : PowerUpInstance
{
    ThrowGrapple holder;

    public ThrowGrapplePowerUpInstance(PowerUpData _Data) : base(_Data)
    {
    }

    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);

        holder = args.Player.GetComponent<ThrowGrapple>();
        if (holder == null) holder = args.Player.gameObject.AddComponent<ThrowGrapple>();

        holder.hascharge = true;
    }

    // the pickup comes back once the grapple has been used
    public override bool CanRemovePowerUp(PowerUpArgs args)
    {
        return holder == null || !holder.hascharge;
    }
}
