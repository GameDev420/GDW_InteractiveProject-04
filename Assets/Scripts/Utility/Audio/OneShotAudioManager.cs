using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
public static class OneShotAudioManager
{
    [System.Serializable]
    public struct DynamicSoundFloat
    {
        public float valueMultiplier;
        public AnimationCurve valueCurve;

        public readonly float GetValue(float time) => valueCurve.Evaluate(time) * valueMultiplier;

        public DynamicSoundFloat(float _valueMultiplier, AnimationCurve _valueCurve)
        {
            valueMultiplier = _valueMultiplier;
            valueCurve = _valueCurve;
        }
    }

    private const int InitialPoolSize = 5;
    private const float DefaultVolume = 1.0f, DefaultPitch = 1.0f;
    
    private static ObjectPool<AudioSource> sourcePool;

    public static void Initialize(ObjectPool<AudioSource>.InitializePoolObject initObj)
    {
        sourcePool = new ObjectPool<AudioSource>(InitialPoolSize, initObj);
    }

    public static void StopAllAudio()
    {
        foreach (var obj in sourcePool.AllObjects)
        {
            if (obj == null || !obj.isActiveAndEnabled) continue;

            obj.Stop();
            sourcePool.ReturnObject(obj);
        }
    }

    /// <summary>
    /// Plays a non-looping audio clip with the given settings.
    /// Note that the instigator object is only required to start a coroutine with, so be careful if the given behaviour uses StopAllCoroutines in conjuction!
    /// </summary>
    public static AudioSource PlayOneShot(MonoBehaviour instigator, AudioClip clip, AudioMixerGroup mixer, float volume = DefaultVolume, float pitch = DefaultPitch, float delay = 0, bool useUnscaledTime = false)
    {
        if (instigator == null)
        {
            Debug.LogWarning("Could not play one-shot audio because the given instigator object is null.");
            return null;
        }
        if (clip == null)
        {
            Debug.LogWarning("Could not play one-shot audio because the given audio clip is null");
            return null;
        }

        // scenes without a OneShotAudioInitializer like TestLevel1 never set up the pool,
        // and the crash stopped the jump from happening
        if (sourcePool == null) return null;

        var obj = sourcePool.GetObject();

        obj.clip = clip;
        obj.outputAudioMixerGroup = mixer;
        obj.volume = volume;
        obj.pitch = pitch;
        obj.loop = false;

        obj.spatialBlend = 0.0f;
        obj.transform.position = Vector3.zero;

        if (delay > 0) obj.PlayDelayed(delay);
        else obj.Play();

        instigator.StartCoroutine(WaitToReturnObject(obj, delay, /*useUnscaledTime*/ true)); // realized audio sources always play through unscaled time

        return obj;
    }

    public static AudioSource PlayOneShot(MonoBehaviour instigator, AudioClip clip, AudioMixerGroup mixer, Vector3 position, float volume = DefaultVolume, float pitch = DefaultPitch, float delay = 0, bool useUnscaledTime = false)
    {
        if (instigator == null)
        {
            Debug.LogWarning("Could not play one-shot audio because the given instigator object is null.");
            return null;
        }
        if (clip == null)
        {
            Debug.LogWarning("Could not play one-shot audio because the given audio clip is null");
            return null;
        }

        var obj = sourcePool.GetObject();

        obj.clip = clip;
        obj.outputAudioMixerGroup = mixer;
        obj.volume = volume;
        obj.pitch = pitch;
        obj.loop = false;

        obj.spatialBlend = 1.0f;
        obj.transform.position = position;

        if (delay > 0) obj.PlayDelayed(delay);
        else obj.Play();

        instigator.StartCoroutine(WaitToReturnObject(obj, delay, /*useUnscaledTime*/ true)); // realized audio sources always play through unscaled time

        return obj;
    }

    private static IEnumerator WaitToReturnObject(AudioSource obj, float delay, bool useUnscaledTime)
    {
        var time = delay + obj.clip.length;
        yield return useUnscaledTime ? new WaitForSecondsRealtime(time) : new WaitForSeconds(time);
        if (obj != null) sourcePool.ReturnObject(obj);
    }

    /// <summary>
    /// Plays a non-looping audio clip with the given settings.
    /// Note that the instigator object is only required to start a coroutine with, so be careful if the given behaviour uses StopAllCoroutines in conjuction!
    /// </summary>
    public static AudioSource PlayOneShot(MonoBehaviour instigator, AudioClip clip, AudioMixerGroup mixer, DynamicSoundFloat volume, DynamicSoundFloat pitch, float delay = 0, bool useUnscaledTime = false)
    {
        if (instigator == null)
        {
            Debug.LogWarning("Could not play one-shot audio because the given instigator object is null.");
            return null;
        }
        if (clip == null)
        {
            Debug.LogWarning("Could not play one-shot audio because the given audio clip is null");
            return null;
        }

        var obj = sourcePool.GetObject();

        obj.clip = clip;
        obj.outputAudioMixerGroup = mixer;
        obj.loop = false;

        if (delay > 0) obj.PlayDelayed(delay);
        else obj.Play();

        instigator.StartCoroutine(WaitToReturnObject(obj, volume, pitch, delay, useUnscaledTime));

        return obj;
    }

    private static IEnumerator WaitToReturnObject(AudioSource obj, DynamicSoundFloat volume, DynamicSoundFloat pitch, float delay, bool useUnscaledTime)
    {
        var currentTime = 0f;
        var fullDuration = obj.clip.length + delay;

        while (currentTime < fullDuration && obj != null)
        {
            // Update audio volume and pitch based on current time
            var normalizedTime = currentTime / fullDuration;
            obj.volume = volume.GetValue(normalizedTime);
            obj.pitch = pitch.GetValue(normalizedTime);

            // Wait until next frame
            yield return null;

            // Add new frame's delta time
            currentTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        if (obj != null) sourcePool.ReturnObject(obj);
    }
}