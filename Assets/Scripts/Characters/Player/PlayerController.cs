using UnityEngine;

// Main player controller responsible for movement, facing, animation, and state transitions.
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Reference to the CharacterController used to move the player physically through the world.
    public CharacterController Controller { get; private set; }

    // Reference to the animation system so movement states can trigger the correct animation clips.
    public AnimationHub AnimHub { get; private set; }

    // The player's active behavior state.
    public PlayerState CurrentState { get; private set; }

    // Default movement state used when the player is not in a special action state.
    // Created in Start so the controller can enter a valid state immediately.
    [field: SerializeField] public PlayerNormalState NormalState { get; private set; }

    [Header("Collision Settings")]
    // Small downward offset used to check whether the player is grounded.
    [field: SerializeField] public Vector3 GroundOffset { get; private set; } = new Vector3(0, -0.1f, 0);

    // Extra forward offset used for collision or obstacle checks.
    [field: SerializeField] public Vector3 ForwardOffset { get; private set; } = new Vector3(0, 0, 0.1f);

    // Radius used for ground and obstacle detection checks.
    [field: SerializeField] public float CheckRadius { get; private set; } = 0.2f;

    // Desired movement direction in world space based on the input and camera orientation.
    public Vector3 MoveDirection { get; set; }

    // The direction the player is currently facing.
    public Vector3 LookDirection { get; set; }

    // Current movement speed calculated from the active state and input magnitude.
    public float CurrentSpeed { get; set; }

    // Vertical velocity used for gravity or grounded force logic.
    public float VerticalSpeed { get; set; }

    // Camera transform used to convert input into world-relative movement.
    private Transform cam;

    private void Start()
    {
        // Cache the CharacterController used to move the player.
        Controller = GetComponent<CharacterController>();

        // Grab the animation controller attached to the player hierarchy.
        AnimHub = GetComponentInChildren<AnimationHub>();

        // Store the camera so movement can be relative to camera direction.
        cam = Camera.main.transform;

        // Build the default state and assign it to the controller.
        NormalState = new PlayerNormalState(this);

        // Start the player in the normal movement state.
        SetState(NormalState);
    }

    private void Update()
    {
        // Run the current state's per-frame logic.
        CurrentState?.UpdateState();

        // Give the state a chance to decide whether it should transition to another state.
        CurrentState?.ChangeState();

        // Apply the final movement vector calculated by the state.
        ApplyMovement();
    }

    public void SetState(PlayerState newState)
    {
        // Exit the previous state so it can clean up before switching.
        CurrentState?.ExitState();

        // Assign the new state and immediately activate it.
        CurrentState = newState;
        CurrentState?.EnterState();
    }

    public Vector3 GetInputDirection()
    {
        // Convert the input vector into a direction relative to the camera's orientation.
        Vector3 direction = cam.right * InputHub.Instance.Move.x + cam.forward * InputHub.Instance.Move.y;

        // Ignore vertical camera tilt so movement stays aligned to the ground plane.
        direction.y = 0;
        return direction.normalized;
    }

    public void FaceDirection(float turnSpeed = 750f)
    {
        // Ignore the rotation request if there is no valid facing direction.
        if (LookDirection == Vector3.zero) return;

        // Smoothly rotate the player toward the face direction using a turn speed.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(LookDirection), turnSpeed * Time.deltaTime);
    }

    public void ApplyMovement()
    {
        // Combine the desired movement direction and speed into a velocity vector.
        Vector3 velocity = MoveDirection * CurrentSpeed;
        velocity.y = VerticalSpeed;

        // Move the CharacterController using frame-rate-independent delta timing.
        Controller.Move(velocity * Time.deltaTime);
    }
}
