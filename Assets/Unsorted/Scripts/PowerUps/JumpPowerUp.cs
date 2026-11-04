using UnityEngine;

public class JumpPowerUpInstance : PowerUpInstance
{
    public JumpPowerUpInstance(PowerUpData _Data) : base(_Data)
    {

    }

    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);

        Debug.LogError("Jump Power Up not implemented.");
    }
}
