using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleFinishNotifier : MonoBehaviour
{
    [SerializeField] bool disableOnEventFired = true;
    [SerializeField] bool destroyOnEventFired = false;
    [SerializeField] UnityEvent onParticleFinish;

    ParticleSystem particles;
    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particles.isPlaying)
        {
            if (onParticleFinish != null) onParticleFinish.Invoke();

            if (destroyOnEventFired) Destroy(gameObject);
            else if (disableOnEventFired) enabled = false;
        }
    }
}
