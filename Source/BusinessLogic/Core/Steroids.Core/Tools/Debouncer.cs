using System;

namespace Steroids.Core.Tools
{
    public class Debouncer
    {
        private readonly TimeSpan _delay;

        private DateTime _lastExecution;

        public Debouncer(TimeSpan delay)
        {
            _delay = delay;
        }

        public void Debounce(Action action)
        {
            if (_lastExecution >= DateTime.Now - _delay)
            {
                return;
            }

            _lastExecution = DateTime.Now;
            action.Invoke();
        }
    }
}
