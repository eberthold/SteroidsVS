using Steroids.CodeStructure.Analyzers;
using Steroids.Roslyn.CSharp;
using Steroids.Roslyn.VisualBasic;

namespace Steroids.Roslyn.StructureAnalysis
{
    public static class TreeAnalyzerFactory
    {
        public static IRoslynTreeAnalyzer Create(string contentType)
        {
            switch (contentType)
            {
                case KnownContentTypes.CSharp:
                    return new CSharpTreeAnalyzer();

                case KnownContentTypes.VisualBasic:
                    return new VisualBasicTreeAnalyzer();

                default:
                    return null;
            }
        }
    }
}
