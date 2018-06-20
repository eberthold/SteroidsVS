using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Steroids.Contracts;
using Steroids.Contracts.UI;
using Steroids.Core;

namespace Steroids.CodeQuality.Models
{
    public class CodeHintLineEntry : BindableBase
    {
        private readonly IEnumerable<DiagnosticInfo> _lineInfos;
        private readonly IWpfTextView _textView;
        private readonly ITrackingSpan _trackingSpan;
        private readonly bool _isActive;
        private readonly IAdornmentSpaceReservation _spaceReservation;

        private double _left;
        private double _width;
        private double _top;
        private string _code;
        private string _message;
        private bool _isVisible;
        private double _scaleFactor;
        private double _opacity;

        public CodeHintLineEntry(
            IWpfTextView textView,
            IEnumerable<DiagnosticInfo> lineInfos,
            SnapshotSpan line,
            IAdornmentSpaceReservation spaceReservation)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _lineInfos = lineInfos ?? throw new ArgumentNullException(nameof(lineInfos));
            _spaceReservation = spaceReservation ?? throw new ArgumentNullException(nameof(spaceReservation));

            WeakEventManager<IAdornmentSpaceReservation, EventArgs>.AddHandler(_spaceReservation, nameof(IAdornmentSpaceReservation.ActualWidthChanged), OnAvailableSpaceChanged);

            _trackingSpan = _textView.TextSnapshot.CreateTrackingSpan(line, SpanTrackingMode.EdgeExclusive);

            var highestDiagnostic = lineInfos.OrderByDescending(x => x.Severity).ThenBy(x => x.Line).ThenBy(x => x.Column).First();
            Code = highestDiagnostic.ErrorCode;
            Message = highestDiagnostic.Message;
            Severity = highestDiagnostic.Severity;

            _isActive = true;
            RefreshPositions();
        }

        /// <summary>
        /// Gets or sets the code of the this code hint entry.
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { Set(ref _code, value); }
        }

        /// <summary>
        /// Gets or sets the message of this code hint entry.
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

        public DiagnosticSeverity Severity { get; }

        /// <summary>
        /// Gets the calculated left value of this entry.
        /// </summary>
        public double Left
        {
            get { return _left; }
            private set { Set(ref _left, value); }
        }

        /// <summary>
        /// Gets the calculated width of this entry.
        /// </summary>
        public double Width
        {
            get { return _width; }
            private set { Set(ref _width, value); }
        }

        public double Height
        {
            get { return 16; }
        }

        public double Top
        {
            get { return _top; }
            set { Set(ref _top, value); }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { Set(ref _isVisible, value); }
        }

        public double ScaleFactor
        {
            get { return _scaleFactor; }
            set { Set(ref _scaleFactor, value); }
        }

        public double Opacity
        {
            get { return _opacity; }
            set { Set(ref _opacity, value); }
        }

        public void RefreshPositions()
        {
            if (!_isActive || _textView.IsClosed)
            {
                return;
            }

            var endPoint = _trackingSpan.GetEndPoint(_textView.TextSnapshot);

            // perf tweak, because GetTextViewLineContainingBufferPosition is rather expensive, so we need to quit early here.
            if (!_textView.TextViewLines.ContainsBufferPosition(endPoint))
            {
                IsVisible = false;
                return;
            }

            var textViewLine = _textView.GetTextViewLineContainingBufferPosition(endPoint);

            IsVisible = textViewLine.VisibilityState > VisibilityState.PartiallyVisible;
            Left = Math.Min(textViewLine.TextRight + 10, _textView.ViewportRight - Height - _spaceReservation.ActualWidth) - _textView.ViewportLeft;

            Width = Math.Max(_textView.ViewportWidth - Left - _spaceReservation.ActualWidth, Height);
            Top = textViewLine.TextTop - _textView.ViewportTop + ((textViewLine.Baseline - Height) / 2);
            ScaleFactor = textViewLine.Baseline / Height;

            CalculateOpacity(textViewLine);
        }

        private void CalculateOpacity(IWpfTextViewLine textViewLine)
        {
            var factor = 0.6 + ((textViewLine.TextRight - _textView.ViewportLeft - Left) / 100);
            Opacity = Math.Max(factor, 0.6);
        }

        /// <summary>
        /// Handles changes of the space that is used by other elements in the same adornment layer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        private void OnAvailableSpaceChanged(object sender, EventArgs e)
        {
            RefreshPositions();
        }
    }
}
