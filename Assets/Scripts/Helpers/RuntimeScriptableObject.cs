using System.Collections.Generic;
namespace UnityEngine
{
    public abstract class RuntimeScriptableObject : ScriptableObject
    {
        private static readonly List<RuntimeScriptableObject> Instances = new List<RuntimeScriptableObject>();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetAllInstances()
        {
            foreach (RuntimeScriptableObject instance in Instances)
                instance.OnReset();
        }
        
        protected abstract void OnReset();

        protected virtual void OnEnable()
        {
            Instances.Add(this);
        }
        
        protected virtual void OnDisable()
        {
            Instances.Remove(this);
        }
    }
}