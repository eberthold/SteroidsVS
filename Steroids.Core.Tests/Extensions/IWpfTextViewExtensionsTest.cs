using System;
using System.Reflection;
using System.Resources;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Core.Extensions;

namespace Steroids.Core.Test.Extensions
{
    [TestClass]
    public class IWpfTextViewExtensionsTest
    {
        [TestMethod]
        public void GetSpanFromLineNumber_OutOfRange_ReturnsNull()
        {
            // Arrange
            var sut = A.Fake<IWpfTextView>();
            A.CallTo(() => sut.TextSnapshot.LineCount).Returns(15);

            // Act
            var result = sut.GetSpanForLineNumber(20);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetSpanFromLineNumber_InRange_RetrurnsSnapshotSpan()
        {
            // Arrange
            var sut = A.Fake<IWpfTextView>();
            var expected = A.Fake<ITextSnapshotLine>();
            A.CallTo(() => sut.TextSnapshot.LineCount).Returns(100);
            A.CallTo(() => sut.TextSnapshot.GetLineFromLineNumber(20)).Returns(expected);

            // Act
            var result = sut.GetSpanForLineNumber(20);

            // Assert
            Assert.AreSame(expected, result);
        }
    }
}
