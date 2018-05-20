using Steroids.Contracts.Core;

namespace Steroids.CodeStructure.Analyzers.Services
{
    public class SyntaxWalkerProvider : ISyntaxWalkerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxWalkerProvider"/> class.
        /// </summary>
        /// <param name="dispatcherService">The <see cref="IDispatcherService"/>.</param>
        public SyntaxWalkerProvider(IDispatcherService dispatcherService)
        {
            SyntaxAnalyzer = new TypeGroupedSyntaxAnalyzer(dispatcherService);
        }

        /// <inheritdoc />
        public ICodeStructureSyntaxAnalyzer SyntaxAnalyzer { get; }
    }
}
