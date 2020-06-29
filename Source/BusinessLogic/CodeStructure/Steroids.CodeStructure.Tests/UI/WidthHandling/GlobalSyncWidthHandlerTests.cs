using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeStructure.UI.WidthHandling;

namespace Steroids.CodeStructure.Tests.UI.WidthHandling
{
    [TestClass]
    public class GlobalSyncWidthHandlerTests
    {
        [TestMethod]
        public async Task UpdateWidth_Called_ValueUpdatedAndEventFired()
        {
            // Arrange
            const double expected = 500;
            bool eventTriggered = false;
            var sut = await CreateSut();
            sut.CurrentWidthChanged += (_, __) => eventTriggered = true;

            // Act
            sut.UpdateWidth(expected, string.Empty);

            // Assert
            Assert.AreEqual(expected, sut.GetWidth(string.Empty));
            Assert.IsTrue(eventTriggered);
        }

        private Task<GlobalSyncWidthHandler> CreateSut()
        {
            return GlobalSyncWidthHandler.CreateAsync();
        }
    }
}
