using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.Models;
using Steroids.CodeQuality.ViewModels;
using Steroids.Contracts;
using Steroids.Contracts.UI;

namespace Steroids.CodeQuality.Tests
{
    [TestClass]
    public class CodeQualityHintsViewModelTests
    {
        private readonly IWpfTextView _textView = A.Fake<IWpfTextView>();
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

        private CodeQualityHintsViewModel CreateSut()
        {
            _codeHintFactory = new CodeHintFactory(_textView, _spaceReservationManager);

            return new CodeQualityHintsViewModel(
                _textView,
                _diagnosticsProvider,
                _codeHintFactory,
                _outliningManager);
        }
    }
}
