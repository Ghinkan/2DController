using UnityEngine;
using UnityEngine.Events;
namespace UnityUtils.Timers
{
    /// <summary>
    /// Timer that ticks at a specific frequency. (N times per second)
    /// </summary>
    public class FrequencyTimer : Timer
    {
        public int TicksPerSecond { get; private set; }

        public UnityAction OnTick = delegate { };

        private float _timeThreshold;

        public FrequencyTimer(int ticksPerSecond, MonoBehaviour owner) : base(0, owner)
        {
            CalculateTimeThreshold(ticksPerSecond);
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime >= _timeThreshold)
            {
                CurrentTime -= _timeThreshold;
                OnTick.Invoke();
            }

            if (IsRunning && CurrentTime < _timeThreshold)
                CurrentTime += Time.deltaTime;
        }

        public override bool IsFinished
        {
            get
            {
                return !IsRunning;
            }
        }

        public override void Reset()
        {
            CurrentTime = 0;
        }

        public void Reset(int newTicksPerSecond)
        {
            CalculateTimeThreshold(newTicksPerSecond);
            Reset();
        }

        private void CalculateTimeThreshold(int ticksPerSecond)
        {
            TicksPerSecond = ticksPerSecond;
            _timeThreshold = 1f / TicksPerSecond;
        }
    }
}