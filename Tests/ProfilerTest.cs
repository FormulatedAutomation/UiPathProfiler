using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulatedAutomation.UiPathProfiler.Activities;
using System.IO;
using System.Reflection;

namespace FormulatedAutomation.UiPathProfiler.Tests
{
    [TestClass]
    public class ProfilerTest
    {
        [TestMethod]
        public void TestProfileCreation()
        {
            Profiler profiler = new Profiler(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            int numRecords = profiler.WriteProfile();
            Assert.IsTrue(numRecords > 0);
        }
    }
}

