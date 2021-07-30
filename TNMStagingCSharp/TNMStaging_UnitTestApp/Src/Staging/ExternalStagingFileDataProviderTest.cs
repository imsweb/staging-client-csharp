using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;

namespace TNMStaging_UnitTestApp.Src
{
    [TestClass]
    public class ExternalStagingFileDataProviderTest
    {
        private static TNMStagingCSharp.Src.Staging.Staging _STAGING;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            String basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
            if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) basedir += "\\..\\";

            String sFilePath = basedir + "Resources\\Test\\Misc\\external_algorithm.zip";

            FileStream SourceStream = File.Open(sFilePath, FileMode.Open);

            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);
        }

        [TestMethod]
        public void testExternalLoad()
        {
            Assert.AreEqual("testing", _STAGING.getAlgorithm());
            Assert.AreEqual("99.99", _STAGING.getVersion());
            Assert.AreEqual(1, _STAGING.getSchemaIds().Count);
            Assert.AreEqual(62, _STAGING.getTableIds().Count);

            Schema schema = _STAGING.getSchema("urethra");
            Assert.IsNotNull(schema);
            Assert.AreEqual("testing", schema.getAlgorithm());
            Assert.AreEqual("99.99", schema.getVersion());

            ITable table = _STAGING.getTable("ajcc_descriptor_codes");
            Assert.IsNotNull(table);
            Assert.AreEqual("testing", table.getAlgorithm());
            Assert.AreEqual("99.99", table.getVersion());
            Assert.AreEqual(6, table.getTableRows().Count);

            HashSet<String> involved = _STAGING.getInvolvedTables("urethra");
            Assert.AreEqual(62, involved.Count);
            Assert.IsTrue(involved.Contains("mets_eval_ipa"));

            StagingData data = new StagingData();
            data.setInput("site", "C680");
            data.setInput("hist", "8000");
            data.setInput("behavior", "3");
            data.setInput("grade", "9");
            data.setInput("year_dx", "2013");
            data.setInput("cs_input_version_original", "020550");
            data.setInput("extension", "100");
            data.setInput("extension_eval", "9");
            data.setInput("nodes", "100");
            data.setInput("nodes_eval", "9");
            data.setInput("mets", "10");
            data.setInput("mets_eval", "9");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(37, data.getPath().Count);

            // check output
            Assert.AreEqual("129", data.getOutput("schema_number"));
            Assert.AreEqual("020550", data.getOutput("csver_derived"));

            // AJCC 6
            Assert.AreEqual("70", data.getOutput("stor_ajcc6_stage"));

            // AJCC 7
            Assert.AreEqual("700", data.getOutput("stor_ajcc7_stage"));

            // Summary Stage
            Assert.AreEqual("7", data.getOutput("stor_ss77"));
            Assert.AreEqual("7", data.getOutput("stor_ss2000"));

            // test glossary items
            Assert.AreEqual(13, _STAGING.getGlossaryTerms().Count);
            GlossaryDefinition entry = _STAGING.getGlossaryDefinition("Adjacent tissue(s), NOS");
            Assert.IsNotNull(entry);
            Assert.AreEqual("Adjacent tissue(s), NOS", entry.getName());
            Assert.IsTrue(entry.getDefinition().StartsWith("The unnamed tissues that immediately surround"));
            CollectionAssert.AreEqual(new List<string>() { "Connective tissue" }, entry.getAlternateNames());
            Assert.IsNotNull(entry.getLastModified());

            List<GlossaryHit> hits = _STAGING.getGlossaryMatches("Some text and Cortex should be only match.");
            Assert.AreEqual(1, hits.Count);
            GlossaryHit hit = hits[0];
            Assert.AreEqual("cortex", hit.getTerm());
            Assert.AreEqual(14, (long)hit.getBegin());
            Assert.AreEqual(19, (long)hit.getEnd());
            hits = _STAGING.getGlossaryMatches("Cortex and stroma should be two matches.");
            Assert.AreEqual(2, hits.Count);
            hits = _STAGING.getGlossaryMatches("stromafoo not be a match since the keyword it is not a whole word");
            Assert.AreEqual(0, hits.Count);

            HashSet<String> glossary = _STAGING.getSchemaGlossary("urethra");
            Assert.AreEqual(1, glossary.Count);
            glossary = _STAGING.getTableGlossary("ssf1_jpd");
            Assert.AreEqual(1, glossary.Count);
        }

        [TestMethod]
        public void testMetadata()
        {
            // verify if reads metadata in old format (List<String>) and new format (List<Metadata>)

            Schema urethra = _STAGING.getSchema("urethra");

            Assert.IsNotNull(urethra);

            // old structure
            IInput ssf1 = urethra.getInputMap()["ssf1"];
            List<Metadata> list = ssf1.getMetadata();
            //Assert.That(list.extracting("name").containsExactlyInAnyOrder("CCCR_IVA_COLLECT_IF_AVAILABLE_IN_PATH_REPORT", "COC_CLINICALLY_SIGNIFICANT", "SEER_CLINICALLY_SIGNIFICANT");
            bool[] foundArray = new bool[3];
            for (int i = 0; i < 3; i++)
            {
                foundArray[i] = false;
            }
            bool foundOthers = false;
            foreach (Metadata md in list)
            {
                if (md.getName().Equals("CCCR_IVA_COLLECT_IF_AVAILABLE_IN_PATH_REPORT"))
                {
                    foundArray[0] = true;
                }
                else if (md.getName().Equals("COC_CLINICALLY_SIGNIFICANT"))
                {
                    foundArray[1] = true;
                }
                else if (md.getName().Equals("SEER_CLINICALLY_SIGNIFICANT"))
                {
                    foundArray[2] = true;
                }
                else
                {
                    foundOthers = true;
                }
            }
            Assert.IsTrue(foundArray[0] && foundArray[1] && foundArray[2] && !foundOthers);

            // new structure
            IInput ssf2 = urethra.getInputMap()["ssf2"];
            list = ssf2.getMetadata();
            bool found = false;
            foundOthers = false;
            foreach (Metadata md in list)
            {
                if (md.getName().Equals("UNDEFINED_SSF"))
                {
                    found = true;
                }
                else
                {
                    foundOthers = true;
                }
            }
            Assert.IsTrue(found && !foundOthers);

            IInput ssf3 = urethra.getInputMap()["ssf3"];
            Assert.AreEqual(ssf3.getMetadata().Count, 2);

            List<StagingMetadata> expected = new List<StagingMetadata>();
            expected.Add(new StagingMetadata("FIRST_ITEM", 2017, 2020));
            expected.Add(new StagingMetadata("SECOND_ITEM", 2021));
            Assert.AreEqual(ssf3.getMetadata(), expected);
        }
    }
}
