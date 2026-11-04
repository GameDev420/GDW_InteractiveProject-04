using UnityEngine;

public class SizePowerUpInstance : PowerUpInstance
{
    public SizePowerUpInstance(PowerUpData _Data) : base(_Data)
    {
        
    }
    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);
        Debug.LogError("Size Power Up not implemented.");
    }
}
