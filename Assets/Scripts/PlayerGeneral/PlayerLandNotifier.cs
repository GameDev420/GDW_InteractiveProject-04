using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class PlayerLandNotifier : MonoBehaviour
{
    [SerializeField] UnityEvent<bool> onGroundedChanged;
    public UnityEvent<bool> OnGroundedChanged => onGroundedChanged;

    CharacterController controller;
    bool wasGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        wasGrounded = controller.isGrounded;
    }

    private void LateUpdate()
    {
        if (wasGrounded != controller.isGrounded)
        {
            wasGrounded = controller.isGrounded;
            if (onGroundedChanged != null) onGroundedChanged.Invoke(wasGrounded);
        }
    }
}
