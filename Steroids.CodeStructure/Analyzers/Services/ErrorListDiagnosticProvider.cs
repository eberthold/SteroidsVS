namespace Steroids.CodeStructure.Analyzers.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Shell.TableControl;
    using Microsoft.VisualStudio.Shell.TableManager;

    public class ErrorListDiagnosticProvider : IDiagnosticProvider
    {
        private const string SuppressionState = "suppressionstate";
        private const string ActiveSuppression = "Active";

        private readonly IErrorList _errorList;
        private List<DiagnosticInfo> _diagnosticInfos = new List<DiagnosticInfo>();

        public ErrorListDiagnosticProvider(IErrorList errorList)
        {
            _errorList = errorList ?? throw new ArgumentNullException(nameof(errorList));
            errorList.TableControl.EntriesChanged += OnErrorListEntriesChanged;
        }

        public event EventHandler<DiagnosticsChangedEventArgs> DiagnosticsChanged;

        private void OnErrorListEntriesChanged(object sender, EntriesChangedEventArgs args)
        {
            _diagnosticInfos.Clear();

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

                _diagnosticInfos.Add(new DiagnosticInfo
                {
                    Severity = severity,
                    Path = path,
                    Message = fullText,
                    ErrorCode = errorCode,
                    HelpUriRaw = helpLink,
                    Line = line,
                    Column = column,
                    IsActive = suppressionState == ActiveSuppression
                });
            }

            DiagnosticsChanged?.Invoke(this, new DiagnosticsChangedEventArgs(_diagnosticInfos.AsReadOnly()));
        }
    }
}
