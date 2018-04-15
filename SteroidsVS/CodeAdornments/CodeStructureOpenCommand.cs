using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Steroids.CodeStructure.Adorners;
using Steroids.CodeStructure.UI;
using Steroids.Contracts.Core;

namespace SteroidsVS.CodeAdornments
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CodeStructureOpenCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("56a7aec5-9a93-44ca-9c9f-1f6166ebbbfd");

        private readonly Package _package;
        private readonly IActiveTextViewProvider _textViewProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureOpenCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="textViewProvider">The <see cref="IActiveTextViewProvider"/>.</param>
        public CodeStructureOpenCommand(
            Package package,
            IActiveTextViewProvider textViewProvider)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _textViewProvider = textViewProvider ?? throw new ArgumentNullException(nameof(textViewProvider));

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return _package;
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
                .First(x => x.Adornment is CodeStructureView)?
                .Adornment as CodeStructureView;

            if (codeStructure == null)
            {
                return;
            }

            if (codeStructure.IsOpen)
            {
                codeStructure.HideCodeStructure();
            }
            else
            {
                codeStructure.ShowCodeStructure();
            }
        }
    }
}
