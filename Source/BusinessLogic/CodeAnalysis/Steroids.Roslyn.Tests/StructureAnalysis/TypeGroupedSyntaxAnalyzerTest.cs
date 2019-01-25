using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Roslyn.StructureAnalysis;

namespace Steroids.Roslyn.Tests.StructureAnalysis
{
    [TestClass]
    public class TypeGroupedSyntaxAnalyzerTest
    {
        [TestMethod]
        public async Task Analyze_ValidFile_CreatesMatchingTree()
        {
            // Arrange
            var expectedNameOrder = new List<string>
            {
                "SteroidsVsPackage",
                "Fields",
                "_initialized",
                "PackageGuidString",
                "Properties",
                "VsServiceProvider",
                "Methods",
                "Initialize",
                "InitializeDictionary"
            };

            const string resourceName = "Steroids.Roslyn.Tests.SampleData.SteroidsVsPackage.txt";
            var rootNode = GetFileAsSyntaxNode(resourceName);
            var sut = CreateSut();

            // Act
            await sut.Analyze(rootNode, CancellationToken.None);

            // Assert
            var buildTree = sut.NodeList.Select(x => x.Name).ToList();
            CollectionAssert.AreEqual(expectedNameOrder, buildTree);
        }

        private TypeGroupedSyntaxAnalyzer CreateSut()
        {
            return new TypeGroupedSyntaxAnalyzer();
        }

        private SyntaxNode GetFileAsSyntaxNode(string resourceName)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using (var reader = new StreamReader(stream))
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(reader.ReadToEnd());
                return syntaxTree.GetRoot();
            }
        }
    }
}