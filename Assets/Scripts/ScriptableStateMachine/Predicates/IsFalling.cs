using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "IsFalling", menuName = "StateMachine/Predicates/IsFalling", order = 0)]
    public class IsFalling : Predicate
    {

        public override bool Evaluate()
        {
            return PlayerContext.PlayerController.Rb.linearVelocityY < 0f;
        }
    }
}