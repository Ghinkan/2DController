using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
namespace UnityUtils.Timers
{
    public abstract class TimerBootstrapper
    {
        private static PlayerLoopSystem _timerSystem;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        internal static void Initialize()
        {
            PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            
            _timerSystem = new PlayerLoopSystem() { type = typeof(TimerManager), updateDelegate = TimerManager.UpdateTimers, subSystemList = null, };
            
            PlayerLoopInterface.SystemsCleared -= RemoveTimerManager;
            PlayerLoopInterface.SystemsCleared += RemoveTimerManager;
            
            if (!PlayerLoopInterface.InsertSystem<Update>(ref currentPlayerLoop, _timerSystem,0))
            {
                Debug.LogWarning("Improved Timers not initialized, unable to register TimerManager into the Update loop.");
                return;
            }

            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
            PlayerLoopInterface.PrintPlayerLoop(currentPlayerLoop);
        }
        
        private static void RemoveTimerManager()
        {
            TimerManager.Clear();
            PlayerLoopInterface.SystemsCleared -= RemoveTimerManager;
        }
    }
}