using UnityEngine;

[CreateAssetMenu(fileName = "SpeedPowerUpData", menuName = "Scriptable Objects/Power Ups/Speed Boost")]
public class SpeedPowerUpData : PowerUpData
{
    public override PowerUpInstance CreatePowerUpInstance() => new SpeedPowerUpInstance(this);
}
