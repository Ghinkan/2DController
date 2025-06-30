using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "CanJump", menuName = "StateMachine/Predicates/CanJump", order = 0)]
    public class CanJump : Predicate
    {
        public override bool Evaluate()
        {
            return PlayerContext.PlayerController.LastPressedJumpTime.IsRunning && PlayerContext.PlayerController.LastOnGroundTimer.IsRunning;
        }
    }
}