using System;
using System.Collections.Generic;
using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [CreateAssetMenu(fileName = "StateMachine", menuName = "StateMachine/StateMachine", order = 0)]
    public class StateMachine: ScriptableObject
    {
        public List<StateNode> NodeList = new List<StateNode>();
        
        private StateNode _currentNode;
        private readonly HashSet<Transition> _anyTransitions = new HashSet<Transition>();

        public State CurrentState => _currentNode.State;

        public void Update()
        {
            Transition transition = GetTransition();

            if (transition != null)
            {
                ChangeState(transition.To);
            }

            _currentNode.State?.Update();
        }

        public void FixedUpdate()
        {
            _currentNode.State?.FixedUpdate();
        }

        public void SetState(State state)
        {
            StateNode node = NodeList.Find(n => n.State.GetType() == state.GetType());
            _currentNode = node;
            _currentNode.State?.OnEnter();
        }

        private void ChangeState(State state)
        {
            if (state == _currentNode.State)
                return;

            State previousState = _currentNode.State;
            StateNode nextNode = NodeList.Find(n => n.State.GetType() == state.GetType());
            State nextState = nextNode.State;

            previousState?.OnExit();
            nextState.OnEnter();
            _currentNode = nextNode;
        }
        
        private Transition GetTransition()
        {
            foreach (Transition transition in _anyTransitions)
                if (transition.Evaluate())
                    return transition;

            foreach (Transition transition in _currentNode.Transitions)
                if (transition.Evaluate())
                    return transition;

            return null;
        }

        public void Initialize()
        {
            SetState(NodeList[0].State);
        }
    }
}