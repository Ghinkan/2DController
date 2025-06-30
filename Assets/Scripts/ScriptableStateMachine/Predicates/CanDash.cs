using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "CanDash", menuName = "StateMachine/Predicates/CanDash", order = 0)]
    public class CanDash : Predicate
    {
        public override bool Evaluate()
        {
            return PlayerContext.DashRefill.DashesLeft > 0 && PlayerContext.PlayerController.LastPressedDashTime.IsRunning;
        }
    }
}