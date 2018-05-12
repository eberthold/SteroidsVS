namespace Steroids.CodeStructure.Analyzers.Services
{
    public class SyntaxWalkerProvider : ISyntaxWalkerProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxWalkerProvider"/> class.
        /// </summary>
        public SyntaxWalkerProvider()
        {
            SyntaxAnalyzer = new TypeGroupedSyntaxAnalyzer();
        }

        /// <inheritdoc />
        public ICodeStructureSyntaxAnalyzer SyntaxAnalyzer { get; }
    }
}
