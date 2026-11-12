using UnityEngine;

/// <summary>
/// Currently just running SFX, but should be migrated to support most other player sound effects if possible (for organization).
/// </summary>
public class PlayerSFX : MonoBehaviour
{
    [SerializeField] float distancePerStep = 0.5f;
    [SerializeField] PlayableSFXObject runningSFX;
    [SerializeField] CharacterController controller;
    Vector3 prevPos;
    float distanceTraveled; // resets upon reaching distancePerStep

    private void Awake() => prevPos = GetCurrentPos();
    private void Update()
    {
        if (runningSFX != null && (controller == null || controller.isGrounded))
        {
            var currentPos = GetCurrentPos();
            distanceTraveled += Vector3.Distance(currentPos, prevPos);

            if (distanceTraveled >= distancePerStep)
            {
                runningSFX.SFX.TryPlay(this);
                distanceTraveled -= distancePerStep;
            }

            prevPos = currentPos;
        }
        else prevPos = GetCurrentPos();
    }

    Vector3 GetCurrentPos() => Vector3.ProjectOnPlane(transform.position, transform.up);
}
