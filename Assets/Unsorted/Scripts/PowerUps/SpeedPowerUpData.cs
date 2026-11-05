using UnityEngine;

[CreateAssetMenu(fileName = "SpeedPowerUpData", menuName = "Scriptable Objects/Power Ups/Speed Boost")]
public class SpeedPowerUpData : PowerUpData
{
    [SerializeField] [Min(0.3f)] float speedMultiplier = 1.0f;
    public override PowerUpInstance CreatePowerUpInstance() => new SpeedPowerUpInstance(this, speedMultiplier);
}
