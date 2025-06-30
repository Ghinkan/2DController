using System.Collections.Generic;
using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "StateNode", menuName = "StateMachine/StateNode", order = 0)]
    public class StateNode : ScriptableObject
    {
        public State State;
        public List<Transition> Transitions;
    }
}