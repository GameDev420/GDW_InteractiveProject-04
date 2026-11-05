using UnityEngine;

public abstract class PowerUpData : ScriptableObject
{
    [Header("General")]
    public string DisplayName;
    public float Duration = 5.0f;

    [Header("Appearance")]
    public GameObject PickUpModel;
    public GameObject VFXPrefab;


    // Probably not the best way to do this, but simple enough for the project.
    public abstract PowerUpInstance CreatePowerUpInstance();
}