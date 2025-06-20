using System.Collections.Generic;
using UnityEngine;
namespace UnityUtils.ServiceLocator
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<string, IGameService> Services = new Dictionary<string, IGameService>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            UnregisterAll();
        }
        
        public static T Get<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!Services.TryGetValue(key, value: out IGameService service))
            {
                return default(T);
            }

            return (T)service;
        }

        public static void Register<T>(T service) where T : IGameService
        {
            string key = typeof(T).Name;
            if (!Services.TryAdd(key, service))
                Debug.LogError($"Attempted to register service of type {key} which is already registered with {nameof(ServiceLocator)}.");
        }

        public static void Unregister<T>() where T : IGameService
        {
            string key = typeof(T).Name;
            if (!Services.Remove(key))
                Debug.LogError($"Attempted to unregister service of type {key} which is not registered with {nameof(ServiceLocator)}.");
        }

        public static void UnregisterAll()
        {
            Services.Clear();
        }
    }
}