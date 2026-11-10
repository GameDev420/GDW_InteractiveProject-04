using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    Vector3 initScale;

    float sizeMultiplier = 1.0f;

    private void Awake()
    {
        initScale = transform.localScale;
    }

    public void SetSizeMultiplier(float multiplier)
    {
        sizeMultiplier = multiplier;

        transform.localScale = initScale * multiplier;
    }
}
