using UnityEngine;
using UnityEngine.Events;

public class TriggerEventReceiver : MonoBehaviour
{
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerStay;
    public UnityEvent<Collider> onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter != null) onTriggerEnter.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        if (onTriggerStay != null) onTriggerStay.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (onTriggerExit != null) onTriggerExit.Invoke(other);
    }
}
