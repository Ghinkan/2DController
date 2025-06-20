using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace UnityUtils.StateMachine
{
    public class StateMachine
    {
        private StateNode _currentNode;
        private readonly Dictionary<Type, StateNode> _nodes = new Dictionary<Type, StateNode>();
        private readonly HashSet<Transition> _anyTransitions = new HashSet<Transition>();

        public IState CurrentState => _currentNode.State;

        public void Update()
        {
            Transition transition = GetTransition();

            if (transition != null)
            {
                ChangeState(transition.To);
                foreach (StateNode node in _nodes.Values)
                    ResetActionPredicateFlags(node.Transitions);
                ResetActionPredicateFlags(_anyTransitions);
            }

            _currentNode.State?.Update();
        }

        private static void ResetActionPredicateFlags(IEnumerable<Transition> transitions)
        {
            foreach (Transition<ActionPredicate> transition in transitions.OfType<Transition<ActionPredicate>>())
                transition.Condition.Flag = false;
        }

        public void FixedUpdate()
        {
            _currentNode.State?.FixedUpdate();
        }

        public void SetState(IState state)
        {
            _currentNode = _nodes[state.GetType()];
            _currentNode.State?.OnEnter();
        }

        private void ChangeState(IState state)
        {
            if (state == _currentNode.State)
                return;

            IState previousState = _currentNode.State;
            IState nextState = _nodes[state.GetType()].State;

            previousState?.OnExit();
            nextState.OnEnter();
            _currentNode = _nodes[state.GetType()];
        }

        public void AddTransition<T>(IState from, IState to, T condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }

        public void AddAnyTransition<T>(IState to, T condition)
        {
            _anyTransitions.Add(new Transition<T>(GetOrAddNode(to).State, condition));
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

        private StateNode GetOrAddNode(IState state)
        {
            StateNode node = _nodes.GetValueOrDefault(state.GetType());
            if (node == null)
            {
                node = new StateNode(state);
                _nodes[state.GetType()] = node;
            }

            return node;
        }

        private class StateNode
        {
            public IState State { get; }
            public HashSet<Transition> Transitions { get; }

            public StateNode(IState state)
            {
                State = state;
                Transitions = new HashSet<Transition>();
            }

            public void AddTransition<T>(IState to, T predicate)
            {
                Transitions.Add(new Transition<T>(to, predicate));
            }
        }
    }
}