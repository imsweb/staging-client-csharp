using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging.CS;


namespace TNMStaging_UnitTestApp.Src
{
    [TestClass]
    public class CsSchemaLookupTest
    {

        [TestMethod]
        public void testDescriminator()
        {
            Assert.IsFalse(new CsSchemaLookup("C629", "9100", null).hasDiscriminator());
            Assert.IsFalse(new CsSchemaLookup("C629", "9100", "").hasDiscriminator());
            Assert.IsTrue(new CsSchemaLookup("C629", "9100", "001").hasDiscriminator());

            CsSchemaLookup lookup = new CsSchemaLookup("C509", "8000");
            Assert.IsFalse(lookup.hasDiscriminator());
            lookup.setInput(CsStagingData.SSF25_KEY, "111");
            Assert.IsTrue(lookup.hasDiscriminator());
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testBadKey()
        {
            CsSchemaLookup lookup = new CsSchemaLookup("C509", "8000");
            lookup.setInput("bad_key", "111");
        }

    }

}

