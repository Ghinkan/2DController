using UnityEngine;
namespace UnityUtils.Timers
{
    /// <summary>
    /// Timer that counts down from a specific value to zero.
    /// </summary>
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value, MonoBehaviour owner) : base(value, owner) { }

        public override bool IsFinished
        {
            get
            {
                return CurrentTime <= 0;
            }
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
                CurrentTime -= Time.deltaTime;

            if (IsRunning && CurrentTime <= 0)
                Stop();
        }
    }
}