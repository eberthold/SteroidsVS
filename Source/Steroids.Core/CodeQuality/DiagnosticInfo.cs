using System;

namespace Steroids.Core.CodeQuality
{
    /// <summary>
    /// Keeps all informations necessary to show in-line diagnostics to the user.
    /// </summary>
    public class DiagnosticInfo : IComparable<DiagnosticInfo>
    {
        /// <summary>
        /// The severity of the diagnostic info.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// The diagnostic info message which is displayed to the user.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The path of the file in which this diagnostic info is found.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The number of the diagnostic info e.g. SA1600.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// The Uri where the user can find some additional informations about this diagnostic.
        /// </summary>
        public string HelpUriRaw { get; set; }

        /// <summary>
        /// The line on which this diagnostic is found.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// The computed line number which also respects collapsed code.
        /// </summary>
        public int ComputedLineNumber { get; set; }

        /// <summary>
        /// The column on which this diagnostic starts.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DiagnosticInfo is not suppressed by comment or suppress attribute.
        /// </summary>
        public bool IsActive { get; set; }

        public static bool operator ==(DiagnosticInfo first, DiagnosticInfo second)
            => first.Equals(second);

        public static bool operator !=(DiagnosticInfo first, DiagnosticInfo second)
            => !first.Equals(second);

        public static bool operator <(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) < 0;

        public static bool operator <=(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) <= 0;

        public static bool operator >(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) > 0;

        public static bool operator >=(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) >= 0;

        /// <summary>
        /// Calculates the hash code based on the given parameters.
        /// Used to compare elements without creating an instance of this class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="severity">The diagnostic severity.</param>
        /// <param name="column">The column.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="isActive">Flag, if the diagnostic is active.</param>
        /// <returns>The calculated hash code.</returns>
        public static int GetHashCode(string path, int lineNumber, int severity, int column, string errorCode, string message, bool isActive)
        {
            var hash = 17;
            hash *= 23 + (path ?? string.Empty).GetHashCode();
            hash *= 23 + lineNumber.GetHashCode();
            hash *= 23 + severity.GetHashCode();
            hash *= 23 + column.GetHashCode();
            hash *= 23 + (errorCode ?? string.Empty).GetHashCode();
            hash *= 23 + (message ?? string.Empty).GetHashCode();
            hash *= 23 + isActive.GetHashCode();

            return hash;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Comparison is done in three separate stages.
        /// 1. Line (smaller comes first)
        /// 2. Severity (higher comes first, to ensure errors are most important)
        /// 3. Column (smaller comes first).
        /// </remarks>
        public int CompareTo(DiagnosticInfo other)
        {
            if (other is null)
            {
                return -1;
            }

            var line = LineNumber - other.LineNumber;
            if (line != 0)
            {
                return line;
            }

            if (Severity > other.Severity)
            {
                return -1;
            }
            else if (Severity < other.Severity)
            {
                return 1;
            }

            return Column - other.Column;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is DiagnosticInfo other))
            {
                return false;
            }

            return GetHashCode() == other.GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GetHashCode(Path, LineNumber, (int)Severity, Column, ErrorCode, Message, IsActive);
        }
    }
}
