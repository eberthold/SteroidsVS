using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeStructure.Settings;
using Steroids.Core.Framework;
using Steroids.Core.Settings;
using SteroidsVS.Settings;

namespace SteroidVS.Tests.Settings
{
    [TestClass]
    public class SettingsControllerTests
    {
        private readonly IEventAggregator _eventAggregator = A.Fake<IEventAggregator>();
        private readonly ISettingsStoreFactory _settingsStoreFactory = A.Fake<ISettingsStoreFactory>();
        private readonly WritableSettingsStore _settingsStore = new DummyStore();

        [TestMethod]
        public async Task ReadAndLoadWorkingAsync()
        {
            /// arrange
            var sut = CreateSut();
            var settings = CreateDefaultSettings();
            settings.CodeStructure.WidthSettings.DefaultWidth = 1000;
            settings.CodeStructure.WidthSettings.WidthMode = WidthMode.SyncGlobally;
            await SaveSettingsAsync(sut, settings.CodeStructure);

            // act
            var result = await LoadSettingsAsync(sut);

            // assert
            Assert.AreEqual(1000, result.WidthSettings.DefaultWidth);
            Assert.AreEqual(WidthMode.SyncGlobally, result.WidthSettings.WidthMode);
        }

        private Task SaveSettingsAsync<T>(SettingsController sut, T settings)
            where T : ISettingsContainer, new()
        {
            return sut.SaveAsync(settings);
        }

        private Task<CodeStructureSettingsContainer> LoadSettingsAsync(SettingsController sut)
        {
            return sut.LoadAsync<CodeStructureSettingsContainer>();
        }

        private SteroidsSettingsContainer CreateDefaultSettings()
        {
            var codeStructureSettings = new CodeStructureSettingsContainer();

            return new SteroidsSettingsContainer
            {
                CodeStructure =  codeStructureSettings
            };
        }

        private SettingsController CreateSut()
        {
            A.CallTo(() => _settingsStoreFactory.Create()).Returns(_settingsStore);
            return new SettingsController(_eventAggregator, _settingsStoreFactory);
        }

        private class DummyStore : WritableSettingsStore
        {
            private Dictionary<string, Dictionary<string, string>> _stringSettings = new Dictionary<string, Dictionary<string, string>>();

            public override bool CollectionExists(string collectionPath)
            {
                return _stringSettings.ContainsKey(collectionPath);
            }

            public override void CreateCollection(string collectionPath)
            {
                _stringSettings.Add(collectionPath, new Dictionary<string, string>());
            }

            public override bool DeleteCollection(string collectionPath)
            {
                throw new NotImplementedException();
            }

            public override bool DeleteProperty(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override bool GetBoolean(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override bool GetBoolean(string collectionPath, string propertyName, bool defaultValue)
            {
                throw new NotImplementedException();
            }

            public override int GetInt32(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override int GetInt32(string collectionPath, string propertyName, int defaultValue)
            {
                throw new NotImplementedException();
            }

            public override long GetInt64(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override long GetInt64(string collectionPath, string propertyName, long defaultValue)
            {
                throw new NotImplementedException();
            }

            public override DateTime GetLastWriteTime(string collectionPath)
            {
                throw new NotImplementedException();
            }

            public override MemoryStream GetMemoryStream(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override int GetPropertyCount(string collectionPath)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<string> GetPropertyNames(string collectionPath)
            {
                throw new NotImplementedException();
            }

            public override SettingsType GetPropertyType(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override string GetString(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override string GetString(string collectionPath, string propertyName, string defaultValue)
            {
                var prop = _stringSettings[collectionPath];

                if (!prop.ContainsKey(propertyName))
                {
                    return defaultValue;
                }

                return prop[propertyName];
            }

            public override int GetSubCollectionCount(string collectionPath)
            {
                throw new NotImplementedException();
            }

            public override IEnumerable<string> GetSubCollectionNames(string collectionPath)
            {
                throw new NotImplementedException();
            }

            public override uint GetUInt32(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override uint GetUInt32(string collectionPath, string propertyName, uint defaultValue)
            {
                throw new NotImplementedException();
            }

            public override ulong GetUInt64(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override ulong GetUInt64(string collectionPath, string propertyName, ulong defaultValue)
            {
                throw new NotImplementedException();
            }

            public override bool PropertyExists(string collectionPath, string propertyName)
            {
                throw new NotImplementedException();
            }

            public override void SetBoolean(string collectionPath, string propertyName, bool value)
            {
                throw new NotImplementedException();
            }

            public override void SetInt32(string collectionPath, string propertyName, int value)
            {
                throw new NotImplementedException();
            }

            public override void SetInt64(string collectionPath, string propertyName, long value)
            {
                throw new NotImplementedException();
            }

            public override void SetMemoryStream(string collectionPath, string propertyName, MemoryStream value)
            {
                throw new NotImplementedException();
            }

            public override void SetString(string collectionPath, string propertyName, string value)
            {
                if (!_stringSettings.ContainsKey(collectionPath))
                {
                    _stringSettings.Add(collectionPath, new Dictionary<string, string>());
                }

                var prop = _stringSettings[collectionPath];

                if (!prop.ContainsKey(propertyName))
                {
                    prop.Add(propertyName, string.Empty);
                }

                prop[propertyName] = value;
            }

            public override void SetUInt32(string collectionPath, string propertyName, uint value)
            {
                throw new NotImplementedException();
            }

            public override void SetUInt64(string collectionPath, string propertyName, ulong value)
            {
                throw new NotImplementedException();
            }
        }
    }
}
