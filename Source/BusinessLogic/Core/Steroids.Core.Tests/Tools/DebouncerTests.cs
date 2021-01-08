using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.Core.Tools;

namespace Steroids.Core.Tests.Tools
{
    [TestClass]
    public class DebouncerTests
    {
        [TestMethod]
        public async Task Start_MultipleCalls_ExecutedOnce()
        {
            // Arrange
            var counter = 0;

            void IncreaseCounter()
            {
                counter++;
            }

            var sut = new Debouncer(TimeSpan.FromSeconds(1));

            // Act
            Enumerable.Range(0, 10).Select(x =>
            {
                sut.Debounce(IncreaseCounter);
                return true;
            }).ToList();

            // Assert
            await Task.Delay(TimeSpan.FromSeconds(2));
            Assert.AreEqual(1, counter);
        }
    }
}
