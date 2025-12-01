using UnityEngine;

public class PlayerState 
{
    protected Player Player;
    protected PlayerStateMachine PlayerStateMachine;

    public PlayerState(Player player , PlayerStateMachine playerStateMachine)
    {
        Player = player;
        PlayerStateMachine = playerStateMachine;
    }

    public virtual void EnterState() {}
    public virtual void ExitState() {}
    public virtual void FrameUpdate() {}
    public virtual void PhysicsUpdate() {}
    public virtual void AnimationTriggerEvent() {}
}
