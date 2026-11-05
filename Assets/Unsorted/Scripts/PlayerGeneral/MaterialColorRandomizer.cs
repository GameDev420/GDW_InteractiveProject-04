using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialColorRandomizer : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 1.0f)] float saturationMin = 0.7f, saturationMax = 1.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float valueMin = 0.4f, valueMax = 0.85f;
    private void Awake()
    {
        var r = GetComponent<Renderer>();
        if (r.material != null) r.material.color = Random.ColorHSV(0, 1, saturationMin, saturationMax, valueMin, valueMax, 1, 1);
    }
}
