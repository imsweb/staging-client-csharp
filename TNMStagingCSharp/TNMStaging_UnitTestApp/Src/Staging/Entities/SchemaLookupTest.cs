using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TNMStagingCSharp.Src.Staging.Entities;

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

        [TestMethod]
        void testSiteAndHistologyMutation()
        {
            SchemaLookup lookup = new SchemaLookup("C629", "9100");

            lookup.setSite("C509");
            lookup.setHistology("8000");

            Assert.AreEqual("C509", lookup.getSite());
            Assert.AreEqual("C509", lookup.getInput(StagingData.PRIMARY_SITE_KEY));
            Assert.AreEqual("8000", lookup.getHistology());
            Assert.AreEqual("8000", lookup.getInput(StagingData.HISTOLOGY_KEY));
        }

        [TestMethod]
        void testEqualsAndHashCode()
        {
            SchemaLookup lookup1 = new SchemaLookup("C629", "9100");
            SchemaLookup lookup2 = new SchemaLookup("C629", "9100");
            SchemaLookup lookup3 = new SchemaLookup("C629", "9100");

            Assert.AreEqual(lookup1, lookup1);
            Assert.AreEqual(lookup1, lookup2);
            Assert.AreEqual(lookup2, lookup1);
            Assert.AreEqual(lookup2, lookup3);
            Assert.AreEqual(lookup1, lookup3);
            Assert.AreEqual(lookup1.GetHashString(), lookup2.GetHashString());
        }

        [TestMethod]
        void testNotEquals()
        {
            SchemaLookup lookup = new SchemaLookup("C629", "9100");

            Assert.AreNotEqual(null, lookup);
            Assert.AreNotEqual("C629", lookup);
            Assert.AreNotEqual(lookup, new TestSchemaLookup("C629", "9100"));
            Assert.AreNotEqual(new SchemaLookup("C509", "9100"), lookup);
            Assert.AreNotEqual(new SchemaLookup("C629", "8000"), lookup);
        }

        [TestMethod]
        void testClearInputs()
        {
            TestSchemaLookup lookup = new TestSchemaLookup("C629", "9100");
            lookup.setInput("allowed", "value");

            lookup.clear();

            Assert.IsTrue(lookup.getInputs().Count == 0);
        }

        [TestMethod]
        void testAllowedKeys()
        {
            SchemaLookup unrestrictedLookup = new SchemaLookup();
            unrestrictedLookup.setInput("anything", "value");
            Assert.AreEqual("value", unrestrictedLookup.getInput("anything"));

            TestSchemaLookup restrictedLookup = new TestSchemaLookup();
            restrictedLookup.setInput("allowed", "value");
            Assert.AreEqual("value", restrictedLookup.getInput("allowed"));
            bool exceptionThrown = false;
            try
            {
                restrictedLookup.setInput("disallowed", "value");
            }
            catch (System.InvalidOperationException ex)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        void testDiscriminator()
        {
            SchemaLookup lookup = new SchemaLookup("C629", "9100");
            Assert.IsFalse(lookup.hasDiscriminator());

            lookup.setInput("discriminator", null);
            Assert.IsFalse(lookup.hasDiscriminator());

            lookup.setInput("discriminator", "");
            Assert.IsFalse(lookup.hasDiscriminator());

            lookup.setInput("discriminator", "001");
            Assert.IsTrue(lookup.hasDiscriminator());
        }

        public class TestSchemaLookup : SchemaLookup
        {
            public TestSchemaLookup() 
            { 
            }

            public TestSchemaLookup(String site, String histology) : base(site, histology)
            {
            }

            public override HashSet<String> getAllowedKeys()
            {
                HashSet<String> retval = new HashSet<String>();
                retval.Add(StagingData.PRIMARY_SITE_KEY);
                retval.Add(StagingData.HISTOLOGY_KEY);
                retval.Add("allowed");
                return retval;
            }

            public void clear()
            {
                clearInputs();
            }
        }
    }
}
