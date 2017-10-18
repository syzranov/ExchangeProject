using Exchange.Extentions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Exchange.UnitTests
{
    [TestClass]
    public class TestRandData
    {
        [TestMethod]
        public void TestDecimal()
        {
            var d = (decimal)1.0;
            for (int i = 0; i < 10; i++)
            {
                var a = d.Next();
            }
        }
    }
}
