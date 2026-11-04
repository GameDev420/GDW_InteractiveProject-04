using UnityEngine;
using UnityEngine.Audio;


public class TimeScaleToPitchSync : MonoBehaviour
{
    [SerializeField] float pitchSmoothDelay = 0.1f;
    [SerializeField] float maxPitchSmoothSpeed = 1.0f;
    [Space]
    [Tooltip("The time scale or below at which the pitch uses the 'Pitch At Time Freeze' value instead of time scale.")]
    [SerializeField] float MinimumThreshold = 0.5f;
    [SerializeField] float timeScaleMultiplier = 2.0f;
    [SerializeField] float pitchAtTimeFreeze = 0.8f;
    [SerializeField] string pitchFloatSuffix = "Pitch";
    [SerializeField] AudioMixerGroup[] mixerGroups;
    float[] groupSmoothVelocity;
    string[] paramNames;

    private void Awake()
    {
        groupSmoothVelocity = new float[mixerGroups.Length];

        paramNames = new string[mixerGroups.Length];
        for (int i = 0; i < mixerGroups.Length; i++)
        {
            paramNames[i] = mixerGroups[i].name + pitchFloatSuffix;
        }
    }

    private void LateUpdate()
    {
        if (mixerGroups == null) return;

        for (int i = 0; i < mixerGroups.Length; i++)
        {
            var g = mixerGroups[i];

            var desired = Time.timeScale;
            if (desired <= MinimumThreshold) desired = pitchAtTimeFreeze;
            else desired *= timeScaleMultiplier;

            g.audioMixer.GetFloat(paramNames[i], out var currentPitch);
            g.audioMixer.SetFloat(paramNames[i], Mathf.Clamp01(Mathf.SmoothDamp(currentPitch, desired, ref groupSmoothVelocity[i], pitchSmoothDelay, maxPitchSmoothSpeed, Time.unscaledDeltaTime)));
        }
    }
}
