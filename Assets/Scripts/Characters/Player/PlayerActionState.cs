using UnityEngine;

public class PlayerActionState : PlayerState
{
    public PlayerActionState(PlayerController controller) : base(controller) { }

    public override void EnterState()
    {
        // Logic for entering the action state, such as playing an animation or setting flags.
    }

    public override void UpdateState()
    {
        // Logic for updating the action state, such as checking for input or timing out the action.
    }

    public override void ChangeState()
    {
        // Logic for transitioning to another state based on conditions or input.
    }

    public override void ExitState()
    {
        // Logic for cleaning up when exiting the action state, such as resetting flags or stopping animations.
    }
}
