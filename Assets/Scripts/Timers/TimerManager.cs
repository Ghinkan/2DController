using System.Collections.Generic;
namespace UnityUtils.Timers
{
    public static class TimerManager
    {
        private static readonly List<Timer> Timers = new List<Timer>();

        public static void RegisterTimer(Timer timer)
        {
            Timers.Add(timer);
        }
        
        public static void DeregisterTimer(Timer timer)
        {
            Timers.Remove(timer);
        }
        
        public static void Clear()
        {
            Timers.Clear();
        }

        public static void UpdateTimers()
        {
            foreach (Timer timer in new List<Timer>(Timers))
                timer.Tick();
        }
    }
}