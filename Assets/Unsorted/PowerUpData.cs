using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpData", menuName = "Scriptable Objects/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    [Header("General")]
    public string DisplayName;
    public float Duration = 5.0f;

    [Header("Appearance")]
    public GameObject PickUpModel;


    // Probably not the best way to do this, but simple enough for the project.
    public virtual PowerUpInstance CreatePowerUpInstance() => new(this);
}