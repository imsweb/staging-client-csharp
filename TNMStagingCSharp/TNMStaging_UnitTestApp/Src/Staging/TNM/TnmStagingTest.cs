using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.TNM;
using TNMStagingCSharp.Src.Staging.CS;


namespace TNMStaging_UnitTestApp.Src.Staging.TNM
{
    [TestClass]
    public class TnmStagingTest : StagingTest
    {
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(TnmDataProvider.getInstance(TnmVersion.LATEST));

            /*
            String filename = "TNM_14.zip";
            FileStream SourceStream = File.Open(filename, FileMode.Open);

            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);
            */
        }

        public override String getAlgorithm()
        {
            return "tnm";
        }


        public override String getVersion()
        {
            return TnmVersion.LATEST.getVersion();
        }

        public override StagingFileDataProvider getProvider()
        {
            return TnmDataProvider.getInstance(TnmVersion.LATEST);
        }

        [TestMethod]
        public void testBasicInitialization()
        {
            Assert.AreEqual(153, _STAGING.getSchemaIds().Count);
            Assert.IsTrue(_STAGING.getTableIds().Count > 0);

            Assert.IsNotNull(_STAGING.getSchema("urethra"));
            Assert.IsNotNull(_STAGING.getTable("ssf4_mpn"));
        }

        [TestMethod]
        public void testVersionInitializationTypes()
        {
            TNMStagingCSharp.Src.Staging.Staging staging10 = TNMStagingCSharp.Src.Staging.Staging.getInstance(TnmDataProvider.getInstance(TnmVersion.LATEST));
            Assert.AreEqual(TnmVersion.LATEST.getVersion(), staging10.getVersion());

            TNMStagingCSharp.Src.Staging.Staging stagingLatest = TNMStagingCSharp.Src.Staging.Staging.getInstance(TnmDataProvider.getInstance());
            Assert.AreEqual(TnmVersion.LATEST.getVersion(), stagingLatest.getVersion());
        }

