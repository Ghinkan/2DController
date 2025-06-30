using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "IsGrounded", menuName = "StateMachine/Predicates/IsGrounded", order = 0)]
    public class IsGrounded : Predicate
    {
        public override bool Evaluate()
        {
            return PlayerContext.PlayerController.GroundSensor.HasDetectedHit();
        }
    }
}