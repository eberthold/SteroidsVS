using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeStructure.Settings;
using SteroidsVS.CodeStructure.Settings;

namespace SteroidsVS.CodeStructure.Tests.Settings
{
    [TestClass]
    public class SettingsViewModelTests
    {
        private readonly ISettingsService _settingService = A.Fake<ISettingsService>();

        [TestMethod]
        public void DefaultRestoreWidth_Set_NewValueSaved()
        {
            var sut = CreateSut();
            const int width = 313;

            sut.DefaultRestoreWidth = width;

            A.CallTo(() => _settingService.SaveSettingsAsync(
                    A<CodeStructureSettingsContainer>
                    .That
                    .Matches(x => x.WidthSettings.DefaultWidth == width)))
                .MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void IsRestoreDefault_Set_NewValueSaved()
        {
            var sut = CreateSut();

            sut.IsRestoreDefault = true;

            A.CallTo(() => _settingService.SaveSettingsAsync(
                    A<CodeStructureSettingsContainer>
                    .That
                    .Matches(x => x.WidthSettings.WidthMode == WidthMode.RestoreDefault)))
                .MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void IsFileBasedRestore_Set_NewValueSaved()
        {
            var sut = CreateSut();

            sut.IsFileBasedRestore = true;

            A.CallTo(() => _settingService.SaveSettingsAsync(
                    A<CodeStructureSettingsContainer>
                    .That
                    .Matches(x => x.WidthSettings.WidthMode == WidthMode.StorePerFile)))
                .MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void IsSyncGlobally_Set_NewValueSaved()
        {
            var sut = CreateSut();

            sut.IsSyncGlobally = true;

            A.CallTo(() => _settingService.SaveSettingsAsync(
                    A<CodeStructureSettingsContainer>
                    .That
                    .Matches(x => x.WidthSettings.WidthMode == WidthMode.SyncGlobally)))
                .MustHaveHappenedOnceExactly();
        }

        private SettingsViewModel CreateSut()
        {
            A.CallTo(() => _settingService.LoadSettingsAsync()).Returns(new CodeStructureSettingsContainer());
            var vm = new SettingsViewModel(_settingService);
            _ = vm.LoadDataAsync();
            return vm;
        }
    }
}
