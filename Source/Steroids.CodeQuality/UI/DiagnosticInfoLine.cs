using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Steroids.Contracts;
using Steroids.Core;

namespace Steroids.CodeQuality.UI
{
    /// <summary>
    /// Collects all diagnostics which belong to a specific line.
    /// </summary>
    public class DiagnosticInfoLine : BindableBase
    {
        private bool _isVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticInfoLine"/> class.
        /// </summary>
        /// <param name="lineNumber">The line of all the diagnostics.</param>
        /// <param name="diagnosticInfos">The collection of <see cref="DiagnosticInfo"/> on this line.</param>
        public DiagnosticInfoLine(int lineNumber, IReadOnlyCollection<DiagnosticInfo> diagnosticInfos)
        {
            LineNumber = lineNumber;
            DiagnosticInfos = new ObservableCollection<DiagnosticInfo>(diagnosticInfos ?? throw new ArgumentNullException(nameof(diagnosticInfos)));
        }

        /// <summary>
        /// The number of the line of all diagnostics.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// All diagnostics belonging to this line.
        /// </summary>
        public ObservableCollection<DiagnosticInfo> DiagnosticInfos { get; }

        /// <summary>
        /// Tells if the line is currently visible or not.
        /// Can become invisible if it is out of sight or it should be hidden.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }
    }
}
