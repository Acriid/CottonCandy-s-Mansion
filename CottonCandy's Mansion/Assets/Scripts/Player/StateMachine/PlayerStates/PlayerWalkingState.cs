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
    }
    public override void ExitState()
    {
        base.ExitState();
        Player.DisableMovement();
    }
    public override void FrameUpdate()
    {
        base.FrameUpdate();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        Player.MovePlayer(Player.GetMoveInput());
    }
    public override void AnimationTriggerEvent()
    {
        base.AnimationTriggerEvent();
    }
    
}
