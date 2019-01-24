using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Core.CodeQuality;

namespace Steroids.Core.Tests.CodeQuality
{
    [TestClass]
    public class DiagnosticInfoTests
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
