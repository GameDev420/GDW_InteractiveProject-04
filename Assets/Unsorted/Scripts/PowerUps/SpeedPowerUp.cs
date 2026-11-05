using UnityEngine;

public class SpeedPowerUpInstance : PowerUpInstance
{
    readonly float speedMultiplier; // Would add a better solution for this if the project wasn't due so fast

    public SpeedPowerUpInstance(PowerUpData _Data, float _SpeedMultiplier) : base(_Data)
    {
        speedMultiplier = _SpeedMultiplier;
    }

    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);

        args.Player.GetComponent<TempMovement>().SetSpeedMultiplier(speedMultiplier, Data.Duration);
    }
}
