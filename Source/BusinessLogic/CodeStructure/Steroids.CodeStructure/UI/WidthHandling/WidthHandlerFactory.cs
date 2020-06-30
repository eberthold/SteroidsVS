using System;
using System.Threading.Tasks;
using Steroids.CodeStructure.Settings;
using Steroids.Core.Framework;
using Steroids.Core.Settings;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    public class WidthHandlerFactory
    {
        private readonly ISettingsController _settingsController;
        private readonly IEventAggregator _eventAggregator;

        public WidthHandlerFactory(ISettingsController settingsController, IEventAggregator eventAggregator)
        {
            _settingsController = settingsController ?? throw new ArgumentNullException(nameof(settingsController));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        }

        public async Task<IWidthHandler> CreateAsync(WidthMode widthMode)
        {
            switch (widthMode)
            {
                case WidthMode.RestoreDefault:
                    var restoreDefaultHandler = new RestoreDefaultWidthHandler(_settingsController, _eventAggregator);
                    await restoreDefaultHandler.LoadAsync().ConfigureAwait(false);
                    return restoreDefaultHandler;

                case WidthMode.StorePerFile:
                    var fileBasedHandler = new FileBasedWidthHandler(_settingsController, _eventAggregator);
                    await fileBasedHandler.LoadAsync().ConfigureAwait(false);
                    return fileBasedHandler;

                case WidthMode.SyncGlobally:
                    return new GlobalSyncWidthHandler();

                default:
                    throw new NotSupportedException("Unknown WidthMode provided");
            }
        }
    }
}
