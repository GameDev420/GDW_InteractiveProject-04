using UnityEngine;

public class PowerUpComponent : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] PowerUpData powerUpData;
    [SerializeField] LayerMask playerLayer;

    [Header("Visuals")]
    [SerializeField] float pingPongSpeed = 1.0f;
    [SerializeField] float pingPongHeight = 0.5f;
    [SerializeField] float rotationSpeed = 90.0f;
    [SerializeField] Vector3 localRotationAxesMultiplier = Vector3.one;

    float initialY;

    private void Awake()
    {
        initialY = transform.localPosition.y;
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

        }
    }
}
