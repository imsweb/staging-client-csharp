/*
 * Copyright (C) 2022 Information Management Services, Inc.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;


namespace TNMStaging_UnitTestApp.Src.Staging.Entities
{
    [TestClass]
    public class GlossaryDefinitionTest
    {
        [TestMethod]
        public void testConstruction()
        {
            DateTime now = new DateTime();

            GlossaryDefinition def = new GlossaryDefinition();
            def.setName("NAME");
            def.setDefinition("DEF");
            List<string> list = new List<string>();
            list.Add("ALT");
            def.setAlternateNames(list);
            def.setLastModified(now);

            List<string> testList = new List<string>();
            testList.Add("ALT");
            GlossaryDefinition testGlossary = new GlossaryDefinition("NAME", "DEF", testList, now);

            Assert.IsTrue(def.Equals(testGlossary));
        }
    }
}

