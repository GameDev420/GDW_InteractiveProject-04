using UnityEngine;

public class ContinuousRotator : MonoBehaviour
{
    [SerializeField] float verticalMoveSpeed = 1.0f;
    [SerializeField] float verticalMoveHeight = 0.5f;
    [Tooltip("In degrees per second.")]
    [SerializeField] Vector3 localAngularSpeed = new(30.0f, 45.0f, 0.0f);
    float initialY;

    private void Awake()
    {
        initialY = transform.localPosition.y;
    }

    private void Update()
    {
        transform.SetLocalPositionAndRotation(
            new Vector3(transform.localPosition.x, initialY + Mathf.Sin(Time.time * verticalMoveSpeed) * verticalMoveHeight * 0.5f + verticalMoveHeight * 0.5f, transform.localPosition.z),
            transform.localRotation * Quaternion.Euler(localAngularSpeed * Time.deltaTime)
        );
    }
}
