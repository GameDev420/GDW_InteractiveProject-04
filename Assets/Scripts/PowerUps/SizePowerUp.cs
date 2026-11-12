using UnityEngine;

public class SizePowerUpInstance : PowerUpInstance
{
    readonly float sizeMultiplier;

    public SizePowerUpInstance(PowerUpData _Data, float _SizeMultiplier) : base(_Data)
    {
        sizeMultiplier = _SizeMultiplier;
    }
    public override void Pickup(PowerUpArgs args)
    {
        base.Pickup(args);

        if (args.Player.TryGetComponent(out PlayerSize sizeComponent)) sizeComponent.SetSizeMultiplier(sizeMultiplier, data.Duration);
    }

    public override void RemovePowerUp(PowerUpArgs args)
    {
        base.RemovePowerUp(args);

        if (args.Player.TryGetComponent(out PlayerSize sizeComponent)) sizeComponent.SetSizeMultiplier(1.0f, 0.0f);
    }
}
