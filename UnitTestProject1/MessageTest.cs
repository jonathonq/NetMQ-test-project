using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using NetMQ_test_project;

namespace UnitTestProject1
{
    [TestClass]
    public class MessageTest
    {
        [TestMethod]
        public void SearchString_PassedKeyStringToFindWithinSourceString_Under200MicroSeconds()
        {
            var sw = new Stopwatch();
            string SourceString = "30000;-12.634 30001;-0.694 30002;4.125";
            string KeyString = "30000";
            sw.Start();
           int result = Message.SearchString(SourceString, KeyString);
            sw.Stop();
            Assert.IsTrue(sw.Elapsed.TotalMilliseconds*1000 < 200, $"{sw.Elapsed.TotalMilliseconds*1000}us");
        }
    }
}
