using UnityEngine;

public class PlayerWalkingState : PlayerState
{
    public PlayerWalkingState(Player player, PlayerStateMachine playerStateMachine) : base(player, playerStateMachine)
    {
        
    }
    public override void EnterState()
    {
        base.EnterState();
        Player.EnableMovement();
        Player.EnableJump();
        Player.EnableInteract();
    }
    public override void ExitState()
    {
        base.ExitState();
        Player.DisableMovement();
        Player.DisableJump();
        Player.DisableInteract();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (Player.CanJump())
        {
            Player.ExecuteJump();
        }
        Player.MovePlayer(Player.GetMoveInput());
    }
    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }
    
}
