using UnityEngine;

/// <summary>
/// Currently just running SFX.
/// </summary>
public class PlayerSFX : MonoBehaviour
{
    [SerializeField] float distancePerStep = 0.5f;
    [SerializeField] PlayableSFXObject runningSFX;
    [SerializeField] CharacterController controller;
    Vector3 prevPos;
    float distanceTraveled; // resets upon reaching distancePerStep

    private void Update()
    {
        if (runningSFX != null && (controller == null || controller.isGrounded))
        {
            distanceTraveled += Vector3.Distance(transform.position, prevPos);
            if (distanceTraveled >= distancePerStep)
            {
                runningSFX.SFX.TryPlay(this);
                distanceTraveled -= distancePerStep;
            }
        }
    }
}
