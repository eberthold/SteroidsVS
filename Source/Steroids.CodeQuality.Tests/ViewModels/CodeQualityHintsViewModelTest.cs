using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.ViewModels;
using Steroids.Contracts;
using Steroids.Contracts.UI;
using Steroids.Core.Diagnostics.Contracts;

namespace Steroids.CodeQuality.Tests
{
    [TestClass]
    public class CodeQualityHintsViewModelTest
    {
        private const string FilePath = @"c:\file.cs";

        private readonly IWpfTextView _wpfTextView = A.Fake<IWpfTextView>();
        private readonly IQualityTextView _textView = A.Fake<IQualityTextView>();
        private readonly IDiagnosticProvider _diagnosticsProvider = A.Fake<IDiagnosticProvider>();
        private readonly IOutliningManagerService _outliningManagerService = A.Fake<IOutliningManagerService>();
        private readonly IOutliningManager _outliningManager = A.Fake<IOutliningManager>();
        private readonly IAdornmentSpaceReservation _adornmentSpaceReservation = A.Fake<IAdornmentSpaceReservation>();

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
            SetupDiagnosticPrerequisites();
            _diagnosticsProvider.DiagnosticsChanged += Raise.With(new DiagnosticsChangedEventArgs(
                new List<DiagnosticInfo>
                {
                    new DiagnosticInfo
                    {
                        Path = FilePath,
                        IsActive = true
                    }
                }.AsReadOnly()));

            // Act
            var result = sut.DiagnosticInfoLines;

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void UpdateDiagnostics_OutliningManagerDisposed_NoExceptionThrown()
        {
            // Arrange
            var sut = CreateSut();
            SetupDiagnosticPrerequisites();
            A.CallTo(_outliningManager).Throws(o => new ObjectDisposedException("OutliningManager"));

            // Act
            _diagnosticsProvider.DiagnosticsChanged += Raise.With(new DiagnosticsChangedEventArgs(
                new List<DiagnosticInfo>
                {
                    new DiagnosticInfo
                    {
                        Path = FilePath,
                        IsActive = true
                    }
                }.AsReadOnly()));

            // Assert
            A.CallTo(_outliningManager).MustHaveHappened();
        }

        [TestMethod]
        public void UpdateDiagnostics_LineIsNull_NoExceptionThrown()
        {
            // Arrange
            var sut = CreateSut();
            SetupDiagnosticPrerequisites();
            A.CallTo(_outliningManager).Throws(o => new ObjectDisposedException("OutliningManager"));

            // Act
            _diagnosticsProvider.DiagnosticsChanged += Raise.With(new DiagnosticsChangedEventArgs(
                new List<DiagnosticInfo>
                {
                    new DiagnosticInfo
                    {
                        Path = FilePath,
                        IsActive = true,
                        LineNumber = 9000
                    }
                }.AsReadOnly()));
        }

        private void SetupDiagnosticPrerequisites()
        {
            A.CallTo(() => _textView.Path).Returns(FilePath);
            var line = A.Fake<ITextSnapshotLine>();
            A.CallTo(() => _textView.TextView.TextSnapshot.LineCount).Returns(5);
            A.CallTo(() => _textView.TextView.TextSnapshot.GetLineFromLineNumber(0)).Returns(line);
            A.CallTo(_outliningManager).WithReturnType<IEnumerable<ICollapsible>>().Returns(Enumerable.Empty<ICollapsible>());
        }

        private CodeQualityHintsViewModel CreateSut()
        {
            A.CallTo(() => _outliningManagerService.GetOutliningManager(_wpfTextView)).Returns(_outliningManager);
            A.CallTo(() => _textView.TextView).Returns(_wpfTextView);

            return new CodeQualityHintsViewModel(
                _textView,
                _diagnosticsProvider,
                _outliningManagerService,
                _adornmentSpaceReservation);
        }
    }
}
