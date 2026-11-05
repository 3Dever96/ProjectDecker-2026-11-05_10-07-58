// Default movement state for the player.
// This state is used when the player is in a normal walking/idle mode and not in a special action state.
public class PlayerNormalState : PlayerState
{
    // Store a reference to the player controller so this state can read and update player data.
    public PlayerNormalState(PlayerController controller) : base(controller) { }

    // Movement speed for the player while in this normal state.
    [SerializeField] private float moveSpeed = 5f;

    // Vertical force used when the player is grounded and entering this state.
    [SerializeField] private float stickForce = -5f;

    public override void EnterState()
    {
        // Reset the player's vertical velocity when entering this state.
        player.VerticalSpeed = stickForce;
    }

    public override void UpdateState()
    {
        // Read the player's current input direction from the camera-relative movement system.
        player.MoveDirection = player.GetInputDirection();

        // If there is input, use normal walk speed; otherwise stop the player.
        player.CurrentSpeed = InputHub.Instance.Move.magnitude != 0f ? moveSpeed : 0f;

        // Keep the last valid facing direction if the player is not currently moving.
        player.LookDirection = player.MoveDirection != Vector3.zero ? player.MoveDirection : player.LookDirection;

        // Rotate the player smoothly toward the direction they are facing.
        player.FaceDirection();
    }

    public override void ChangeState()
    {
        // Check for transitions to other states like attacking, jumping, or stunned.
    }

    public override void ExitState()
    {
        // Perform any cleanup needed before leaving this state.
    }
}