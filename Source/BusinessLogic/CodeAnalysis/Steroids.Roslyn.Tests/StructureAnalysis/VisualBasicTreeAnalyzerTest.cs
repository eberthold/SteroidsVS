using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Roslyn.VisualBasic;

namespace Steroids.Roslyn.Tests.StructureAnalysis
{
    [TestClass]
    public class VisualBasicTreeAnalyzerTest
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

            const string resourceName = "Steroids.Roslyn.Tests.SampleData.SteroidsVsPackageVB.txt";
            var rootNode = GetFileAsSyntaxNode(resourceName);
            var sut = CreateSut();

            // Act
            await sut.Analyze(rootNode, CancellationToken.None);

            // Assert
            var buildTree = sut.NodeList.Select(x => x.Data.Name).ToList();
            CollectionAssert.AreEqual(expectedNameOrder, buildTree);
        }

        private VisualBasicTreeAnalyzer CreateSut()
        {
            return new VisualBasicTreeAnalyzer();
        }

        private SyntaxNode GetFileAsSyntaxNode(string resourceName)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            using (var reader = new StreamReader(stream))
            {
                var syntaxTree = VisualBasicSyntaxTree.ParseText(reader.ReadToEnd());
                return syntaxTree.GetRoot();
            }
        }
    }
}