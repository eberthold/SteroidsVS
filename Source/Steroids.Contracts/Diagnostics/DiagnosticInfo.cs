using System;
using Microsoft.CodeAnalysis;

namespace Steroids.Contracts
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
        public int Line { get; set; }

        /// <summary>
        /// The column on which this diagnostic starts.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the DiagnosticInfo is not suppressed by comment or suppress attribute.
        /// </summary>
        public bool IsActive { get; set; }

        public static bool operator ==(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) == 0;

        public static bool operator !=(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) != 0;

        public static bool operator <(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) < 0;

        public static bool operator <=(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) <= 0;

        public static bool operator >(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) > 0;

        public static bool operator >=(DiagnosticInfo first, DiagnosticInfo second)
            => first.CompareTo(second) >= 0;

        /// <inheritdoc />
        /// <remarks>
        /// Comparison is done in three separate stages.
        /// 1. Line (smaller comes first)
        /// 2. Severity (higher comes first, to ensure errors are most important)
        /// 3. Column (smaller comes first).
        /// </remarks>
        public int CompareTo(DiagnosticInfo other)
        {
            var line = Line - other.Line;
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

            return CompareTo(other) == 0;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = 17;
            hash += 23 + Path.GetHashCode();
            hash += 23 + Line.GetHashCode();
            hash += 23 + Severity.GetHashCode();
            hash += 23 + Column.GetHashCode();
            hash += 23 + ErrorCode.GetHashCode();

            return hash;
        }
    }
}
