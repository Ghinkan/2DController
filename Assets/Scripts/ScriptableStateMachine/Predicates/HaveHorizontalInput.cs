using Controller2DProject.Controllers.Inputs;
using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine.Predicates
{
    [CreateAssetMenu(fileName = "HaveHorizontalInput", menuName = "StateMachine/Predicates/HaveHorizontalInput", order = 0)]
    public class HaveHorizontalInput : Predicate
    {
        [SerializeField] private InputReader _input;
        private const float MovementThreshold = 0.01f;
        
        public override bool Evaluate()
        {
            return Mathf.Abs(_input.Direction.x) > MovementThreshold;
        }
    }

}