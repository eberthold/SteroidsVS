using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using Steroids.CodeStructure.Adorners;
using Steroids.CodeStructure.UI;
using Steroids.Contracts.Core;
using Threading = System.Threading.Tasks;

namespace SteroidsVS.CodeAdornments
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CodeStructureOpenCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("56a7aec5-9a93-44ca-9c9f-1f6166ebbbfd");

        private readonly IActiveTextViewProvider _textViewProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureOpenCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="vsServiceProvider">The <see cref="IVsServiceProvider"/>.</param>
        /// <param name="textViewProvider">The <see cref="IActiveTextViewProvider"/>.</param>
        public CodeStructureOpenCommand(
            IVsServiceProvider vsServiceProvider,
            IActiveTextViewProvider textViewProvider)
        {
            _textViewProvider = textViewProvider ?? throw new ArgumentNullException(nameof(textViewProvider));
            if (vsServiceProvider == null)
            {
                throw new ArgumentNullException(nameof(vsServiceProvider));
            }

            RegisterCommandAsync(vsServiceProvider).ConfigureAwait(false);
        }

        private async Threading.Task RegisterCommandAsync(IVsServiceProvider vsServiceProvider)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var commandService = vsServiceProvider.MenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var codeStructure = _textViewProvider
                .ActiveTextView?
                .GetAdornmentLayer(nameof(CodeStructureAdorner))?
                .Elements
                .FirstOrDefault(x => x.Adornment is ContentControl)?
                .Adornment as ContentControl;

            var viewModel = codeStructure.Content as CodeStructureViewModel;
            if (viewModel == null)
            {
                return;
            }

            viewModel.IsOpen = !viewModel.IsOpen;
        }
    }
}
