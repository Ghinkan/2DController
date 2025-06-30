using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    public abstract class Predicate: ScriptableObject
    {
        [SerializeField] protected PlayerContext PlayerContext;
        
        public abstract bool Evaluate();
    }
}