using UnityEngine;

[CreateAssetMenu(fileName = "SizePowerUpData", menuName = "Scriptable Objects/Power Ups/Size Change")]
public class SizePowerUpData : PowerUpData
{
    [SerializeField] [Min(0.4f)] float sizeMultiplier = 1.0f;
    public override PowerUpInstance CreatePowerUpInstance() => new SizePowerUpInstance(this, sizeMultiplier);
}
