using UnityEngine;
using System.Collections;

public class PowerUpComponent : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] PowerUpData powerUpData;
    [SerializeField] LayerMask playerLayer;
    PowerUpInstance powerUpInstance;

    [Header("Visuals")]
    [SerializeField] float pingPongSpeed = 1.0f;
    [SerializeField] float pingPongHeight = 0.5f;
    [SerializeField] float rotationSpeed = 90.0f;
    [SerializeField] Vector3 localRotationAxesMultiplier = Vector3.one;

    float initialY;

    private void Awake()
    {
        initialY = transform.localPosition.y;
        powerUpInstance = powerUpData.CreatePowerUpInstance();
    }

    private void Update()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, initialY + Mathf.PingPong(Time.time * pingPongSpeed, pingPongHeight), transform.localPosition.z);
        transform.localEulerAngles += rotationSpeed * Time.deltaTime * localRotationAxesMultiplier;
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
        powerUpInstance.Pickup(player);
        StartCoroutine(WaitToRemovePowerUp(player));
    }

    IEnumerator WaitToRemovePowerUp(PlayerController player)
    {
        yield return new WaitUntil(() => powerUpInstance.CanRemovePowerUp(player));
        powerUpInstance.RemovePowerUp(player);
        Destroy(gameObject);
    }
}