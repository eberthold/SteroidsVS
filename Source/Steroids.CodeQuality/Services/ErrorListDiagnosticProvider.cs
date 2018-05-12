using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Steroids.Contracts;

namespace Steroids.CodeQuality.Services
{
    public class ErrorListDiagnosticProvider : IDiagnosticProvider
    {
        private const string SuppressionState = "suppressionstate";
        private const string NotSuppressed = "Active";

        public ErrorListDiagnosticProvider(IErrorList errorList)
        {
            if (errorList == null)
            {
                throw new ArgumentNullException(nameof(errorList));
            }

            WeakEventManager<IWpfTableControl, EntriesChangedEventArgs>.AddHandler(errorList.TableControl, nameof(IWpfTableControl.EntriesChanged), OnErrorListEntriesChanged);
        }

        /// <summary>
        /// Triggered when a new set of Diagnostics get popuplated.
        /// </summary>
        public event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;

        private void OnErrorListEntriesChanged(object sender, EntriesChangedEventArgs args)
        {
            var diagnostics = new List<DiagnosticInfo>();

            foreach (var entry in args.AllEntries)
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

                diagnostics.Add(new DiagnosticInfo
                {
                    Severity = severity,
                    Path = path,
                    Message = fullText,
                    ErrorCode = errorCode,
                    HelpUriRaw = helpLink,
                    Line = line,
                    Column = column,
                    IsActive = suppressionState == NotSuppressed
                });
            }

            DiagnosticsChanged?.Invoke(this, new DiagnosticsChangedEventArgs(diagnostics.AsReadOnly()));
        }
    }
}
