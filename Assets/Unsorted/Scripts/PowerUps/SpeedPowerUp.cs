using UnityEngine;

public class SpeedPowerUpInstance : PowerUpInstance
{
    public SpeedPowerUpInstance(PowerUpData _Data) : base(_Data)
    {

    }

    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);
        Debug.LogError("Speed Boost Power Up not implemented.");
    }
}
