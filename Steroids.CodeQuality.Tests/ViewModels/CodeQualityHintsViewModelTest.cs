using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.Models;
using Steroids.CodeQuality.ViewModels;
using Steroids.Contracts;
using Steroids.Contracts.UI;
using Steroids.Core.Diagnostics.Contracts;
using Steroids.Core.Extensions;

namespace Steroids.CodeQuality.Tests
{
    [TestClass]
    public class CodeQualityHintsViewModelTests
    {
        private const string FilePath = @"c:\file.cs";

        private readonly IWpfTextView _wpfTextView = A.Fake<IWpfTextView>();
        private readonly IQualityTextView _textView = A.Fake<IQualityTextView>();
        private readonly IDiagnosticProvider _diagnosticsProvider = A.Fake<IDiagnosticProvider>();
        private readonly IAdornmentSpaceReservation _spaceReservationManager = A.Fake<IAdornmentSpaceReservation>();
        private readonly IOutliningManager _outliningManager = A.Fake<IOutliningManager>();

        private CodeHintFactory _codeHintFactory;

        [TestMethod]
        public void QualityHints_ClassInitialized_IsEmpty()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.QualityHints;

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void QualityHints_AfterDiagnosticsChanged_HasItems()
        {
            // Arrange
            var sut = CreateSut();

            // TODO: That much arranging work should be reduced by better seams for mocking...
            A.CallTo(() => _textView.Path).Returns(FilePath);
            var line = A.Fake<ITextSnapshotLine>();
            A.CallTo(() => _textView.TextView.TextSnapshot.LineCount).Returns(5);
            A.CallTo(() => _textView.TextView.TextSnapshot.GetLineFromLineNumber(0)).Returns(line);
            A.CallTo(_outliningManager).WithReturnType<IEnumerable<ICollapsible>>().Returns(Enumerable.Empty<ICollapsible>());
            _diagnosticsProvider.DiagnosticsChanged += Raise.With(new DiagnosticsChangedEventArgs
            (
                new List<DiagnosticInfo>
                {
                    new DiagnosticInfo
                    {
                        Path = FilePath,
                        IsActive = true
                    }
                }.AsReadOnly()
            ));

            // Act
            var result = sut.QualityHints;

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        private CodeQualityHintsViewModel CreateSut()
        {
            A.CallTo(() => _textView.TextView).Returns(_wpfTextView);
            _codeHintFactory = new CodeHintFactory(_textView.TextView, _spaceReservationManager);

            return new CodeQualityHintsViewModel(
                _textView,
                _diagnosticsProvider,
                _codeHintFactory,
                _outliningManager);
        }
    }
}
