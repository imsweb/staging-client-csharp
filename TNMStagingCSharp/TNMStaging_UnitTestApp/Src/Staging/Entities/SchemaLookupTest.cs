using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging;


namespace TNMStaging_UnitTestApp.Src.Staging.Entities
{
    [TestClass]
    public class SchemaLookupTest
    {
        [TestMethod]
        public void testConstructorMissingValues()
        {
            Assert.IsTrue((new SchemaLookup().getKeys()).SetEquals(new HashSet<String>()));
            Assert.IsTrue((new SchemaLookup(null, null).getKeys()).SetEquals(new HashSet<String>()));
            Assert.IsTrue((new SchemaLookup("", null).getKeys()).SetEquals(new HashSet<String>()));
            Assert.IsTrue((new SchemaLookup(null, "").getKeys()).SetEquals(new HashSet<String>()));
            Assert.IsTrue((new SchemaLookup("", "").getKeys()).SetEquals(new HashSet<String>()));

            Assert.IsTrue((new SchemaLookup("C629", null).getKeys()).SetEquals(new HashSet<String>() { "site"}));
            Assert.IsTrue((new SchemaLookup("C629", "").getKeys()).SetEquals(new HashSet<String>() { "site" }));
            Assert.IsTrue((new SchemaLookup(null, "9100").getKeys()).SetEquals(new HashSet<String>() { "hist" }));
            Assert.IsTrue((new SchemaLookup("", "9100").getKeys()).SetEquals(new HashSet<String>() { "hist" }));
            Assert.IsTrue((new SchemaLookup("C629", "9100").getKeys()).SetEquals(new HashSet<String>() { "site", "hist" }));

        }

    }
}
