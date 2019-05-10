using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeStructure.Analyzers;

namespace Steroids.CodeStructure.Tests.Analyzers
{
    [TestClass]
    public class SortedTreeTests
    {
        [TestMethod]
        public void QueryChilds_ReturnsAllLayers()
        {
            // Arrange
            var sut = new SortedTree<int>(1);
            sut.Add(12).Add(122).Add(1222);
            sut.Add(13);
            sut.Add(11).Add(111).Add(1111);
            var expected = new List<int> { 1, 11, 111, 1111, 12, 122, 1222, 13 };

            // Act
            var result = sut.Select(x => x.Data).ToList();

            // Assert
            CollectionAssert.AreEqual(expected, result, $"{string.Join(",", expected.Select(x => x.ToString()))} => {string.Join(",", result.Select(x => x.ToString()))}");
        }
    }
}
