using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Steroids.Contracts;
using Steroids.Core;

namespace Steroids.CodeQuality.UI
{
    /// <summary>
    /// Collects all diagnostics which belong to a specific line.
    /// </summary>
    public class DiagnosticInfoLine : BindableBase
    {
        private readonly DiagnosticInfoComputedLineComparer _computedLineComparer = new DiagnosticInfoComputedLineComparer();

        private bool _isVisible;
        private IReadOnlyCollection<DiagnosticInfo> _diagnosticInfos;
        private DiagnosticInfo _visibleDiagnostic;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticInfoLine"/> class.
        /// </summary>
        /// <param name="lineNumber">The line of all the diagnostics.</param>
        /// <param name="diagnosticInfos">The collection of <see cref="DiagnosticInfo"/> on this line.</param>
        public DiagnosticInfoLine(int lineNumber, IReadOnlyCollection<DiagnosticInfo> diagnosticInfos)
        {
            LineNumber = lineNumber;
            DiagnosticInfos = diagnosticInfos ?? throw new ArgumentNullException(nameof(diagnosticInfos));
        }

        /// <summary>
        /// The number of the line of all diagnostics.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// All diagnostics belonging to this line.
        /// </summary>
        public IReadOnlyCollection<DiagnosticInfo> DiagnosticInfos
        {
            get => _diagnosticInfos;
            internal set
            {
                if (!Set(ref _diagnosticInfos, value))
                {
                    return;
                }

                var highestdiagnostic = _diagnosticInfos.OrderBy(x => x, _computedLineComparer).First();
                if (highestdiagnostic == _visibleDiagnostic)
                {
                    return;
                }

                _visibleDiagnostic = highestdiagnostic;
                RaisePropertyChanged(nameof(Severity));
                RaisePropertyChanged(nameof(ErrorCode));
                RaisePropertyChanged(nameof(Message));
            }
        }

        /// <summary>
        /// Tells if the line is currently visible or not.
        /// Can become invisible if it is out of sight or it should be hidden.
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        /// <summary>
        /// The severity of the most important diagnostic.
        /// </summary>
        public DiagnosticSeverity Severity => _visibleDiagnostic?.Severity ?? DiagnosticSeverity.Hidden;

        /// <summary>
        /// The error code of the most important diagnostic.
        /// </summary>
        public string ErrorCode => _visibleDiagnostic.ErrorCode;

        /// <summary>
        /// The message of the most important diagnostic.
        /// </summary>
        public string Message => _visibleDiagnostic.Message;
    }
}
