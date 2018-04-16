using System;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SteroidsVS.Services
{
    public class VsServiceProvider : IVsServiceProvider
    {
        public VsServiceProvider(IServiceProvider package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            ComponentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            OutliningManagerService = ComponentModel.GetService<IOutliningManagerService>();
            EditorAdapterFactory = ComponentModel.GetService<IVsEditorAdaptersFactoryService>();

            ErrorList = package.GetService(typeof(SVsErrorList)) as IErrorList;
            VsTextManager = package.GetService(typeof(SVsTextManager)) as IVsTextManager;
        }

        public IComponentModel ComponentModel { get; }

        /// <inheritdoc />
        public IErrorList ErrorList { get; }

        /// <inheritdoc />
        public IOutliningManagerService OutliningManagerService { get; }

        /// <inheritdoc />
        public IVsTextManager VsTextManager { get; }

        public IVsEditorAdaptersFactoryService EditorAdapterFactory { get; }
    }
}
