using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MaterialColorRandomizer : MonoBehaviour
{
    static Color DefaultColor = Color.white;

    [SerializeField] [Range(0.0f, 1.0f)] float saturationMin = 0.7f, saturationMax = 1.0f;
    [SerializeField] [Range(0.0f, 1.0f)] float valueMin = 0.4f, valueMax = 0.85f;

    Renderer m_renderer;
    Color color = DefaultColor;
    public Color Color => color;

    private void Awake()
    {
        if (m_renderer == null) m_renderer = GetComponent<Renderer>();
        if (m_renderer.material != null && color == DefaultColor)
        {
            SetColor(Random.ColorHSV(0, 1, saturationMin, saturationMax, valueMin, valueMax, 1, 1));
        }
    }

    public void SetColor(Color c)
    {
        color = c;
        
        if (m_renderer == null) m_renderer = GetComponent<Renderer>();
        if (m_renderer.material != null)
        {
            m_renderer.material.color = color;
        }
    }
}
