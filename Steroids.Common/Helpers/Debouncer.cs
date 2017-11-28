namespace Steroids.Common.Helpers
{
    using System;
    using System.Windows.Threading;

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

            _timer.Tick += Execute;
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
