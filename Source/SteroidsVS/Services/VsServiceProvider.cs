﻿using System;
using System.ComponentModel.Design;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.TextManager.Interop;

using Interop = Microsoft.VisualStudio.Shell.Interop;
using Threading = System.Threading.Tasks;

namespace SteroidsVS.Services
{
    public class VsServiceProvider : IVsServiceProvider
    {
        private readonly IAsyncServiceProvider _package;

        public VsServiceProvider(IAsyncServiceProvider package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
        }

        /// <inheritdoc />
        public IComponentModel ComponentModel { get; private set; }

        /// <inheritdoc />
        public IErrorList ErrorList { get; private set; }

        /// <inheritdoc />
        public IOutliningManagerService OutliningManagerService { get; private set; }

        /// <inheritdoc />
        public IVsTextManager VsTextManager { get; private set; }

        /// <inheritdoc />
        public IVsEditorAdaptersFactoryService EditorAdapterFactory { get; private set; }

        /// <inheritdoc />
        public IMenuCommandService MenuCommandService { get; private set; }

        /// <summary>
        /// Initializes the VisualStudio service map async.
        /// </summary>
        /// <returns>A <see cref="Task"/> represenmting the asycnhronous operation.</returns>
        public async Threading.Task InitializeAsync()
        {
            ComponentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            OutliningManagerService = ComponentModel.GetService<IOutliningManagerService>();
            EditorAdapterFactory = ComponentModel.GetService<IVsEditorAdaptersFactoryService>();

            ErrorList = (await _package.GetServiceAsync(typeof(Interop.SVsErrorList)).ConfigureAwait(false)) as IErrorList;
            VsTextManager = (await _package.GetServiceAsync(typeof(SVsTextManager)).ConfigureAwait(false)) as IVsTextManager;
            MenuCommandService = (await _package.GetServiceAsync(typeof(IMenuCommandService)).ConfigureAwait(false)) as OleMenuCommandService;
        }
    }
}
