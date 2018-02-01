﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Steroids.CodeStructure.Analyzers;
using Steroids.Common;

namespace Steroids.CodeStructure.Models
{
    public class CodeHintLineEntry : BindableBase
    {
        private readonly IEnumerable<DiagnosticInfo> _lineInfos;
        private readonly IWpfTextView _textView;
        private readonly ITrackingSpan _trackingSpan;
        private readonly bool _isActive;

        private double _left;
        private double _width;
        private double _top;
        private string _code;
        private string _message;
        private bool _isVisible;
        private double _scaleFactor;

        public CodeHintLineEntry(
            IWpfTextView textView,
            IEnumerable<DiagnosticInfo> lineInfos,
            int lineNumber)
        {
            // in some strange cases we are getting diagnostics for lines which aren't available anymore
            if (_textView.TextSnapshot.LineCount <= lineNumber)
            {
                return;
            }

            var line = _textView.TextSnapshot.GetLineFromLineNumber(lineNumber);
            _trackingSpan = _textView.TextSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeExclusive);

            _lineInfos = lineInfos;
            _textView = textView;

            var highestDiagnostic = lineInfos.OrderByDescending(x => x.Severity).ThenBy(x => x.Column).First();
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

        public void RefreshPositions()
        {
            if (!_isActive)
            {
                return;
            }

            var endPoint = _trackingSpan.GetEndPoint(_textView.TextSnapshot);
            var textViewLine = _textView.GetTextViewLineContainingBufferPosition(endPoint);

            IsVisible = textViewLine.VisibilityState > VisibilityState.PartiallyVisible;
            Left = textViewLine.TextRight + 10 - _textView.ViewportLeft;
            Width = _textView.ViewportWidth - Left;
            Top = textViewLine.TextTop - _textView.ViewportTop + ((textViewLine.Baseline - Height) / 2);
            ScaleFactor = textViewLine.Baseline / Height;
        }
    }
}
