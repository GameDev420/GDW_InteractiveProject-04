using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "New SFX Group Info", menuName = "ScriptableObjects/SFX Group Info")]
public class SFXInfo : ScriptableObject
{
    public float volumeMultiplier = 1.0f;
    public float pitchMultiplier = 1.0f;
    public AudioMixerGroup mixerGroup;
}
