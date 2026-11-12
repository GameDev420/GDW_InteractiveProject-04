using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerHitEventReceiver : MonoBehaviour
{
    [SerializeField] UnityEvent<ControllerColliderHit> onControllerColliderHitReceived;
    public UnityEvent<ControllerColliderHit> OnControllerColliderHitReceived => onControllerColliderHitReceived;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (onControllerColliderHitReceived != null) onControllerColliderHitReceived.Invoke(hit);
    }
}
