using System;
using System.Windows;
using System.Windows.Threading;

namespace Steroids.Common.Helpers
{
    public class Debouncer
    {
        private readonly TimeSpan _delay;
        private readonly Action _action;
        private readonly DispatcherTimer _timer;

        public Debouncer(Action action, TimeSpan delay)
        {
            _delay = delay;
            _action = action;
            _timer = new DispatcherTimer
            {
                Interval = delay
            };

            WeakEventManager<DispatcherTimer, EventArgs>.AddHandler(_timer, nameof(DispatcherTimer.Tick), Execute);
        }

        public void Start()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void Execute(object sender, EventArgs e)
        {
            _timer.Stop();
            _action.Invoke();
        }
    }
}
