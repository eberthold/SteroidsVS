using System;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.Core.Editor;

namespace SteroidsVS.Editor
{
    public class FoldingManagerAdapter : IFoldingManager
    {
        private readonly IOutliningManager _outliningManager;

        public FoldingManagerAdapter(
            IWpfTextView textView,
            IOutliningManagerService outliningManagerService)
        {
            _outliningManager = outliningManagerService.GetOutliningManager(textView);
            _outliningManager.RegionsExpanded += OnExpanded;
            _outliningManager.RegionsCollapsed += OnCollapsed;
        }

        public event EventHandler Collapsed;

        public event EventHandler Expanded;

        private void OnCollapsed(object sender, RegionsCollapsedEventArgs e)
        {
            Collapsed?.Invoke(this, EventArgs.Empty);
        }

        private void OnExpanded(object sender, RegionsExpandedEventArgs e)
        {
            Expanded?.Invoke(this, EventArgs.Empty);
        }
    }
}
