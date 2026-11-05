using UnityEngine;

// Main player controller that owns movement, state switching, and input-driven behavior.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Reference to the CharacterController used to move this player.
    public CharacterController Controller { get; private set; }

    // The player's current behavior state.
    public PlayerState CurrentState { get; private set; }

    // The default walking state for the player.
    public PlayerNormalState NormalState { get; private set; } = new PlayerNormalState(this);

    [Header("Collision Settings")]
    // Small downward offset used when checking if the player is grounded.
    [field: SerializeField] public Vector3 GroundOffset { get; private set; } = new Vector3(0, -0.1f, 0);

    // Extra forward offset used for collision or surface detection checks.
    [field: SerializeField] public Vector3 ForwardOffset { get; private set; } = new Vector3(0, 0, 0.1f);

    // Radius for ground or obstacle checks.
    [field: SerializeField] public float CheckRadius { get; private set; } = 0.2f;

    // Current direction the player wants to travel in world space.
    public Vector3 MoveDirection { get; set; }

    // Direction the player is currently facing.
    public Vector3 LookDirection { get; set; }

    // Current movement speed after input is applied.
    public float CurrentSpeed { get; set; }

    // Vertical velocity used for gravity or ground-related movement.
    public float VerticalSpeed { get; set; }

    // Main camera transform used to make movement relative to camera direction.
    private Transform cam;

    private void Start()
    {
        // Cache the CharacterController for later movement calls.
        Controller = GetComponent<CharacterController>();

        // Store the camera so input can be converted into world-relative directions.
        cam = Camera.main.transform;

        // Begin the player in the normal state.
        SetState(NormalState);
    }

    private void Update()
    {
        // Run the current state's update logic every frame.
        CurrentState?.UpdateState();

        // Let the state decide if it should transition to another state.
        CurrentState?.ChangeState();

        // Apply the movement calculated by the state.
        ApplyMovement();
    }

    public void SetState(PlayerState newState)
    {
        // Exit the previous state so it can clean up.
        CurrentState?.ExitState();

        // Switch to the new state and immediately activate it.
        CurrentState = newState;
        CurrentState?.EnterState();
    }

    public Vector3 GetInputDirection()
    {
        // Convert the input vector into a direction relative to the camera.
        Vector3 direction = cam.right * InputHub.Instance.Move.x + cam.forward * InputHub.Instance.Move.y;

        // Remove vertical tilt so movement stays on the ground plane.
        direction.y = 0;
        return direction.normalized;
    }

    public void FaceDirection(float turnSpeed = 750f)
    {
        // Do nothing if there is no valid facing direction.
        if (LookDirection == Vector3.zero) return;

        // Smoothly rotate the player toward their facing direction.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    public void ApplyMovement()
    {
        // Combine the movement direction and current speed into a velocity vector.
        Vector3 velocity = MoveDirection * CurrentSpeed;
        velocity.y = VerticalSpeed;

        // Move the CharacterController using frame-rate independent timing.
        Controller.Move(velocity * Time.deltaTime);
    }
}
