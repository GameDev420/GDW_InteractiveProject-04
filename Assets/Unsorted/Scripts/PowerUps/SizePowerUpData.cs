using UnityEngine;

[CreateAssetMenu(fileName = "SizePowerUpData", menuName = "Scriptable Objects/Power Ups/Size Change")]
public class SizePowerUpData : PowerUpData
{
    public override PowerUpInstance CreatePowerUpInstance() => new SizePowerUpInstance(this);
}
