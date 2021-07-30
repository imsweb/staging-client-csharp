// Copyright (C) 2021 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging.Entities;


namespace TNMStaging_UnitTestApp.Src.Staging.Entities.Impl
{
    [TestClass]
    public class StagingSchemaInputTest
    {
        [TestMethod]
        public void testEquals()
        {
            StagingSchemaInput input1 = new StagingSchemaInput();
            StagingSchemaInput input2 = new StagingSchemaInput();

            Assert.IsTrue(input1.Equals(input2));

            input1.setKey("key");
            input1.setName("name");
            input1.setTable("table");
            input1.setMetadata(new HashSet<String>() { "META1" });

            input2 = new StagingSchemaInput("key", "name", "table");
            input2.setMetadata(new HashSet<String>() { "META1" });

            Assert.IsTrue(input1.Equals(input2));

            input2.setMetadata(new HashSet<String>() { "META2" });

            Assert.IsFalse(input1.Equals(input2));

            // test copy constructor
            StagingSchemaInput input3 = new StagingSchemaInput(input1);
            Assert.IsTrue(input1.Equals(input3));
        }
    }
}


