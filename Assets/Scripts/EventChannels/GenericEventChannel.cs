using UnityEngine;
using UnityEngine.Events;
namespace UnityUtils.EventChannels
{
    public abstract class GenericEventChannel<T> : ScriptableObject
    {
        public UnityAction<T> GameEvent;
        public void RaiseEvent(T parameter)
        {
            GameEvent?.Invoke(parameter);
        }
    }
}