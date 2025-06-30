using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    [Serializable]
    public class Transition
    {
        public State To;
        [SerializeField] private SerializedDictionary<Predicate, bool> _rules = new SerializedDictionary<Predicate, bool>();

        public bool Evaluate()
        {
            foreach (KeyValuePair<Predicate, bool> rule in _rules)
            {
                if (rule.Key.Evaluate() != rule.Value)
                    return false;
            }
            return true;
        }
    }
}