using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.DecisionEngine;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.TNM;
using TNMStagingCSharp.Src.Staging.CS;
using TNMStagingCSharp.Src.Staging.Entities;


namespace TNMStaging_UnitTestApp.Src
{
    [TestClass]
    public class StagingStringRangeTest
    {
        [TestMethod]
        public void testRanges()
        {
            Assert.IsFalse(new StagingStringRange("100", "103").contains("099", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingStringRange("100", "103").contains("100", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingStringRange("100", "103").contains("102", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingStringRange("100", "103").contains("103", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingStringRange("100", "103").contains("104", new Dictionary<String, String>()));

            // test that if the value is a shorter length it is not found to be a match
            Assert.IsFalse(new StagingStringRange("020500", "999999").contains("1", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testContext()
        {
            Dictionary<String, String> context = new Dictionary<String, String>();
            StagingStringRange range = new StagingStringRange("2000", "{{current_year}}");

            Assert.IsFalse(range.contains("2004", context));

            context["current_year"] = "X";
            Assert.IsFalse(range.contains("2004", context));

            context["current_year"] = "";
            Assert.IsFalse(range.contains("2004", context));

            // this is a tricky one; the end result is 2000-XXXX, which 2004 is in the range; it's odd when one side is not a number
            // but since we are doing string ranges, this will match
            context["current_year"] = "XXXX";
            Assert.IsTrue(range.contains("2004", context));

            context["current_year"] = "1990";
            Assert.IsFalse(range.contains("2004", context));

            context["current_year"] = "2015";
            Assert.IsFalse(range.contains("2020", context));
            Assert.IsTrue(range.contains("2004", context));
        }

        [TestMethod] //(expected = IllegalStateException.class)
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testNull()
        {
            Assert.IsFalse(new StagingStringRange(null, null).contains("099", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testEmptyBothValues()
        {
            Assert.IsFalse(new StagingStringRange("", "").contains("099", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testEmptyOneValues()
        {
            Assert.IsFalse(new StagingStringRange("999", "").contains("999", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingStringRange("", "999").contains("999", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testDifferentLength()
        {
            Assert.IsFalse(new StagingStringRange("99", "999").contains("099", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingStringRange("999", "99").contains("099", new Dictionary<String, String>()));

            Assert.IsFalse(new StagingStringRange("999", "99").contains("99", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingStringRange("99", "999").contains("99", new Dictionary<String, String>()));
        }
    }

}


