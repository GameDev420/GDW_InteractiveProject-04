using UnityEngine;

public abstract class PowerUpData : ScriptableObject
{
    [Header("General")]
    public string DisplayName;
    public float Duration = 5.0f;

    [Header("Cosmetic")]
    public GameObject PickUpModel;
    public PlayableSFXObject SFX;
    public GameObject VFXPrefab; // for particles and such

    // Probably not the best way to do this, but simple enough for the project.
    public abstract PowerUpInstance CreatePowerUpInstance();
}