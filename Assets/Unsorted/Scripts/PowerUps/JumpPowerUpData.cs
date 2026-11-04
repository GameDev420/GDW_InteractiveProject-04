using UnityEngine;

[CreateAssetMenu(fileName = "JumpPowerUpData", menuName = "Scriptable Objects/Power Ups/Jump Boost")]
public class JumpPowerUpData : PowerUpData
{
    public override PowerUpInstance CreatePowerUpInstance() => new JumpPowerUpInstance(this);
}
