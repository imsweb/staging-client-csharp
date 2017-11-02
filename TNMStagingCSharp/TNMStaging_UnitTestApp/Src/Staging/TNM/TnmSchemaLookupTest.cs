
using Microsoft.VisualStudio.TestTools.UnitTesting;


using TNMStagingCSharp.Src.Staging.TNM;



namespace TNMStaging_UnitTestApp.Src.Staging.TNM
{
    [TestClass]
    public class TnmSchemaLookupTest
    {

        [TestMethod]
        public void testDescriminator()
        {
            TnmSchemaLookup lookup = new TnmSchemaLookup("C629", "9100");
            Assert.IsFalse(lookup.hasDiscriminator());
            lookup.setInput(TnmStagingData.SSF25_KEY, "");
            Assert.IsFalse(lookup.hasDiscriminator());
            lookup.setInput(TnmStagingData.SSF25_KEY, "001");
            Assert.IsTrue(lookup.hasDiscriminator());
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testBadKey()
        {
            TnmSchemaLookup lookup = new TnmSchemaLookup("C509", "8000");
            lookup.setInput("bad_key", "111");
        }

    }

}

