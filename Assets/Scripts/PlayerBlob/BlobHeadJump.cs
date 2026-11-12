using UnityEngine;

public partial class BlobController
{
    [Tooltip("The force applied when jumping on top of another player's head.")]
    public float headJumpForce = 10f;
    [Tooltip("The force applied when being pushed down by another player's head. Automatically negated, so use a positive value to push down.")]
    public float headPushDownForce = 5f;
    public LayerMask playerHeadLayer;

    public void OnHeadTriggerEnter(Collider other)
    {
        var otherRb = other.attachedRigidbody;
        if (otherRb == null) return;

        var moveDir = otherRb.linearVelocity.normalized;
        var rayOrigin = other.transform.position - (moveDir * 1.0f);
        var ray = new Ray(rayOrigin, moveDir);

        // Return if the raycast does not hit the other player's head collider
        if (!controller.Raycast(ray, out RaycastHit hit, 2.0f)) return;
        
        var exactNormal = hit.normal;

        if (hit.collider.gameObject.layer == playerHeadLayer)
        {
            if (hit.normal.y > 0.5f)
            {
                // Jump on top of the other player
                velocity.y = headJumpForce;
            }
            else
            {
                // Get pushed down
                velocity.y = -headPushDownForce;
            }
        }
    }
}
