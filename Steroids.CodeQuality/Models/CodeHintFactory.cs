using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Contracts;
using Steroids.Contracts.UI;

namespace Steroids.CodeQuality.Models
{
    public class CodeHintFactory
    {
        private readonly IWpfTextView _textView;
        private readonly IAdornmentSpaceReservation _spaceReservation;

        public CodeHintFactory(IWpfTextView textView, IAdornmentSpaceReservation spaceReservation)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _spaceReservation = spaceReservation ?? throw new ArgumentNullException(nameof(spaceReservation));
        }

        public CodeHintLineEntry Create(IEnumerable<DiagnosticInfo> infos, int lineNuber)
        {
            return new CodeHintLineEntry(_textView, infos, lineNuber, _spaceReservation);
        }
    }
}
