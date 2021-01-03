using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Steroids.Core.CodeQuality;

namespace SteroidsVS.CodeQuality.Diagnostic
{
    /// <summary>
    /// Provides <see cref="DiagnosticInfo"/> by crawling the ErrorList UI directly.
    /// </summary>
    public class ErrorListDiagnosticProvider : IDiagnosticProvider
    {
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
        /// Handles the <see cref="IWpfTableControl.EntriesChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="EntriesChangedEventArgs"/>.</param>
        private void OnErrorListEntriesChanged(object sender, EntriesChangedEventArgs args)
        {
            var diagnostics = CurrentDiagnostics.ToDictionary(
                x => x.GetHashCode(),
                x => x);
            var tableEntries = args.AllEntries.ToLookup(
                x => x.DiagnosticInfoHashCode(),
                x => x);

            // remove entries which seems to be unused with the new diagnostic set.
            foreach (var item in diagnostics
                .Where(x => !tableEntries.Any(y => y.Key == x.Key))
                .Select(x => x.Key)
                .ToList())
            {
                diagnostics.Remove(item);
            }

            foreach (var item in tableEntries.Where(x => !diagnostics.ContainsKey(x.Key)).ToList())
            {
                diagnostics.Add(item.Key, item.First().ToDiagnosticInfo());
            }

            CurrentDiagnostics = diagnostics.Select(x => x.Value).ToList();
            DiagnosticsChanged?.Invoke(this, new DiagnosticsChangedEventArgs(CurrentDiagnostics));
        }
    }
}
