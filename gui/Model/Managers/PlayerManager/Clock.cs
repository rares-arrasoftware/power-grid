using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace gui.Model.Managers.PlayerManager
{
    public class Clock
    {
        private readonly Timer _timer;
        private Timer? _delayTimer; // nullable so we know if delay is active
        private DateTime _startTime;
        private TimeSpan _elapsed;

        private bool _isDelaying;

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

        public bool IsDelaying => _isDelaying;

        public event EventHandler? TimePassedChanged;

        public Clock()
        {
            TimePassed = TimeSpan.Zero;

            _timer = new Timer(1000); // Tick every second
            _timer.Elapsed += OnTick;
        }

        public void Start()
        {
            if (!_timer.Enabled && !_isDelaying)
            {
                _startTime = DateTime.Now;
                _timer.Start();
            }
        }

        public void Stop()
        {
            if (_isDelaying)
            {
                _delayTimer?.Stop();
                _delayTimer?.Dispose();
                _delayTimer = null;
                _isDelaying = false;
            }

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
            if (_timer.Enabled || _isDelaying)
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

        public void StartWithDelay(int delaySeconds = 20)
        {
            if (_timer.Enabled || _isDelaying)
                return;

            _isDelaying = true;

            _delayTimer = new Timer(delaySeconds * 1000) { AutoReset = false };
            _delayTimer.Elapsed += (_, _) =>
            {
                _isDelaying = false;
                _delayTimer?.Dispose();
                _delayTimer = null;

                Start();
            };
            _delayTimer.Start();
        }

    }
}
