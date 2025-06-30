using UnityEngine;
namespace Controller2DProject.ScriptableStateMachine
{
    public abstract class State: ScriptableObject
    {
        public virtual void Update()      { }
        public virtual void FixedUpdate() { }
        public virtual void OnEnter()     { }
        public virtual void OnExit()      { }
    }
}