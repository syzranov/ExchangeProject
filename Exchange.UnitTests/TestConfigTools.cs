using Exchange.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exchange.UnitTests
{
    [TestClass]
    public class TestConfigTools
    {
        [TestMethod]
        public void GetHost()
        {
            Assert.AreEqual("localhost", ConfigTools.Instance.Get<string>("exchange-server-host"));
        }

        [TestMethod]
        public void GetPort()
        {
            Assert.AreEqual(11000, ConfigTools.Instance.Get<int>("exchange-server-port"));
        }

        [TestMethod]
        public void GetPeriod()
        {
            Assert.AreEqual(3000, ConfigTools.Instance.Get<int>("exchange-monitor-refresh-period"));
        }
    }
}
