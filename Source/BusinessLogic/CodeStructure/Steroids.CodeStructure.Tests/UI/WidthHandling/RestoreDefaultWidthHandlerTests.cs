using System.Diagnostics;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeStructure.Settings;
using Steroids.CodeStructure.UI.WidthHandling;
using Steroids.Core.Framework;
using Steroids.Core.Settings;

namespace Steroids.CodeStructure.Tests.UI.WidthHandling
{
    [TestClass]
    public class RestoreDefaultWidthHandlerTests
    {
        private readonly ISettingsController _settingsController = A.Fake<ISettingsController>();
        private readonly IEventAggregator _eventAggregator = new EventAggregator();

        [TestMethod]
        public async Task CreateAsync_Called_DataLoaded()
        {
            // Arrange
            const int expectedWidth = 731;
            A.CallTo(() => _settingsController.LoadAsync<CodeStructureSettingsContainer>()).Returns(
                new CodeStructureSettingsContainer
                {
                    WidthSettings = new CodeStructureWidthSettings
                    {
                        DefaultWidth = expectedWidth
                    }
                });

            // Act
            var sut = await CreateSut();

            // Assert
            A.CallTo(() => _settingsController.LoadAsync<CodeStructureSettingsContainer>()).MustHaveHappened();
            Assert.AreEqual(expectedWidth, sut.GetWidth(string.Empty));
        }

        [TestMethod]
        public async Task EventAggregator_SettingsChanged_WidthUpdated()
        {
            // Arrange
            var sut = await CreateSut();

            // Act
            _eventAggregator.Publish(new CodeStructureSettingsContainer
            {
                WidthSettings = new CodeStructureWidthSettings
                {
                    DefaultWidth = 300
                }
            });

            // Assert
            Assert.AreEqual(300, sut.GetWidth(string.Empty));
        }

        private async Task<IWidthHandler> CreateSut()
        {
            var instance = new RestoreDefaultWidthHandler(_settingsController, _eventAggregator);
            await instance.LoadAsync();
            return instance;
        }
    }
}
