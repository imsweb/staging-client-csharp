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
    public class StagingRangeTest
    {
        [TestMethod]
        public void testRanges()
        {
            Assert.IsFalse(new StagingRange("100", "103").contains("099", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingRange("100", "103").contains("100", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingRange("100", "103").contains("102", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingRange("100", "103").contains("103", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("100", "103").contains("104", new Dictionary<String, String>()));

            // test that if the value is a shorter length it is not found to be a match
            Assert.IsFalse(new StagingRange("020500", "999999").contains("1", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testContext()
        {
            Dictionary<String, String> context = new Dictionary<String, String>();
            StagingRange range = new StagingRange("2000", "{{current_year}}");

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
            Assert.IsFalse(new StagingRange(null, null).contains("099", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testEmptyBothValues()
        {
            Assert.IsFalse(new StagingRange("", "").contains("099", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testEmptyOneValues()
        {
            Assert.IsFalse(new StagingRange("999", "").contains("999", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("", "999").contains("999", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testDifferentLength()
        {
            // string ranges must be the same length
            Assert.IsFalse(new StagingRange("AA", "AAA").contains("AAA", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("BBB", "BB").contains("BBB", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("CCC", "CC").contains("CC", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("DD", "DDD").contains("DD", new Dictionary<String, String>()));

            // numeric ranges do not have to be the same length
            Assert.IsTrue(new StagingRange("99", "999").contains("150", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("999", "99").contains("150", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testNumericRanges()
        {
            Assert.IsFalse(new StagingRange("0.1", "99999.9").contains("0.0", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("0.1", "99999.9").contains("100000", new Dictionary<String, String>()));
            Assert.IsFalse(new StagingRange("0.1", "99999.9").contains("100000.1", new Dictionary<String, String>()));

            Assert.IsTrue(new StagingRange("0.1", "99999.9").contains("0.1", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingRange("0.1", "99999.9").contains("500.1", new Dictionary<String, String>()));
            Assert.IsTrue(new StagingRange("0.1", "99999.9").contains("99999.9", new Dictionary<String, String>()));

            // nothing checks that a decimal is there.  Non-decimal value will still be considered in the range.
            Assert.IsTrue(new StagingRange("0.1", "99999.9").contains("1000", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testIsNumeric()
        {
            Assert.IsTrue(StagingRange.isNumeric("0"));
            Assert.IsTrue(StagingRange.isNumeric("1"));
            Assert.IsTrue(StagingRange.isNumeric("-1"));
            Assert.IsTrue(StagingRange.isNumeric("1.1"));

            Assert.IsFalse(StagingRange.isNumeric(null));
            Assert.IsFalse(StagingRange.isNumeric(""));
            Assert.IsFalse(StagingRange.isNumeric("1.1.1"));
            Assert.IsFalse(StagingRange.isNumeric("NAN"));
        }


    }

}


