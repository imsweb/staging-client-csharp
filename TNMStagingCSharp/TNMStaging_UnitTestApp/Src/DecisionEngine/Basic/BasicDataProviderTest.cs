using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStaging_UnitTestApp.Src.DecisionEngine.Basic;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src
{
    [TestClass]
    public class BasicDataProviderTest
    {
        [TestMethod]
        public void testParseEndpoint() //throws Exception
        {
            BasicEndpoint endpoint;
            BasicDataProvider provider = new BasicDataProvider();

            endpoint = provider.parseEndpoint("VALUE:123");
            Assert.AreEqual(EndpointType.VALUE, endpoint.getType());
            Assert.AreEqual("123", endpoint.getValue());

            endpoint = provider.parseEndpoint("VALUE:");
            Assert.AreEqual(EndpointType.VALUE, endpoint.getType());
            Assert.AreEqual("", endpoint.getValue());

            endpoint = provider.parseEndpoint("VALUE");
            Assert.AreEqual(EndpointType.VALUE, endpoint.getType());
            Assert.IsNull(endpoint.getValue());

            endpoint = provider.parseEndpoint("VALUE:ABC:123:XYZ");
            Assert.AreEqual(EndpointType.VALUE, endpoint.getType());
            Assert.AreEqual("ABC:123:XYZ", endpoint.getValue());
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testNullJumpValue()
        {
            BasicDataProvider provider = new BasicDataProvider();

            provider.parseEndpoint("JUMP");
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testEmptyJumpValue()
        {
            BasicDataProvider provider = new BasicDataProvider();

            provider.parseEndpoint("JUMP:");
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testBadType()
        {
            BasicDataProvider provider = new BasicDataProvider();

            provider.parseEndpoint("XXX:123");
        }

    }
}
