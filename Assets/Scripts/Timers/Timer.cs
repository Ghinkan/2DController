using System;
using UnityEngine;
using UnityEngine.Events;
namespace UnityUtils.Timers
{
    public abstract class Timer : IDisposable
    {
        public UnityAction OnTimerStart = delegate() { };
        public UnityAction OnTimerStop = delegate() { };

        public float CurrentTime { get; protected set; }
        public bool IsRunning { get; private set; }
        public float Progress
        {
            get
            {
                return Mathf.Clamp(CurrentTime / InitialTime, 0, 1);
            }
        }

        protected float InitialTime;

        protected Timer(float value)
        {
            InitialTime = value;
        }

        protected Timer(float value, MonoBehaviour owner)
        {
            InitialTime = value;
            owner.destroyCancellationToken.Register(Dispose);
        }

        public void Start()
        {
            CurrentTime = InitialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                CurrentTime = 0f;
                TimerManager.DeregisterTimer(this);
                OnTimerStop.Invoke();
            }
        }

        public void Resume()
        {
            IsRunning = true;
        }
        
        public void Pause()
        {
            IsRunning = false;
        }
        
        public virtual void Reset()
        {
            CurrentTime = InitialTime;
        }
        
        public virtual void Reset(float newTime)
        {
            InitialTime = newTime;
            Reset();
        }

        public virtual void Restart()
        {
            Reset();
            Start();
        }

        public virtual void Restart(float newTime)
        {
            Reset(newTime);
            Start();
        }

        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Dispose()
        {
            TimerManager.DeregisterTimer(this);
            GC.SuppressFinalize(this);
        }
    }
}