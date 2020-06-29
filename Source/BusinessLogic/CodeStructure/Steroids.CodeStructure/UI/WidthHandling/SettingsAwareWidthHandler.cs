using System;
using Steroids.CodeStructure.Settings;
using Steroids.Core.Framework;
using Steroids.Core.Settings;

namespace Steroids.CodeStructure.UI.WidthHandling
{
    public class SettingsAwareWidthHandler : IWidthHandler
    {
        private IWidthHandler _currentHandler;

        public SettingsAwareWidthHandler(IEventAggregator eventAggregator)
        {
            eventAggregator.Subscribe<SettingsChangedMessage<CodeStructureSettingsContainer>>(OnSettingsChanged);

        }

        public event EventHandler<double> CurrentWidthChanged;

        public void UpdateWidth(double width, string fileName)
        {
            throw new NotImplementedException();
        }

        public double GetWidth(string fileName)
        {
            throw new NotImplementedException();
        }

        private void OnSettingsChanged(SettingsChangedMessage<CodeStructureSettingsContainer> obj)
        {
            throw new NotImplementedException();
        }
    }
}
