using UnityEngine;

// Default movement state for the player.
// This is the standard walking/idle state used whenever the player is not in a special action or combat state.
[System.Serializable]
public class PlayerNormalState : PlayerState
{
    // Hold a reference to the player controller so this state can read input, update movement, and trigger animation.
    public PlayerNormalState(PlayerController controller) : base(controller) { }

    // Base speed for normal movement while this state is active.
    [SerializeField] private float moveSpeed = 5f;

    // Vertical value used when the player first enters this state to keep them grounded and stable.
    [SerializeField] private float stickForce = -5f;

    public override void EnterState()
    {
        // Reset vertical velocity when entering the normal state to keep the player grounded.
        player.VerticalSpeed = stickForce;

        player.AnimHub.PlayAnimation("Dash", 0.2f);
    }

    public override void UpdateState()
    {
        // Convert the camera-relative input into a flat movement direction for the player.
        player.MoveDirection = player.GetInputDirection();

        // Scale the input magnitude by the normal move speed to determine how fast the player should move.
        player.CurrentSpeed = InputHub.Instance.Move.magnitude * moveSpeed;

        // Play the correct animation based on whether the player is idle, walking, or running.
        if (player.CurrentSpeed == 0f)
        {
            player.AnimHub.PlayAnimation("Idle", 0.2f);
        }
        else
        {
            player.AnimHub.PlayAnimation(player.CurrentSpeed <= 3f ? "Walk" : "Run", 0.2f);
        }

        // Keep the last valid facing direction when the player is not actively moving.
        player.LookDirection = player.MoveDirection != Vector3.zero ? player.MoveDirection : player.LookDirection;

        // Smoothly rotate the player toward the direction they are facing.
        player.FaceDirection();
    }

    public override void ChangeState()
    {
        // Check whether the player should switch to a different state such as attack, jump, or stunned.
    }

    public override void ExitState()
    {
        // Any cleanup for leaving this state should happen here.
    }
}