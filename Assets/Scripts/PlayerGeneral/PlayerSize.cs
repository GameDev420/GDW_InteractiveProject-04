using UnityEngine;
using UnityEngine.Events;

public class PlayerSize : MonoBehaviour
{
    Vector3 initScale;
    float sizeMultiplier = 1.0f;
    [SerializeField] UnityEvent<float> onSizeChanged;

    public float SizeMultiplier => sizeMultiplier;
    public UnityEvent<float> OnSizeChanged => onSizeChanged;

    private void Awake()
    {
        initScale = transform.localScale;
    }

    public void SetSizeMultiplier(float multiplier)
    {
        sizeMultiplier = multiplier;

        transform.localScale = initScale * multiplier;
        if (onSizeChanged != null) onSizeChanged.Invoke(multiplier);
    }
}
