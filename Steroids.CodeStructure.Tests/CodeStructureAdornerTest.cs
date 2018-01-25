using System.Linq;
using Steroids.CodeStructure.Analyzers;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeStructure.Adorners;

namespace Steroids.CodeStructure.Tests
{
    [TestClass]
    public class CodeStructureAdornerTest
    {
        private readonly IAdornmentLayer adornmentLayer = A.Fake<IAdornmentLayer>();
        private readonly IWpfTextView textView = A.Fake<IWpfTextView>();

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
            //sut.AddOrUpdateDiagnosticLine(diagnostics);

            // Assert
            A.CallTo(adornmentLayer).Where(x => x.Method.Name == nameof(adornmentLayer.AddAdornment)).MustHaveHappened();
        }

        public CodeStructureAdorner CreateSut()
        {
            return new CodeStructureAdorner(textView);
        }
    }
}
