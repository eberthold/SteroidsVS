using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.Roslyn.StructureAnalysis
{
    public interface IRoslynTreeAnalyzer : ICodeStructureSyntaxAnalyzer
    {
        /// <summary>
        /// Parses the text and generates a syntax tree
        /// </summary>
        /// <param name="text">The text to parse.</param>
        /// <returns>The parsed <see cref="SyntaxTree"/>.</returns>
        SyntaxTree ParseText(string text);
    }
}
