using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Steroids.Contracts;

namespace Steroids.CodeQuality.Diagnostic
{
    /// <summary>
    /// Provides <see cref="DiagnosticInfo"/> by crawling the ErrorList UI directly.
    /// </summary>
    public class ErrorListDiagnosticProvider : IDiagnosticProvider
    {
        private const string SuppressionState = "suppressionstate";
        private const string NotSuppressed = "Active";

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorListDiagnosticProvider"/> class.
        /// </summary>
        /// <param name="errorList">The <see cref="IErrorList"/>.</param>
        public ErrorListDiagnosticProvider(IErrorList errorList)
        {
            if (errorList == null)
            {
                throw new ArgumentNullException(nameof(errorList));
            }

            WeakEventManager<IWpfTableControl, EntriesChangedEventArgs>.AddHandler(errorList.TableControl, nameof(IWpfTableControl.EntriesChanged), OnErrorListEntriesChanged);
        }

        /// <inheritdoc />
        public event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;

        /// <inheritdoc />
        public IReadOnlyCollection<DiagnosticInfo> CurrentDiagnostics { get; private set; } = new List<DiagnosticInfo>();

        /// <summary>
        /// Creates a <see cref="DiagnosticInfo"/> from a <see cref="ITableEntriesHandle"/>.
        /// </summary>
        /// <param name="entry">The <see cref="ITableEntryHandle"/>.</param>
        /// <returns>The created <see cref="DiagnosticInfo"/>.</returns>
        private static DiagnosticInfo CreateDiagnosticInfoFromTableEntry(ITableEntryHandle entry)
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

            var severity = DiagnosticSeverity.Hidden;
            switch (errorCategory)
            {
                case __VSERRORCATEGORY.EC_ERROR:
                    severity = DiagnosticSeverity.Error;
                    break;
                case __VSERRORCATEGORY.EC_WARNING:
                    severity = DiagnosticSeverity.Warning;
                    break;
                case __VSERRORCATEGORY.EC_MESSAGE:
                    severity = DiagnosticSeverity.Info;
                    break;
            }

            return new DiagnosticInfo
            {
                Severity = severity,
                Path = path,
                Message = fullText,
                ErrorCode = errorCode,
                HelpUriRaw = helpLink,
                LineNumber = line,
                Column = column,
                IsActive = suppressionState == NotSuppressed
            };
        }

        private void OnErrorListEntriesChanged(object sender, EntriesChangedEventArgs args)
        {
            var diagnostics = new List<DiagnosticInfo>();

            foreach (var entry in args.AllEntries)
            {
                DiagnosticInfo diagnosticInfo = CreateDiagnosticInfoFromTableEntry(entry);

                diagnostics.Add(diagnosticInfo);
            }

            CurrentDiagnostics = diagnostics;
            DiagnosticsChanged?.Invoke(this, new DiagnosticsChangedEventArgs(diagnostics));
        }
    }
}
