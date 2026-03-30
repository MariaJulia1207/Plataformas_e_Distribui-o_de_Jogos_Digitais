using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // ...existing code...

    // Internal tuning values (not exposed in Inspector)
    private float moveSpeed = 10f; // units/s used as acceleration scale
    private float sprintMultiplier = 1.6f;
    private float jumpForce = 5f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    // Ground check
    private float groundCheckDistance = 0.6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        if (rb == null)
            Debug.LogError("PlayerController requires a Rigidbody.");
        if (playerInput == null)
            Debug.LogError("PlayerController requires a PlayerInput component.");

        // cache actions from the PlayerInput's action asset
        if (playerInput != null && playerInput.actions != null)
        {
            moveAction = playerInput.actions["Move"];
            sprintAction = playerInput.actions["Sprint"];
            jumpAction = playerInput.actions["Jump"];
        }

        // try to set a better default ground check distance from collider if available
        var col = GetComponent<Collider>();
        if (col != null)
            groundCheckDistance = col.bounds.extents.y + 0.05f;
    }

    void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
            playerInput.actions.Enable();

        if (jumpAction != null)
            jumpAction.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        if (playerInput != null && playerInput.actions != null)
            playerInput.actions.Disable();

        if (jumpAction != null)
            jumpAction.performed -= OnJumpPerformed;
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector2 input2D = Vector2.zero;
        if (moveAction != null)
            input2D = moveAction.ReadValue<Vector2>();

        // convert to X/Z plane (Y = 0)
        Vector3 move = new Vector3(input2D.x, 0f, input2D.y);

        // sprint check
        bool sprinting = false;
        if (sprintAction != null)
            sprinting = sprintAction.ReadValue<float>() > 0.5f;

        float multiplier = sprinting ? sprintMultiplier : 1f;

        // apply acceleration force only on X/Z, do not override vertical velocity
        if (move.sqrMagnitude > 0f)
        {
            Vector3 force = move.normalized * moveSpeed * multiplier;
            rb.AddForce(force, ForceMode.Acceleration);
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        if (rb == null)
            return;

        if (IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        // Raycast down from slightly above the transform position to detect ground
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        float distance = groundCheckDistance + 0.02f;
        return Physics.Raycast(origin, Vector3.down, distance);
    }
}

