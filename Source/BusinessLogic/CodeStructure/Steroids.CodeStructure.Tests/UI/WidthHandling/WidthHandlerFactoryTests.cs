using System;
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
    public class WidthHandlerFactoryTests
    {
        private readonly ISettingsController _settingsController = A.Fake<ISettingsController>();
        private readonly IEventAggregator _eventAggregator = A.Fake<IEventAggregator>();

        [TestMethod]
        [DataRow(WidthMode.RestoreDefault, typeof(RestoreDefaultWidthHandler))]
        [DataRow(WidthMode.SyncGlobally, typeof(GlobalSyncWidthHandler))]
        [DataRow(WidthMode.StorePerFile, typeof(FileBasedWidthHandler))]
        public async Task CreateAsync_RestoreDefault_ExpectedInstance(WidthMode mode, Type expectedType)
        {
            // Arrange
            var sut = CreateSut();

            // Act
            var instance = await sut.CreateAsync(mode);

            // Assert
            Assert.IsInstanceOfType(instance, expectedType);
        }

        private WidthHandlerFactory CreateSut()
        {
            return new WidthHandlerFactory(_settingsController, _eventAggregator);
        }
    }
}
