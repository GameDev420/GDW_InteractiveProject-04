using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreGameBarrier : MonoBehaviour
{
    [System.Serializable]
    struct BarrierData
    {
        public Collider collider;
        public MeshRenderer renderer;
    }

    [Header("General")]
    [SerializeField] bool enableBarriers = true;
    [SerializeField] BarrierData[] barriers = new BarrierData[0];

    [Header("Animation")]
    [SerializeField] string shaderColorName = "_Base_Color";
    [SerializeField] SmoothedValueConfig alphaSmoothing;
    [SerializeField] float onAlpha = 1.0f;
    [SerializeField] float offAlpha = 0.0f;

    private void Awake()
    {
        if (!enableBarriers) SetBarrierEnabled(enableBarriers);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStart += OnGameStart;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStart -= OnGameStart;
    }

    void OnGameStart()
    {
        if (enableBarriers) SetBarrierEnabled(false);
    }

    void SetBarrierEnabled(bool status)
    {
        var renderers = new List<BarrierData>();
        for (int i = 0; i < barriers.Length; i++)
        {
            var b = barriers[i];
            if (b.collider != null) b.collider.enabled = status;
            if (b.renderer != null && b.renderer.material.HasProperty(shaderColorName)) renderers.Add(b);
        }

        if (renderers.Count > 0) StartCoroutine(PlayFadeAnimatiton(renderers, status));
    }

    IEnumerator PlayFadeAnimatiton(List<BarrierData> barriers, bool status)
    {
        const float MarginOfError = 0.025f;
        
        void SetAlpha(float a)
        {
            for (int i = 0; i < barriers.Count; i++)
            {
                var color = barriers[i].renderer.material.GetColor(shaderColorName);
                barriers[i].renderer.material.SetColor(shaderColorName, new Color(color.r, color.b, color.g, a));
            }
        }

        var smoothedAlpha = new SmoothedFloat(alphaSmoothing)
        {
            SmoothedValue = status ? offAlpha : onAlpha,
            TargetValue = status ? onAlpha : offAlpha
        };

        while (Mathf.Abs(smoothedAlpha.SmoothedValue - smoothedAlpha.TargetValue) > MarginOfError)
        {
            yield return null;
            smoothedAlpha.Smooth(Time.deltaTime);
            SetAlpha(smoothedAlpha.SmoothedValue);
        }

        for (int i = 0; i < barriers.Count; i++)
            barriers[i].renderer.enabled = status;
    }
}
