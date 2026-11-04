using UnityEngine;
using UnityEngine.InputSystem;

public class TempMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5.0f;
    Vector2 moveInput;

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        var movement = Camera.main.transform.TransformDirection(moveInput.x, 0.0f, moveInput.y);
        movement = moveSpeed * Time.deltaTime * Vector3.ProjectOnPlane(movement, Vector3.up).normalized;
        transform.Translate(movement);
    }
}
