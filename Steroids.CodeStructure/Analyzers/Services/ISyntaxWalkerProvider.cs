namespace Steroids.CodeStructure.Analyzers.Services
{
    public interface ISyntaxWalkerProvider
    {
        /// <summary>
        /// Gets the current <see cref="SyntaxWalker"/>.
        /// </summary>
        ICodeStructureSyntaxAnalyzer SyntaxAnalyzer { get; }
    }
}
