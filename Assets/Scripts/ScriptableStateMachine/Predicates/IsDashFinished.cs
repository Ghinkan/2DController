using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "IsDashFinished", menuName = "StateMachine/Predicates/IsDashFinished", order = 0)]
    public class IsDashFinished : Predicate
    {
        public override bool Evaluate()
        {
            return !PlayerContext.IsDashing;
        }
    }
}