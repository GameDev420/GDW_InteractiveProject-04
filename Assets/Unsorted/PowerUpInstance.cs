using UnityEngine;

public class PowerUpInstance
{
    public PowerUpInstance(PowerUpData Data)
    {
        data = Data;
    }

    readonly protected PowerUpData data;
    protected float pickupTime;

    public virtual void Pickup(PlayerController player)
    {
        pickupTime = Time.time;

        Debug.Log("Activating powerup.");
    }

    public virtual void RemovePowerUp(PlayerController player)
    {
        Debug.Log("Removing powerup.");
    }

    public virtual bool CanPickup(PlayerController player)
    {
        return true;
    }

    /// <summary>
    /// Determines if the power-up can be removed from the player. This is useful for power-ups that have a duration or specific conditions for removal.
    /// </summary>
    public virtual bool CanRemovePowerUp(PlayerController player)
    {
        return Time.time - pickupTime >= data.Duration;
    }
}