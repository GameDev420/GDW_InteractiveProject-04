using UnityEngine;

public class JumpPowerUpInstance : PowerUpInstance
{
    readonly float jumpMultiplier;

    public JumpPowerUpInstance(PowerUpData _Data, float _JumpMultiplier) : base(_Data)
    {
        jumpMultiplier = _JumpMultiplier;
    }

    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);

        if (args.Player.TryGetComponent(out BlobController blob)) blob.jumpspeed *= jumpMultiplier;
    }

    public override void RemovePowerUp(PowerUpArgs args)
    {
        base.RemovePowerUp(args);

        if (args.Player.TryGetComponent(out BlobController blob)) blob.jumpspeed /= jumpMultiplier;
    }
}
