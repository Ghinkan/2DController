using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "CanWallJump", menuName = "StateMachine/Predicates/CanWallJump", order = 0)]
    public class CanWallJump : Predicate
    {

        public override bool Evaluate()
        {
            return PlayerContext.PlayerController.LastPressedJumpTime.IsRunning && PlayerContext.PlayerController.LastOnWallTimer.IsRunning;
        }
    }
}