        [TestMethod]
        public void testDescriminatorKeys()
        {
            HashSet<String> hash1 = new HashSet<String>() { "ssf25" };
            HashSet<String> hash2 = _STAGING.getSchema("nasopharynx").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "sex" };
            hash2 = _STAGING.getSchema("peritoneum_female_gen").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));
        }

        [TestMethod]
        public void testSchemaSelection() 
        {
            // test bad values
            List<StagingSchema> lookup = _STAGING.lookupSchema(new SchemaLookup());
            Assert.AreEqual(0, lookup.Count);

            lookup = _STAGING.lookupSchema(new TnmSchemaLookup("XXX", "YYY"));
            Assert.AreEqual(0, lookup.Count);

            // test valid combinations that do not require a discriminator
            TnmSchemaLookup schemaLookup = new TnmSchemaLookup("C629", "9231");
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, null);
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            // now test one that does do AJCC7
            lookup = _STAGING.lookupSchema(new TnmSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);

            // test value combinations that do not require a discriminator and are supplied 988
            schemaLookup = new TnmSchemaLookup("C629", "9231");
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "988");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            // test valid combination that requires a discriminator but is not supplied one
            lookup = _STAGING.lookupSchema(new TnmSchemaLookup("C111", "8200"));
            Assert.AreEqual(2, lookup.Count);

            HashSet<String> hash1 = null;
            HashSet<String> hash2 = null;
            foreach (StagingSchema schema in lookup)
            {
                hash1 = new HashSet<String>() { "ssf25" };
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));
            }

            // test valid combination that requires discriminator and a good discriminator is supplied
            schemaLookup = new TnmSchemaLookup("C111", "8200");
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "010");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            foreach (StagingSchema schema in lookup)
            {
                hash1 = new HashSet<String>() { "ssf25" };
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));
            }
            Assert.AreEqual("nasopharynx", lookup[0].getId());

            // test valid combination that requires a discriminator but is supplied a bad disciminator value
            schemaLookup = new TnmSchemaLookup("C111", "8200");
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "999");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);

            // test searching on only site
            lookup = _STAGING.lookupSchema(new TnmSchemaLookup("C401", null));
            Assert.AreEqual(5, lookup.Count);

            // test searching on only hist
            lookup = _STAGING.lookupSchema(new TnmSchemaLookup(null, "9702"));
            Assert.AreEqual(2, lookup.Count);

            // test that searching on only ssf25 returns no results
            schemaLookup = new TnmSchemaLookup(null, null);
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "001");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);
            schemaLookup = new TnmSchemaLookup("", null);
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "001");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);
            schemaLookup = new TnmSchemaLookup(null, "");
            schemaLookup.setInput(TnmStagingData.SSF25_KEY, "001");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);
        }

        [TestMethod]
        public void testLookupCache()
        {
            // do the same lookup twice
            List<StagingSchema> lookup = _STAGING.lookupSchema(new TnmSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            lookup = _STAGING.lookupSchema(new TnmSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());

            // now invalidate the cache
            TnmDataProvider.getInstance(TnmVersion.LATEST).invalidateCache();

            // try the lookup again
            lookup = _STAGING.lookupSchema(new TnmSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("testis", lookup[0].getId());
        }

        [TestMethod]
        public void testFindTableRow()
        {
            Assert.AreEqual(-1, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "cZ"));
            Assert.AreEqual(-1, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "c9"));

            // null maps to blank
            Assert.AreEqual(7, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", null));

            Assert.AreEqual(0, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "cX"));
            Assert.AreEqual(1, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "c0"));
            Assert.AreEqual(2, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "c1"));
            Assert.AreEqual(3, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "c2"));
            Assert.AreEqual(4, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "c3"));
            Assert.AreEqual(5, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", "clin_t", "c4"));

            Dictionary<String, String> context = new Dictionary<String, String>();
            context["clin_t"] = "c4";
            Assert.AreEqual(5, _STAGING.findMatchingTableRow("adrenal_gland_t_18187", context));

            // test a table that has multiple inputs
            context = new Dictionary<String, String>();
            context["m_prefix"] = "p";
            context["root_m"] = "0";
            Assert.AreEqual(8, _STAGING.findMatchingTableRow("concatenate_m_40642", context));
        }

        [TestMethod]
        public void testInputBuilder()
        {
            TnmStagingData data1 = new TnmStagingData();
            data1.setInput(TnmInput.PRIMARY_SITE, "C680");
            data1.setInput(TnmInput.HISTOLOGY, "8000");
            data1.setInput(TnmInput.BEHAVIOR, "3");
            data1.setInput(TnmInput.GRADE, "9");
            data1.setInput(TnmInput.DX_YEAR, "2013");
            data1.setInput(TnmInput.REGIONAL_NODES_POSITIVE, "99");
            data1.setInput(TnmInput.AGE_AT_DX, "060");
            data1.setInput(TnmInput.SEX, "1");
            data1.setInput(TnmInput.RX_SUMM_SURGERY, "8");
            data1.setInput(TnmInput.RX_SUMM_RADIATION, "9");
            data1.setInput(TnmInput.CLIN_T, "1");
            data1.setInput(TnmInput.CLIN_N, "2");
            data1.setInput(TnmInput.CLIN_M, "3");
            data1.setInput(TnmInput.PATH_T, "4");
            data1.setInput(TnmInput.PATH_N, "5");
            data1.setInput(TnmInput.PATH_M, "6");
            data1.setSsf(1, "020");

            TnmStagingData data2 = new TnmStagingData.TnmStagingInputBuilder()
                .withInput(TnmInput.PRIMARY_SITE, "C680")
                .withInput(TnmInput.HISTOLOGY, "8000")
                .withInput(TnmInput.BEHAVIOR, "3")
                .withInput(TnmInput.GRADE, "9")
                .withInput(TnmInput.DX_YEAR, "2013")
                .withInput(TnmInput.REGIONAL_NODES_POSITIVE, "99")
                .withInput(TnmInput.AGE_AT_DX, "060")
                .withInput(TnmInput.SEX, "1")
                .withInput(TnmInput.RX_SUMM_SURGERY, "8")
                .withInput(TnmInput.RX_SUMM_RADIATION, "9")
                .withInput(TnmInput.CLIN_T, "1")
                .withInput(TnmInput.CLIN_N, "2")
                .withInput(TnmInput.CLIN_M, "3")
                .withInput(TnmInput.PATH_T, "4")
                .withInput(TnmInput.PATH_N, "5")
                .withInput(TnmInput.PATH_M, "6")
                .withSsf(1, "020").build();

            Assert.IsTrue(TNMStaging_UnitTestApp.Src.Staging.ComparisonUtils.CompareStringDictionaries(data1.getInput(), data2.getInput()));
        }


        [TestMethod]
        public void testStageUrethra()
        {
            TnmStagingData data = new TnmStagingData();
            data.setInput(TnmInput.PRIMARY_SITE, "C680");
            data.setInput(TnmInput.HISTOLOGY, "8000");
            data.setInput(TnmInput.BEHAVIOR, "3");
            data.setInput(TnmInput.DX_YEAR, "2016");
            data.setInput(TnmInput.RX_SUMM_SURGERY, "2");
            data.setInput(TnmInput.RX_SUMM_RADIATION, "4");
            data.setInput(TnmInput.REGIONAL_NODES_POSITIVE, "02");
            data.setInput(TnmInput.CLIN_T, "c0");
            data.setInput(TnmInput.CLIN_N, "c1");
            data.setInput(TnmInput.CLIN_M, "c0");
            data.setInput(TnmInput.PATH_T, "p0");
            data.setInput(TnmInput.PATH_N, "p1");
            data.setInput(TnmInput.PATH_M, "p1");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("urethra", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(25, data.getPath().Count);
            Assert.AreEqual(10, data.getOutput().Count);

            // check outputs
            Assert.AreEqual(TnmVersion.LATEST.getVersion(), data.getOutput(TnmOutput.DERIVED_VERSION));
            Assert.AreEqual("3", data.getOutput(TnmOutput.CLIN_STAGE_GROUP));
            Assert.AreEqual("4", data.getOutput(TnmOutput.PATH_STAGE_GROUP));
            Assert.AreEqual("4", data.getOutput(TnmOutput.COMBINED_STAGE_GROUP));
            Assert.AreEqual("c0", data.getOutput(TnmOutput.COMBINED_T));
            Assert.AreEqual("1", data.getOutput(TnmOutput.SOURCE_T));
            Assert.AreEqual("c1", data.getOutput(TnmOutput.COMBINED_N));
            Assert.AreEqual("1", data.getOutput(TnmOutput.SOURCE_N));
            Assert.AreEqual("p1", data.getOutput(TnmOutput.COMBINED_M));
            Assert.AreEqual("2", data.getOutput(TnmOutput.SOURCE_M));
        }

        [TestMethod]
        public void testBadLookupInStage()
        {
            TnmStagingData data = new TnmStagingData();

            // if site/hist are not supplied, no lookup
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY, data.getResult());

            // add hist only and it should fail with same result
            data.setInput(TnmInput.PRIMARY_SITE, "C489");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY, data.getResult());

            // put a site/hist combo that doesn't match a schema
            data.setInput(TnmInput.HISTOLOGY, "9898");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_NO_MATCHING_SCHEMA, data.getResult());

            // now a site/hist that returns multiple results
            data.setInput(TnmInput.PRIMARY_SITE, "C111");
            data.setInput(TnmInput.HISTOLOGY, "8200");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS, data.getResult());
        }

        [TestMethod]
        public void testInvolvedTables()
        {
            HashSet<String> tables = _STAGING.getInvolvedTables("adnexa_uterine_other");
            HashSet<String> hash1 = new HashSet<String>()
                { "primary_site", "histology", "schema_selection_adnexa_uterine_other", "year_dx_validation" };
            Assert.IsTrue(tables.SetEquals(hash1));
        }

        [TestMethod]
        public void testInvolvedSchemas()
        {
            HashSet<String> schemas = _STAGING.getInvolvedSchemas("ssf1_jpd");
            HashSet<String> hash1 = new HashSet<String>() { "kidney_renal_pelvis", "bladder", "urethra" };
            Assert.IsTrue(schemas.SetEquals(hash1));
        }

        [TestMethod]
        public void testGetInputs()
        {
            HashSet<String> test1 = new HashSet<String>() { "site", "hist" };
            HashSet<String> test2 = _STAGING.getInputs(_STAGING.getSchema("adnexa_uterine_other"));
            Assert.IsTrue(test1.SetEquals(test2));

            test1 = new HashSet<String>() {"site", "hist", "behavior", "systemic_surg_seq", "radiation_surg_seq", "nodes_pos", "clin_t", "clin_n", "clin_m",
                            "path_t", "path_n", "path_m", "ssf13", "ssf15", "ssf16", "clin_stage_group_direct",
                            "path_stage_group_direct" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("testis"));
            Assert.IsTrue(test1.SetEquals(test2));

            // test with and without context
            test1 = new HashSet<String>() {"site", "hist", "systemic_surg_seq", "radiation_surg_seq", "nodes_pos", "clin_t", "clin_n", "clin_m", "path_t",
                            "path_n", "path_m", "ssf1", "ssf8", "ssf10", "clin_stage_group_direct",
                            "path_stage_group_direct" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("prostate"));
            Assert.IsTrue(test1.SetEquals(test2));


            Dictionary<String, String> context = new Dictionary<String, String>();
            context[StagingData.PRIMARY_SITE_KEY] = "C619";
            context[StagingData.HISTOLOGY_KEY] = "8120";
            context[StagingData.YEAR_DX_KEY] = "2004";

            // for that context, only summary stage is calculated
            test1 = new HashSet<String>() { "site", "hist"};
            test2 = _STAGING.getInputs(_STAGING.getSchema("prostate"), context);
            Assert.IsTrue(test1.SetEquals(test2));
        }


        [TestMethod]
        public void testIsCodeValid()
        {
            // test bad parameters for schema or field
            Assert.IsFalse(_STAGING.isCodeValid("bad_schema_name", "site", "C509"));
            Assert.IsFalse(_STAGING.isCodeValid("testis", "bad_field_name", "C509"));

            // test null values
            Assert.IsFalse(_STAGING.isCodeValid(null, null, null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", null, null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "site", null));

            // test fields that have a "value" specified
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "200"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "2003"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "2145"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "year_dx", "2016"));

            // test valid and invalid fields
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "clin_t", "c4"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "clin_t", "c5"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "ssf1", "020"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "ssf1", "030"));
        }

        [TestMethod]
        public void testIsContextValid()
        {
            TnmStagingData data = new TnmStagingData();

            data.setInput(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT, "2016");

            // test valid year
            data.setInput(TnmInput.DX_YEAR, "2016");
            Assert.IsTrue(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test invalid year
            data.setInput(TnmInput.DX_YEAR, "2014");
            Assert.IsFalse(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));
        }

        [TestMethod]
        public void testGetSchemaIds()
        {
            HashSet<String> algorithms = _STAGING.getSchemaIds();

            Assert.IsTrue(algorithms.Count > 0);
            Assert.IsTrue(algorithms.Contains("testis"));
        }

        [TestMethod]
        public void testGetTableIds()
        {
            HashSet<String> tables = _STAGING.getTableIds();

            Assert.IsTrue(tables.Count > 0);
            Assert.IsTrue(tables.Contains("determine_default_n"));
        }

        [TestMethod]
        public void testGetSchema()
        {
            Assert.IsNull(_STAGING.getSchema("bad_schema_name"));
            Assert.IsNotNull(_STAGING.getSchema("brain"));
            Assert.AreEqual("Brain", _STAGING.getSchema("brain").getName());
        }

        [TestMethod]
        public void testLookupInputs()
        {
            // test valid combinations that do not require a discriminator
            StagingSchema schema = _STAGING.getSchema("prostate");
            TnmSchemaLookup lookup = new TnmSchemaLookup("C619", "8000");
            Assert.IsTrue(_STAGING.getInputs(schema, lookup.getInputs()).Contains("clin_t"));

            lookup = new TnmSchemaLookup("C619", "8120");
            Assert.IsFalse(_STAGING.getInputs(schema, lookup.getInputs()).Contains("clin_t"));
        }

        [TestMethod]
        public void testLookupOutputs()
        {
            TnmSchemaLookup lookup = new TnmSchemaLookup("C680", "8590");
            List<StagingSchema> lookups = _STAGING.lookupSchema(lookup);
            Assert.AreEqual(1, lookups.Count);

            StagingSchema schema = _STAGING.getSchema(lookups[0].getId());
            Assert.AreEqual("urethra", schema.getId());

            // build list of output keys
            List<StagingSchemaOutput> outputs = schema.getOutputs();
            HashSet<String> definedOutputs = new HashSet<String>();
            foreach (StagingSchemaOutput o in outputs)
            {
                definedOutputs.Add(o.getKey());
            }

            // test without context
            Assert.IsTrue(definedOutputs.SetEquals(_STAGING.getOutputs(schema)));

            // test with context
            Assert.IsTrue(definedOutputs.SetEquals(_STAGING.getOutputs(schema, lookup.getInputs())));
        }

        [TestMethod]
        public void testRangeParsing()
        {
            StagingTable table = _STAGING.getTable("path_n_daj");

            Assert.IsNotNull(table);
            Assert.AreEqual("p0I-", table.getRawRows()[2][0]);

            StagingTableRow tablerow = (table.getTableRows()[2] as StagingTableRow);
            Assert.AreEqual("p0I-", tablerow.getInputs()["path_n"][0].getLow());

            tablerow = (table.getTableRows()[2] as StagingTableRow);
            Assert.AreEqual("p0I-", tablerow.getInputs()["path_n"][0].getHigh());

        }

        [TestMethod]
        public void testEncoding() 
        {
            StagingTable table = _STAGING.getTable("thyroid_t_6166");

            Assert.IsNotNull(table);

            // the notes of this table contain UTF-8 characters, specifically the single quote character in this phrase: "You may use a physicianâ€™s statement"

            // converting to UTF-8 should change nothing

            String OrigNotes = table.getNotes();
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(OrigNotes);
            char[] utf8Chars = new char[Encoding.UTF8.GetCharCount(utf8Bytes, 0, utf8Bytes.Length)];
            Encoding.UTF8.GetChars(utf8Bytes, 0, utf8Bytes.Length, utf8Chars, 0);
            string utf8String = new string(utf8Chars);
            Assert.AreEqual(table.getNotes(), utf8String);


            // converting to other encoding should change the text
            byte[] asciiBytes = Encoding.ASCII.GetBytes(OrigNotes);
            char[] asciiChars = new char[Encoding.ASCII.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            Encoding.ASCII.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);
            Assert.AreNotEqual(table.getNotes(), asciiString);

        }

        [TestMethod]
        public void testSchemaSelectionIntegration() 
        {
            if (DebugSettings.RUN_LARGE_TNM_TESTS)
            {
                /*
                // test complete file of cases
                String sFilePath = "cs_schema_identification_unit_test.txt.gz";

                FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);

                TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult result =
                        TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.processSchemaSelection(_STAGING, sFilePath, decompressionStream);

                Assert.AreEqual(0, result.getNumFailures());
                */

            }
        }

        [TestMethod]
        public void testExpectedOutput() 
        {
            if (DebugSettings.RUN_LARGE_TNM_TESTS)
            {
                String basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
                if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) basedir += "\\..\\";

                String sFilePath = basedir + "Resources\\Test\\TNM\\TNM_13.zip";

                FileStream SourceStream = File.Open(sFilePath, FileMode.Open);
                ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);
                TNMStagingCSharp.Src.Staging.Staging TNM13_STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);

                sFilePath = basedir + "Resources\\Test\\TNM\\TNM_V13_StagingTestLarge.txt.gz";

                FileStream fstream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                GZipStream decompressionStream = new GZipStream(fstream, CompressionMode.Decompress);

                TNMStagingCSharp.Src.Staging.Staging OLD_STAGING = _STAGING;
                _STAGING = TNM13_STAGING;

                TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.IntegrationResult TNMResult =
                        TNMStaging_UnitTestApp.Src.Staging.TNM.TnmIntegrationSchemaStage.processTNMSchema(_STAGING, sFilePath, decompressionStream, true);

                fstream.Close();
                decompressionStream.Close();

                _STAGING = OLD_STAGING;

                // make sure there were no errors returned
                Assert.AreEqual(0, TNMResult.getNumFailures(), "There were failures in the TNMResult Staging tests.");
            }
        }


    }
}


