using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "CanSlide", menuName = "StateMachine/Predicates/CanSlide", order = 0)]
    public class CanSlide : Predicate
    {
        public override bool Evaluate()
        {
            return PlayerContext.PlayerController.LastOnWallTimer.IsRunning &&
                ((PlayerContext.PlayerController.LastOnWallLeftTime.IsRunning && PlayerContext.PlayerController.Input.Direction.x < 0) || 
                    (PlayerContext.PlayerController.LastOnWallRightTime.IsRunning && PlayerContext.PlayerController.Input.Direction.x > 0));
        }
    }

}