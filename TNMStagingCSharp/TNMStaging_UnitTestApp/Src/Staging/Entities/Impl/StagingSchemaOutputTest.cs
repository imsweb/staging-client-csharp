// Copyright (C) 2021 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging.Entities;


namespace TNMStaging_UnitTestApp.Src.Staging.Entities.Impl
{
    [TestClass]
    public class StagingSchemaOutputTest
    {
        [TestMethod]
        public void testEquals()
        {
            StagingSchemaOutput output1 = new StagingSchemaOutput();
            StagingSchemaOutput output2 = new StagingSchemaOutput();

            Assert.IsTrue(output1.Equals(output2));

            output1.setKey("key");
            output1.setName("name");
            output1.setTable("table");
            output1.setNaaccrXmlId("test");
            output1.setMetadata(new HashSet<String>() { "META1" });

            output2 = new StagingSchemaOutput("key", "name", "table");
            output2.setNaaccrXmlId("test");
            output2.setMetadata(new HashSet<String>() { "META1" });

            Assert.IsTrue(output1.Equals(output2));

            output2.setMetadata(new HashSet<String>() { "META2" });

            Assert.IsFalse(output1.Equals(output2));

            // test copy constructor
            StagingSchemaOutput output3 = new StagingSchemaOutput(output1);
            Assert.IsTrue(output1.Equals(output3));
        }
    }
}

