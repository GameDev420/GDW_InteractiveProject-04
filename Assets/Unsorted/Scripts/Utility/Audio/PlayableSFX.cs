using UnityEngine;

/// <summary>
/// All the required parameters, references, and logic for a basic sound effect to play when convenient.
/// </summary>
[System.Serializable]
public class PlayableSFX
{
    [System.Serializable]
    public class ClipData
    {
        public float BaseVolume = 1.0f;
        public float BasePitch = 1.0f;
        public AudioClip SFX;

        public ClipData()
        {
            BaseVolume = 1.0f;
            BasePitch = 1.0f;
            SFX = null;
        }
    }

    [Header("Parameters")]
    public float MinimumTimeBetweenPlays = 0.0f;

    [Header("References")]
    [Tooltip("When played, one of these will be chosen at random. If there's only one clip, it will play each time.")]
    public ClipData[] Clips;
    public SFXInfo GroupInfo;

    float lastTimeSincePlay = Mathf.NegativeInfinity;

    public bool TryPlay(MonoBehaviour instigator, float delay = 0.0f)
    {
        // Check if one has already been played recently
        if (Mathf.Max(Time.unscaledTime, lastTimeSincePlay + MinimumTimeBetweenPlays) - lastTimeSincePlay < MinimumTimeBetweenPlays || Clips.Length == 0)
            return false;
        
        // Play audio
        var randomClipData = Clips[Random.Range(0, Clips.Length)];
        OneShotAudioManager.PlayOneShot(instigator, randomClipData.SFX, GroupInfo.mixerGroup, randomClipData.BaseVolume * GroupInfo.volumeMultiplier, randomClipData.BasePitch * GroupInfo.pitchMultiplier, delay);
        
        // Reset time tracker
        lastTimeSincePlay = Time.unscaledTime;

        return true;
    }

    public bool TryPlay(MonoBehaviour instigator, out AudioSource source, float delay = 0.0f)
    {
        source = null;

        // Check if one has already been played recently
        if (Mathf.Max(Time.unscaledTime, lastTimeSincePlay + MinimumTimeBetweenPlays) - lastTimeSincePlay < MinimumTimeBetweenPlays || Clips.Length == 0)
            return false;

        // Play audio
        var randomClipData = Clips[Random.Range(0, Clips.Length)];
        source = OneShotAudioManager.PlayOneShot(instigator, randomClipData.SFX, GroupInfo.mixerGroup, randomClipData.BaseVolume * GroupInfo.volumeMultiplier, randomClipData.BasePitch * GroupInfo.pitchMultiplier, delay);

        // Reset time tracker
        lastTimeSincePlay = Time.unscaledTime;

        return true;
    }

    public bool TryPlay(MonoBehaviour instigator, Vector3 position, float delay = 0.0f)
    {
        // Check if one has already been played recently
        if (Mathf.Max(Time.unscaledTime, lastTimeSincePlay + MinimumTimeBetweenPlays) - lastTimeSincePlay < MinimumTimeBetweenPlays || Clips.Length == 0)
            return false;

        // Play audio
        var randomClipData = Clips[Random.Range(0, Clips.Length)];
        OneShotAudioManager.PlayOneShot(instigator, randomClipData.SFX, GroupInfo.mixerGroup, position, randomClipData.BaseVolume * GroupInfo.volumeMultiplier, randomClipData.BasePitch * GroupInfo.pitchMultiplier, delay);

        // Reset time tracker
        lastTimeSincePlay = Time.unscaledTime;

        return true;
    }

    public bool TryPlay(MonoBehaviour instigator, Vector3 position, out AudioSource source, float delay = 0.0f)
    {
        source = null;

        // Check if one has already been played recently
        if (Mathf.Max(Time.unscaledTime, lastTimeSincePlay + MinimumTimeBetweenPlays) - lastTimeSincePlay < MinimumTimeBetweenPlays || Clips.Length == 0)
            return false;

        // Play audio
        var randomClipData = Clips[Random.Range(0, Clips.Length)];
        source = OneShotAudioManager.PlayOneShot(instigator, randomClipData.SFX, GroupInfo.mixerGroup, position, randomClipData.BaseVolume * GroupInfo.volumeMultiplier, randomClipData.BasePitch * GroupInfo.pitchMultiplier, delay);

        // Reset time tracker
        lastTimeSincePlay = Time.unscaledTime;

        return true;
    }

    public void Reset()
    {
        lastTimeSincePlay = Mathf.NegativeInfinity;
    }
}
