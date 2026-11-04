using UnityEngine;

[CreateAssetMenu(fileName = "New SFX Object", menuName = "ScriptableObjects/Playable SFX Object")]
public class PlayableSFXObject : ScriptableObject
{
    public PlayableSFX SFX;

    private void OnEnable()
    {
        ResetSFX();
    }
    private void OnDisable()
    {
        ResetSFX();
    }

    private void OnValidate()
    {
        ResetSFX();
    }

    public void ResetSFX() // Lazy solution to the problem of SFX time since last played not resetting between play modes scene changes.
    {
        if (SFX != null) SFX.Reset();
    }
}
