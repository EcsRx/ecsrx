using System;
using System.Timers;
using EcsRx.MicroRx.Subjects;

namespace EcsRx.Infrastructure.Scheduling
{
    public class DefaultObservableScheduler : IObservableScheduler
    {
        private readonly Timer _timer;
        private DateTime _previousDateTime;
        private readonly Subject<TimeSpan> _onUpdate = new Subject<TimeSpan>();

        public IObservable<TimeSpan> OnUpdate => _onUpdate;
        
        public DefaultObservableScheduler(int desiredFps = 60)
        {
            _timer = new Timer { Interval = 1000f / desiredFps };
            _timer.Elapsed += UpdateTick;

            _previousDateTime = DateTime.Now;
            _timer.Start();
        }

        private void UpdateTick(object sender, ElapsedEventArgs e)
        {
            var elapsed = e.SignalTime - _previousDateTime;
            _onUpdate.OnNext(elapsed);
            _previousDateTime = e.SignalTime;
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}