using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Idea behind this is to be a sort of 'manager' for each player, handling general things like individual deaths.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] MonoBehaviour movementComponent;
    [SerializeField] GameObject visuals;

    bool isDead;
    PlayerInput playerInput;
    
    // Getters
    public bool IsDead => isDead;
    public PlayerInput Input => playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void Kill()
    {
        if (isDead) return;

        visuals.SetActive(false);
        if (movementComponent != null) movementComponent.enabled = false;

        isDead = true;
    }
}
