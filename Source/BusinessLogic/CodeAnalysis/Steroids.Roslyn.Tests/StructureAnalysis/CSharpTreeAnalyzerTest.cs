using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Roslyn.CSharp;

namespace Steroids.Roslyn.Tests.StructureAnalysis
{
    [TestClass]
    public class CSharpTreeAnalyzerTest
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

            const string resourceName = "Steroids.Roslyn.Tests.SampleData.SteroidsVsPackageCS.txt";
            var rootNode = GetFileAsSyntaxNode(resourceName);
            var sut = CreateSut();

            // Act
            await sut.Analyze(rootNode, CancellationToken.None);

            // Assert
            var buildTree = sut.NodeList.Select(x => x.Data.Name).ToList();
            CollectionAssert.AreEqual(expectedNameOrder, buildTree);
        }

        private CSharpTreeAnalyzer CreateSut()
        {
            return new CSharpTreeAnalyzer();
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