using System;
using System.Linq;
using System.Threading.Tasks;
using Steroids.CodeStructure.Settings;
using Steroids.Core.Framework;
using Steroids.Core.Settings;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    public class FileBasedWidthHandler : IWidthHandler
    {
        private readonly ISettingsController _settingsController;
        private CodeStructureSettingsContainer _settings;

        private FileBasedWidthHandler(
            ISettingsController settingsController,
            IEventAggregator eventAggregator)
        {
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
            eventAggregator.Subscribe<CodeStructureSettingsContainer>(OnCodeStructureSettingsChanged);
        }

        /// <inheritdoc />
        public event EventHandler<double> CurrentWidthChanged;

        /// <inheritdoc />
        public double GetWidth(string fileName)
        {
            var isKnownFile = _settings.WidthSettings.FileWidthInfos.Any(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            if (!isKnownFile)
            {
                return _settings.WidthSettings.DefaultWidth;
            }

            var fileWidth = _settings.WidthSettings.FileWidthInfos.First(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            return fileWidth.LastKnownWidth;
        }

        /// <inheritdoc />
        public void UpdateWidth(double width, string fileName)
        {
            var isKnownFile = _settings.WidthSettings.FileWidthInfos.Any(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            if (!isKnownFile)
            {
                _settings.WidthSettings.FileWidthInfos.Add(new FileWidthInfo
                {
                    FileName = fileName,
                    LastKnownWidth = width
                });
            }
            else
            {
                var fileInfo = _settings.WidthSettings.FileWidthInfos.First(x => x.FileName.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
                fileInfo.LastKnownWidth = width;
                _ = _settingsController.SaveAsync(_settings).ConfigureAwait(false);
            }
        }

        internal static async Task<FileBasedWidthHandler> CreateAsync(ISettingsController settingsController, IEventAggregator eventAggregator)
        {
            var instance = new FileBasedWidthHandler(settingsController, eventAggregator);
            await instance.LoadAsync().ConfigureAwait(false);
            return instance;
        }

        private void OnCodeStructureSettingsChanged(CodeStructureSettingsContainer obj)
        {
            _settings = obj;
        }

        private async Task LoadAsync()
        {
            _settings = await _settingsController
                .LoadAsync<CodeStructureSettingsContainer>()
                .ConfigureAwait(false);
        }
    }
}
