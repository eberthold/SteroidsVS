using System;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Shell;

namespace SteroidsVS.Logging
{
    /// <summary>
    /// <see cref="ILogger"/> for writing to Visual Studio Activity Log.
    /// </summary>
    public class ActivityLogLogger : ILogger
    {
        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <inheritdoc />
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                    ActivityLog.LogInformation()
            }
        }
    }
}
