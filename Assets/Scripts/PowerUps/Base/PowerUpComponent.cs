using UnityEngine;
using System.Collections;
using static PowerUpInstance;

public class PowerUpComponent : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] PowerUpVisuals visuals;
    [SerializeField] Collider trigger;
    [SerializeField] PowerUpData powerUpData;
    [SerializeField] LayerMask playerLayer;
    PowerUpInstance powerUpInstance;

    [SerializeField] float respawntime = 10f;

    private void Awake()
    {
        powerUpInstance = powerUpData.CreatePowerUpInstance();

        if (visuals == null)
        {
            visuals = GetComponentInChildren<PowerUpVisuals>();
            if (visuals == null)
            {
                Debug.LogError("PowerUpComponent: No PowerUpVisuals assigned or found in children.", this);
                return;
            }
        }

        visuals.Init(powerUpInstance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            var player = other.GetComponentInParent<PlayerController>();

            if (player != null && !powerUpInstance.CanPickup(new PowerUpArgs(player))) return;

            if (player != null) Pickup(player);
        }
    }

    void Pickup(PlayerController player)
    {
        
        PowerUpInstance instance = powerUpData.CreatePowerUpInstance();
        instance.Pickup(new PowerUpArgs(player));

        StartCoroutine(WaitToRemovePowerUp(instance, player));
        StartCoroutine(WaitToRespawn());

        trigger.enabled = false;
        visuals.SetVisibility(false);
    }

    // Name is misleading - changed this to replenish power up now
    IEnumerator WaitToRemovePowerUp(PowerUpInstance instance, PlayerController player)
    {
        var args = new PowerUpArgs(player);

        yield return new WaitUntil(() => instance.CanRemovePowerUp(args));

        instance.RemovePowerUp(args);
    }

    IEnumerator WaitToRespawn()
    {
        yield return new WaitForSeconds(respawntime);

        trigger.enabled = true;
        visuals.SetVisibility(true);
    }
}