// Base class for all player states in the finite-state machine.
// Each concrete state handles a specific player action or mode, such as idle, walking, or attacking.
public abstract class PlayerState
{
    // Reference to the player controller that owns this state.
    protected PlayerController player;

    // Initialize the state with the player instance it belongs to.
    public PlayerState(PlayerController controller)
    {
        player = controller;
    }

    // Called when this state becomes active.
    public abstract void EnterState();

    // Called every frame while this state is active.
    public abstract void UpdateState();

    // Called to request a transition to a different state.
    public abstract void ChangeState();

    // Called when leaving this state so cleanup can happen.
    public abstract void ExitState();
}