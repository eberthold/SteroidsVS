using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableManager;
using Steroids.Contracts;

namespace Steroids.CodeQuality.Diagnostic
{
    /// <summary>
    /// Extensions for <see cref="ITableEntry"/> objects.
    /// </summary>
    public static class ITableEntryExtensions
    {
        private const string SuppressionState = "SuppressionState";
        private const string Suppressed = "Suppressed";

        /// <summary>
        /// Creates a <see cref="DiagnosticInfo"/> from a <see cref="ITableEntriesHandle"/>.
        /// </summary>
        /// <param name="entry">The <see cref="ITableEntryHandle"/>.</param>
        /// <returns>The created <see cref="DiagnosticInfo"/>.</returns>
        public static DiagnosticInfo ToDiagnosticInfo(this ITableEntry entry)
        {
            if (!entry.TryGetValue(StandardTableKeyNames.ErrorSeverity, out __VSERRORCATEGORY errorCategory))
            {
                errorCategory = __VSERRORCATEGORY.EC_MESSAGE;
            }

            entry.TryGetValue(StandardTableKeyNames.DocumentName, out string path);
            entry.TryGetValue(StandardTableKeyNames.Text, out string text);
            entry.TryGetValue(StandardTableKeyNames.FullText, out string fullText);
            entry.TryGetValue(StandardTableKeyNames.ErrorCode, out string errorCode);
            entry.TryGetValue(StandardTableKeyNames.HelpLink, out string helpLink);
            entry.TryGetValue(StandardTableKeyNames.Line, out int line);
            entry.TryGetValue(StandardTableKeyNames.Column, out int column);
            entry.TryGetValue(SuppressionState, out string suppressionState);

            if (string.IsNullOrWhiteSpace(fullText))
            {
                fullText = text;
            }

            return new DiagnosticInfo
            {
                Severity = MapErrorCategoryToSeverity(errorCategory),
                Path = path,
                Message = fullText,
                ErrorCode = errorCode,
                HelpUriRaw = helpLink,
                LineNumber = line,
                Column = column,
                IsActive = suppressionState != Suppressed
            };
        }

        /// <summary>
        /// Calculates the hash code of the <see cref="ITableEntry"/> as if it was already mapped to a diagnostic info instance.
        /// </summary>
        /// <param name="entry">The <see cref="ITableEntry"/>.</param>
        /// <returns>The calculated hash code.</returns>
        public static int DiagnosticInfoHashCode(this ITableEntry entry)
        {
            if (!entry.TryGetValue(StandardTableKeyNames.ErrorSeverity, out __VSERRORCATEGORY errorCategory))
            {
                errorCategory = __VSERRORCATEGORY.EC_MESSAGE;
            }

            entry.TryGetValue(StandardTableKeyNames.DocumentName, out string path);
            entry.TryGetValue(StandardTableKeyNames.Text, out string text);
            entry.TryGetValue(StandardTableKeyNames.FullText, out string fullText);
            entry.TryGetValue(StandardTableKeyNames.ErrorCode, out string errorCode);
            entry.TryGetValue(StandardTableKeyNames.HelpLink, out string helpLink);
            entry.TryGetValue(StandardTableKeyNames.Line, out int line);
            entry.TryGetValue(StandardTableKeyNames.Column, out int column);
            entry.TryGetValue(SuppressionState, out string suppressionState);

            if (string.IsNullOrWhiteSpace(fullText))
            {
                fullText = text;
            }

            return DiagnosticInfo.GetHashCode(path, line, (int)MapErrorCategoryToSeverity(errorCategory), column, errorCode, fullText);
        }

        /// <summary>
        /// Maps the <see cref="__VSERRORCATEGORY"/> to <see cref="DiagnosticSeverity"/>.
        /// </summary>
        /// <param name="errorCategory">The <see cref="__VSERRORCATEGORY"/>.</param>
        /// <returns>The matching <see cref="DiagnosticSeverity"/>. </returns>
        private static DiagnosticSeverity MapErrorCategoryToSeverity(__VSERRORCATEGORY errorCategory)
        {
            switch (errorCategory)
            {
                case __VSERRORCATEGORY.EC_ERROR:
                    return DiagnosticSeverity.Error;

                case __VSERRORCATEGORY.EC_WARNING:
                    return DiagnosticSeverity.Warning;

                case __VSERRORCATEGORY.EC_MESSAGE:
                    return DiagnosticSeverity.Info;
            }

            return DiagnosticSeverity.Hidden;
        }
    }
}
