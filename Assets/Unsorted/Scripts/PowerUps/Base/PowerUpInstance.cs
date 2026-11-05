using UnityEngine;

public abstract class PowerUpInstance
{
    public struct PowerUpArgs
    {
        public PlayerController Player;

        public PowerUpArgs(PlayerController player)
        {
            Player = player;
        }
    }

    public PowerUpInstance(PowerUpData _Data)
    {
        data = _Data;
    }

    readonly protected PowerUpData data;
    protected float pickupTime;

    public PowerUpData Data => data;

    public virtual void Pickup(PowerUpArgs args)
    {
        pickupTime = Time.time;

        if (Data.VFXPrefab != null && args.Player.TryGetComponent(out PlayerVFX vfxComponent)) vfxComponent.SpawnVFX(Data.VFXPrefab, Data.Duration);
    }

    public virtual void RemovePowerUp(PowerUpArgs args)
    {
        
    }

    public virtual bool CanPickup(PowerUpArgs args)
    {
        return true;
    }

    /// <summary>
    /// Determines if the power-up can be removed from the player. This is useful for power-ups that have a duration or specific conditions for removal.
    /// </summary>
    public virtual bool CanRemovePowerUp(PowerUpArgs args)
    {
        return Time.time - pickupTime >= data.Duration;
    }
}