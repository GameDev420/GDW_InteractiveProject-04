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
        /*
        //if ((playerHeadLayer & (1 << other.gameObject.layer)) == 0) return;

        var otherController = other.GetComponentInParent<BlobController>();

        if (otherController == null || otherController.Velocity.sqrMagnitude <= 0.01f) return;

        var moveDir = otherController.Velocity.normalized;
        var rayOrigin = other.transform.position - (moveDir * 1.0f);
        var ray = new Ray(rayOrigin, moveDir);

        if (controller == null) controller = GetComponent<CharacterController>();

        // Return if the raycast does not hit the other player's head collider
        if (!controller.Raycast(ray, out RaycastHit hit, 2.0f)) return;

        print("test3: " + other.gameObject.name);
        if (other.CompareTag("Head") && hit.normal.y > 0.5f)
        {
            // Jump on top of the other player
            velocity.y = headJumpForce;
            print("test4");
        }
        else if (other.CompareTag("Feet"))
        {
            // Get pushed down
            velocity.y = -headPushDownForce;
            print("test5");
        }*/

        // Who needs raycasts anyways
        // Should probably actually calculate/use the hit normal, but yk
        if (other.CompareTag("Feet") && Velocity.y > 0.0f) velocity.y = -headPushDownForce;
    }

    public void OnFeetTriggerEnter(Collider other)
    {
        if (other.CompareTag("Head") && Velocity.y < 0.0f) velocity.y = headJumpForce;
    }
}
