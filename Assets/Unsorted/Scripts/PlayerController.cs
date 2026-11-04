using UnityEngine;
using UnityEngine.InputSystem;


public struct BlobButtons
{
    public float movex;
    public bool jumppressed;
    public bool jumpheld;
    public bool grapplepressed;
    public bool grappleheld;
}


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


    const float StickDeadzone = 0.5f;

    InputAction move;
    InputAction jump;
    InputAction grapple;


    // Getters
    public bool IsDead => isDead;
    public PlayerInput Input => playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();  
    }

        void Start()
    {
        move = playerInput.actions["Move"];
        jump = playerInput.actions["Jump"];
        grapple = playerInput.actions["Grapple"];
    }

        public BlobButtons ReadButtons()
    {
        BlobButtons buttons = new BlobButtons();

        float stickx = move.ReadValue<Vector2>().x;
        if (Mathf.Abs(stickx) > StickDeadzone) buttons.movex = stickx;

        buttons.jumppressed = jump.WasPressedThisFrame();
        buttons.jumpheld = jump.IsPressed();
        buttons.grapplepressed = grapple.WasPressedThisFrame();
        buttons.grappleheld = grapple.IsPressed();

        return buttons;
    }

    public void Kill()
    {
        if (isDead) return;

        visuals.SetActive(false);
        if (movementComponent != null) movementComponent.enabled = false;

        isDead = true;
    }

    public void Respawn()
    {
        visuals.SetActive(true);
        if (movementComponent != null) movementComponent.enabled = true;
        isDead = false;
    }

    /// <summary>
    /// Removes this player from the game.
    /// </summary>
    public void Remove()
    {
        if (PlayerManager.Players.Contains(this)) PlayerManager.Players.Remove(this);

        Destroy(gameObject);
    }
}
