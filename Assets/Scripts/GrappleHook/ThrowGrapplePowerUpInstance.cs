using UnityEngine;

[CreateAssetMenu(fileName = "ThrowGrapplePowerUpData", menuName = "Scriptable Objects/Power Ups/Throw Grapple")]
public class ThrowGrapplePowerUp : PowerUpData
{
    public override PowerUpInstance CreatePowerUpInstance()
    {
        return new ThrowGrapplePowerUpInstance(this);
    }
}
