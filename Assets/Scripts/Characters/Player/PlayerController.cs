using UnityEngine;

// Handles player movement and facing logic using input from the shared InputHub.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Reference to the CharacterController used to move the player.
    private CharacterController controller;

    // Main camera transform, used to align movement with the camera's axes.
    private Transform cam;

    [Header("Collision Settings")]
    // Small downward offset for checking whether the player is near the ground.
    [SerializeField] private Vector3 groundOffset = new Vector3(0, -0.1f, 0);

    // Forward offset used for obstacle or surface detection checks.
    [SerializeField] private Vector3 forwardOffset = new Vector3(0, 0, 0.1f);

    // Radius used for collision/proximity checks.
    [SerializeField] private float checkRadius = 0.2f;

    [Header("Movement Settings")]
    // Base speed applied to the player when moving.
    [SerializeField] private float moveSpeed = 5f;

    // Additional stick-based force, kept for tuning or future movement adjustments.
    [SerializeField] private float stickForce = -5f;

    // Gravity value used for vertical motion if implemented later.
    [SerializeField] private float gravity = -20f;

    // Direction the player is trying to move in world space.
    private Vector3 moveDirection;

    // Last valid direction the player is facing.
    private Vector3 lookDirection;

    // Current move speed after input is considered.
    private float currentSpeed;

    // Vertical velocity used for gravity/falling behavior.
    private float verticalSpeed;

    // Tracks whether the player is currently grounded.
    private bool isGrounded;

    private void Start()
    {
        // Cache the CharacterController component for movement calls.
        controller = GetComponent<CharacterController>();

        // Get the active camera so movement can be relative to the camera direction.
        cam = Camera.main.transform;
    }

    private void Update()
    {
        // Combine camera-right and camera-forward movement vectors with input values.
        moveDirection = cam.right * InputHub.Instance.Move.x + cam.forward * InputHub.Instance.Move.y;

        // Remove vertical camera tilt so the player moves along a flat plane.
        moveDirection.y = 0;
        moveDirection = moveDirection.normalized;

        // If there is input, move at the configured speed; otherwise stop.
        currentSpeed = InputHub.Instance.Move.magnitude != 0f ? moveSpeed : 0f;

        // Keep the last facing direction when the player is not actively moving.
        lookDirection = moveDirection != Vector3.zero ? moveDirection : lookDirection;

        // Smoothly rotate the player toward the movement direction at a fixed angular speed.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookDirection), 750f * Time.deltaTime);

        // Check whether the player is standing on a solid surface by probing below the character.
        // This uses a small sphere cast/check at the player's current position with a downward offset.
        isGrounded = verticalSpeed <= 0f && Physics.CheckSphere(transform.position + groundOffset, checkRadius, LayerMask.GetMask("Solid"));

        // If grounded, keep the player planted on the surface and apply a small stick force.
        // Otherwise, apply gravity so the player falls when not supported.
        if (isGrounded)
        {
            verticalSpeed = stickForce;
        }
        else
        {
            verticalSpeed += gravity * Time.deltaTime;
        }

        // Apply velocity based on the movement direction and current vertical speed.
        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalSpeed;

        // Move the character using the CharacterController for physics-like movement.
        controller.Move(velocity * Time.deltaTime);
    }
}
