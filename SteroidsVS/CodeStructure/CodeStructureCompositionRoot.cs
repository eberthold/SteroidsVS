namespace SteroidsVS.CodeStructure
{
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Adorners;
    using Steroids.CodeStructure.Analyzers.Services;
    using Steroids.CodeStructure.ViewModels;
    using Unity;
    using Unity.Lifetime;

    public class CodeStructureCompositionRoot : CompositionRoot
    {
        private readonly IWpfTextView _textView;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureCompositionRoot"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to analyze.</param>
        public CodeStructureCompositionRoot(IWpfTextView textView)
        {
            _textView = textView;
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            Container = RootContainer.CreateChildContainer();
            Container.RegisterInstance(_textView);
            Container.RegisterType<ISyntaxWalkerProvider, SyntaxWalkerProvider>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureAdorner>(new ContainerControlledLifetimeManager());
            Container.RegisterType<CodeStructureViewModel>(new ContainerControlledLifetimeManager());
            Container.RegisterType<FloatingDiagnosticHintsViewModel>(new ContainerControlledLifetimeManager());
        }
    }
}
