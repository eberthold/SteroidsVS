using System.Collections.Generic;
using System.Linq;
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
    public class FileBasedWidthHandlerTests
    {
        private readonly ISettingsController _settingsController = A.Fake<ISettingsController>();
        private readonly IEventAggregator _eventAggregator = A.Fake<IEventAggregator>();

        [TestMethod]
        public async Task GetWidth_RetoredFromSettings_ExpectedValue()
        {
            // Arrange
            const string file = "myFile.dat";
            const double expected = 433;
            A.CallTo(() => _settingsController.LoadAsync<CodeStructureSettingsContainer>()).Returns(new CodeStructureSettingsContainer
            {
                WidthSettings = new CodeStructureWidthSettings
                {
                    FileWidthInfos = new List<FileWidthInfo>
                    {
                        new FileWidthInfo
                        {
                            FileName = file,
                            LastKnownWidth = expected
                        }
                    }
                }
            });

            var sut = await CreateSut();

            // Act
            var value = sut.GetWidth(file);

            // Assert
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        public async Task SetWidth_WithValue_SettingsSaved()
        {
            // Arrange
            const string file = "myFile.dat";
            const double expected = 433;
            var settings = new CodeStructureSettingsContainer
            {
                WidthSettings = new CodeStructureWidthSettings
                {
                    FileWidthInfos = new List<FileWidthInfo>
                    {
                        new FileWidthInfo
                        {
                            FileName = file,
                            LastKnownWidth = 200
                        }
                    }
                }
            };

            A.CallTo(() => _settingsController.LoadAsync<CodeStructureSettingsContainer>()).Returns(settings);

            var sut = await CreateSut();

            // Act
            sut.UpdateWidth(expected, file);

            // Assert
            Assert.AreEqual(expected, settings.WidthSettings.FileWidthInfos.Single().LastKnownWidth);
            A.CallTo(() => _settingsController.SaveAsync<CodeStructureSettingsContainer>(settings)).MustHaveHappened();
        }

        private async Task<FileBasedWidthHandler> CreateSut()
        {
            var instance = new FileBasedWidthHandler(_settingsController, _eventAggregator);
            await instance.LoadAsync();
            return instance;
        }
    }
}
