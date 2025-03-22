using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace PlayerInput.Model.Managers.PlayerManager
{
    public class Clock
    {
        private readonly Timer _timer;
        private DateTime _startTime;
        private TimeSpan _elapsed;

        public TimeSpan TimePassed
        { 
            get => _elapsed;
            set 
            {
                if (_elapsed != value)
                {
                    _elapsed = value;
                    TimePassedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler? TimePassedChanged;

        public Clock()
        {
            TimePassed = TimeSpan.Zero;

            _timer = new Timer(1000); // Tick every second
            _timer.Elapsed += OnTick;
        }

        public void Start()
        {
            if (!_timer.Enabled)
            {
                _startTime = DateTime.Now;
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                UpdateElapsed();
            }
        }

        public void Reset()
        {
            Stop();
            TimePassed = TimeSpan.Zero;
        }

        public void ChangeState()
        {
            if (_timer.Enabled)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }

        private void OnTick(object? sender, ElapsedEventArgs e)
        {
            UpdateElapsed();
        }

        private void UpdateElapsed()
        {
            if (_timer.Enabled)
            {
                TimePassed += DateTime.Now - _startTime;
                _startTime = DateTime.Now; // Reset the start time for the next tick
            }
        }
    }
}
