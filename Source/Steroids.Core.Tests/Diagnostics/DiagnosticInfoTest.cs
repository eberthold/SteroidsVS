using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Contracts;

namespace Steroids.Core.Test.Diagnostics
{
    [TestClass]
    public class DiagnosticInfoTest
    {
        [TestMethod]
        public void CompareTo_OnlyIsActiveChanged_NotEqual()
        {
            // Arrange
            var a = new DiagnosticInfo { LineNumber = 10 };
            var b = new DiagnosticInfo { LineNumber = 10 };

            // Pre-Assert
            Assert.AreEqual(a, b);

            // Act
            b.IsActive = true;

            // Assert
            Assert.AreNotEqual(a, b);
        }
    }
}
