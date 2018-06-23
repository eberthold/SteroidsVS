using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Steroids.CodeQuality.Services;

namespace Steroids.CodeQuality.Tests.Services
{
    [TestClass]
    public class ErrorListDiagnosticProviderTest
    {
        private readonly IWpfTableControl _tableControl = A.Fake<IWpfTableControl>();
        private readonly IErrorList _errorList = A.Fake<IErrorList>();

        //[TestMethod]
        //public void MethodName_Scenario_Expectation()
        //{
        //    // Arrange
        //    var sut = CreateSut();
        //    var entries = A.Fake<EntriesChangedEventArgs>();
        //    A.CallTo(() => entries.AllEntries).Returns(new List<ITableEntryHandle>
        //    {

        //    });

        //    // Act
        //    _tableControl.EntriesChanged += Raise.With()

        //    // Assert
        //}

        //private static ITableEntryHandle CreateTableEntryFake(string path)
        //{
        //    string documentName;

        //    var tableEntry = A.Fake<ITableEntryHandle>();
        //    A.CallTo(() => tableEntry.TryGetValue(StandardTableKeyNames.DocumentName, out documentName)).AssignsOutAndRefParameters(path);
        //}

        private ErrorListDiagnosticProvider CreateSut()
        {
            A.CallTo(() => _errorList.TableControl).Returns(_tableControl);
            return new ErrorListDiagnosticProvider(_errorList);
        }
    }
}
