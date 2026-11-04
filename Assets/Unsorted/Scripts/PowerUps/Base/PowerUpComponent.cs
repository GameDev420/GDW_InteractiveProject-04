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

            if (player != null) Pickup(player);
        }
    }

    void Pickup(PlayerController player)
    {
        powerUpInstance.Pickup(new PowerUpArgs(player));
        StartCoroutine(WaitToRemovePowerUp(player));

        trigger.enabled = false;
        visuals.SetVisibility(false);
    }

    IEnumerator WaitToRemovePowerUp(PlayerController player)
    {
        var args = new PowerUpArgs(player);

        yield return new WaitUntil(() => powerUpInstance.CanRemovePowerUp(args));
        powerUpInstance.RemovePowerUp(args);
        Destroy(gameObject);
    }
}