using UnityEngine;
using UnityEngine.Events;

public class PlayerSize : MonoBehaviour
{
    [SerializeField] TempMovement walking;
    Vector3 initScale;
    float sizeMultiplier = 1.0f;
    [SerializeField] UnityEvent<float> onSizeChanged;

    public float SizeMultiplier => sizeMultiplier;
    public UnityEvent<float> OnSizeChanged => onSizeChanged;

    private void Awake()
    {
        initScale = transform.localScale;
    }

    // TODO: create similar system to TempMovement's SetSpeedMultiplier, but for size (or make it reusable)
    // ^ I can do this if you want, just remind me later
    public void SetSizeMultiplier(float multiplier, float duration) // 'duration' only used for movement speed multiplier
    {
        sizeMultiplier = multiplier;

        transform.localScale = initScale * multiplier;
        if (onSizeChanged != null) onSizeChanged.Invoke(multiplier);

        if (walking != null) walking.SetSpeedMultiplier(multiplier, duration);
    }
}
