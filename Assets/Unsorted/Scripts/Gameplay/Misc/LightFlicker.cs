using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [System.Serializable]
    public struct LightFlickerConfig
    {
        [Header("Intensity")]
        public float MinIntensityMultiplier;
        public float MaxIntensityMultiplier;

        [Header("Noise")]
        public float NoiseSpeed;
        [Min(0.0f)] public float NoiseScale;
    }

    [SerializeField] LightFlickerConfig flickerConfig;
    [SerializeField] bool independentRandomness = true;
    

    [Header("References")]
    [SerializeField] Light targetLight;
    float initIntensity;
    float time;


	public Light TargetLight => targetLight;
    public LightFlickerConfig FlickerConfig => flickerConfig;

    float randomOffset;

    private void Awake()
    {
        time = 0.0f;
        initIntensity = targetLight.intensity;

        if (targetLight == null) targetLight = GetComponentInChildren<Light>();

        if (independentRandomness) randomOffset = Random.Range(0.0f, 9999f);
    }

    private void Update()
    {
        time += Time.deltaTime * flickerConfig.NoiseSpeed;
        var noise = Mathf.PerlinNoise1D(time + randomOffset) * flickerConfig.NoiseScale;

        targetLight.intensity = initIntensity * Mathf.Lerp(flickerConfig.MinIntensityMultiplier, flickerConfig.MaxIntensityMultiplier, noise);
    }

    public void SetFlickerConfig(LightFlickerConfig config) => flickerConfig = config;
}
