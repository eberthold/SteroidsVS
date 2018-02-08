using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeStructure.Adorners;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.CodeStructure.Tests
{
    [TestClass]
    public class CodeStructureAdornerTest
    {
        private readonly IAdornmentLayer _adornmentLayer = A.Fake<IAdornmentLayer>();
        private readonly IWpfTextView _textView = A.Fake<IWpfTextView>();

        [TestMethod]
        public void AddOrUpdateDiagnosticLine_NewDiagnosticInfo_AddsAdornment()
        {
            // Arrange
            var sut = CreateSut();
            var di = A.Fake<DiagnosticInfo>();

            var diagnostics = new List<DiagnosticInfo>
            {
                new DiagnosticInfo()
            }.ToLookup(x => 0);

            // Act
            // sut.AddOrUpdateDiagnosticLine(diagnostics);

            // Assert
            A.CallTo(_adornmentLayer).Where(x => x.Method.Name == nameof(_adornmentLayer.AddAdornment)).MustHaveHappened();
        }

        public CodeStructureAdorner CreateSut()
        {
            return new CodeStructureAdorner(_textView);
        }
    }
}
