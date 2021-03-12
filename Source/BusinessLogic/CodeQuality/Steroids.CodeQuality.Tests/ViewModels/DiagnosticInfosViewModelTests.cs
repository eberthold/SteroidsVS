using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeQuality.UI;
using Steroids.Core.CodeQuality;
using Steroids.Core.Editor;
using Steroids.Core.Framework;

namespace Steroids.CodeQuality.Tests
{
    [TestClass]
    public class DiagnosticInfosViewModelTests
    {
        private const string FilePath = @"c:\file.cs";

        private readonly IEditor _editor = A.Fake<IEditor>();
        private readonly IFoldingManager _foldingManager = A.Fake<IFoldingManager>();
        private readonly IDiagnosticProvider _diagnosticsProvider = A.Fake<IDiagnosticProvider>();
        private readonly IDispatcherServiceFactory _dispatcherServiceProvider = A.Fake<IDispatcherServiceFactory>();

        [TestMethod]
        public void QualityHints_ClassInitialized_IsEmpty()
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var result = sut.DiagnosticInfoLines;

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

        private void SetupDiagnosticPrerequisites()
        {
            A.CallTo(() => _editor.FilePath).Returns(FilePath);
        }

        private DiagnosticInfosViewModel CreateSut()
        {
            A.CallTo(() => _editor.FoldingManager).Returns(_foldingManager);

            return new DiagnosticInfosViewModel(
                _editor,
                _dispatcherServiceProvider,
                _diagnosticsProvider);
        }
    }
}
