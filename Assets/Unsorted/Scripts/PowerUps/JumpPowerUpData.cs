using UnityEngine;

[CreateAssetMenu(fileName = "JumpPowerUpData", menuName = "Scriptable Objects/Power Ups/Jump Boost")]
public class JumpPowerUpData : PowerUpData
{
    [SerializeField] [Min(0.1f)] float jumpMultiplier = 1.0f;
    public override PowerUpInstance CreatePowerUpInstance() => new JumpPowerUpInstance(this, jumpMultiplier);
}
