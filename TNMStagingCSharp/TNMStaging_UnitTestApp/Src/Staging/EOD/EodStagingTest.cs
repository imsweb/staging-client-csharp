﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.EOD;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;

namespace TNMStaging_UnitTestApp.Src.Staging.EOD
{
    [TestClass]
    public class EodStagingTest : StagingTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///  Gets or sets the test context which provides
        ///  information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        public override string getAlgorithm()
        {
            return "eod_public";
        }

        public override string getVersion()
        {
            return EodVersion.V3_2.getVersion();
        }

        public override StagingFileDataProvider getProvider()
        {
            return EodDataProvider.getInstance(EodVersion.LATEST);
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(EodDataProvider.getInstance(EodVersion.LATEST));

            /*
            String filename = "CS_02_05_50.zip";
            FileStream SourceStream = File.Open(filename, FileMode.Open);

            ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);
            */
        }

        [TestMethod]
        public void testBasicInitialization()
        {
            Assert.AreEqual(_STAGING.getSchemaIds().Count, 139);
            Assert.IsTrue(_STAGING.getTableIds().Count > 0);

            Assert.IsNotNull(_STAGING.getSchema("urethra"));
            Assert.IsNotNull(_STAGING.getTable("ss2018_urethra_14363"));
        }

        [TestMethod]
        public void testVersionInitializationTypes()
        {
            TNMStagingCSharp.Src.Staging.Staging staging10 = TNMStagingCSharp.Src.Staging.Staging.getInstance(EodDataProvider.getInstance(EodVersion.LATEST));
            Assert.AreEqual(EodVersion.LATEST.getVersion(), staging10.getVersion());

            TNMStagingCSharp.Src.Staging.Staging stagingLatest = TNMStagingCSharp.Src.Staging.Staging.getInstance(EodDataProvider.getInstance());
            Assert.AreEqual(EodVersion.LATEST.getVersion(), stagingLatest.getVersion());
        }

        [TestMethod]
        public void testDescriminatorKeys()
        {
            HashSet<String> hash1 = new HashSet<String>() { "discriminator_1", "year_dx" };
            HashSet<String> hash2 = _STAGING.getSchema("nasopharynx").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "discriminator_1", "discriminator_2" };
            hash2 = _STAGING.getSchema("oropharynx_p16_neg").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));
        }

        [TestMethod]
        public void testSchemaSelection()
        {
            // test bad values
            List<Schema> lookup = _STAGING.lookupSchema(new SchemaLookup());
            Assert.AreEqual(0, lookup.Count);

            lookup = _STAGING.lookupSchema(new EodSchemaLookup("XXX", "YYY"));
            Assert.AreEqual(0, lookup.Count);

            // test valid combinations that do not require a discriminator
            EodSchemaLookup schemaLookup = new EodSchemaLookup("C629", "9231");
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("soft_tissue_rare", lookup[0].getId());
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), null);
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("soft_tissue_rare", lookup[0].getId());

            // test valid combination that requires a discriminator but is not supplied one
            lookup = _STAGING.lookupSchema(new EodSchemaLookup("C111", "8200"));
            Assert.AreEqual(4, lookup.Count);

            HashSet<String> hash1 = new HashSet<String>() {
                    "oropharynx_p16_neg",
                    "nasopharynx",
                    "nasopharynx_v9_2025",
                    "oropharynx_hpv_mediated_p16_pos" };
            HashSet<String> hash2 = new HashSet<String>();
            foreach (Schema schema in lookup)
            {
                hash2.Add(schema.getId());
            }
            Assert.IsTrue(hash1.SetEquals(hash2));

            //assertThat(lookup.stream().flatMap(d->d.getSchemaDiscriminators().stream()).collect(Collectors.toSet())).isEqualTo(new HashSet<>(Arrays.asList("year_dx", "discriminator_1", "discriminator_2")));
            hash1 = new HashSet<String>() {
                    "year_dx",
                    "discriminator_1",
                    "discriminator_2" };
            hash2 = new HashSet<String>();
            foreach (Schema schema in lookup)
            {
                hash2.UnionWith(schema.getSchemaDiscriminators());
            }
            Assert.IsTrue(hash1.SetEquals(hash2));

            // test valid combination that requires discriminator and a good discriminator is supplied
            schemaLookup = new EodSchemaLookup("C111", "8200");
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "1");
            schemaLookup.setInput(EodInput.DX_YEAR.toString(), "2022");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            foreach (Schema schema in lookup)
            {
                hash1 = new HashSet<String>() { "discriminator_1", "year_dx" };
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));

                /*
                System.Diagnostics.Trace.WriteLine("==========================================================================");
                String s = schema.GetDebugString("Test Schema ");
                System.Diagnostics.Trace.WriteLine(s);

                HashSet<String> tables = schema.getInvolvedTables();
                StagingTable thisTable;
                foreach (String tableName in tables)
                {
                    System.Diagnostics.Trace.WriteLine("---------------------------------------------------------------------------------");
                    thisTable = _STAGING.getTable(tableName);
                    s = thisTable.GetDebugString("Table " + tableName + " ");
                    System.Diagnostics.Trace.WriteLine(s);

                    List<DecisionEngine.IColumnDefinition> defList = thisTable.getColumnDefinitions();

                }
                */
            }
            Assert.AreEqual("nasopharynx", lookup[0].getId());

            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "2");
            schemaLookup.setInput(EodInput.DISCRIMINATOR_2.toString(), "1");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            foreach (Schema schema in lookup)
            {
                hash1 = new HashSet<String>() { "discriminator_1", "discriminator_2" };
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));
            }
            Assert.AreEqual("oropharynx_p16_neg", lookup[0].getId());

            // test valid combination that requires a discriminator but is supplied a bad disciminator value
            schemaLookup = new EodSchemaLookup("C111", "8200");
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "X");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);

            // test searching on only site
            lookup = _STAGING.lookupSchema(new EodSchemaLookup("C401", null));
            Assert.AreEqual(9, lookup.Count);

            // test searching on only hist
            lookup = _STAGING.lookupSchema(new EodSchemaLookup(null, "9702"));
            Assert.AreEqual(8, lookup.Count);

            // test that searching on only discriminator_1 returns no results
            schemaLookup = new EodSchemaLookup(null, null);
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "1");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);
            schemaLookup = new EodSchemaLookup("", null);
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "1");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);
            schemaLookup = new EodSchemaLookup(null, "");
            schemaLookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "1");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);

            // test lookups based on sex
            schemaLookup = new EodSchemaLookup("C481", "8720");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(2, lookup.Count);
            schemaLookup.setInput(EodInput.SEX.toString(), "1");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("retroperitoneum", lookup[0].getId());

        }

        [TestMethod]
        public void testDiscriminatorInputs()
        {
            HashSet<String> allDiscriminators = new HashSet<String>();
            HashSet<String> theseDiscriminators;
            Schema ss;

            HashSet<String> algorithms = _STAGING.getSchemaIds();
            foreach (String schemaId in algorithms)
            {
                ss = _STAGING.getSchema(schemaId);
                if (ss != null)
                {
                    theseDiscriminators = ss.getSchemaDiscriminators();
                    if (theseDiscriminators != null)
                    {
                        allDiscriminators.UnionWith(theseDiscriminators);
                    }
                }
            }

            HashSet<String> test1 = new HashSet<String>();
            test1.Add("year_dx");
            test1.Add("sex");
            test1.Add("behavior");
            test1.Add("discriminator_1");
            test1.Add("discriminator_2");

            Assert.IsTrue(test1.SetEquals(allDiscriminators));
        }

        [TestMethod]
        public void testLookupCache()
        {
            // do the same lookup twice
            List<Schema> lookup = _STAGING.lookupSchema(new EodSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("soft_tissue_rare", lookup[0].getId());

            lookup = _STAGING.lookupSchema(new EodSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("soft_tissue_rare", lookup[0].getId());

            // now invalidate the cache
            EodDataProvider.getInstance(EodVersion.LATEST).invalidateCache();

            // try the lookup again
            lookup = _STAGING.lookupSchema(new EodSchemaLookup("C629", "9231"));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("soft_tissue_rare", lookup[0].getId());
        }

        [TestMethod]
        public void testFindTableRow()
        {
            Assert.AreEqual(-1, _STAGING.findMatchingTableRow("tumor_size_clinical_60979", "size_clin", "00X"));

            // null maps to blank
            Assert.AreEqual(0, _STAGING.findMatchingTableRow("tumor_size_clinical_60979", "size_clin", "000"));
            Assert.AreEqual(2, _STAGING.findMatchingTableRow("tumor_size_clinical_60979", "size_clin", "002"));
            Assert.AreEqual(2, _STAGING.findMatchingTableRow("tumor_size_clinical_60979", "size_clin", "100"));
            Assert.AreEqual(2, _STAGING.findMatchingTableRow("tumor_size_clinical_60979", "size_clin", "988"));
            Assert.AreEqual(5, _STAGING.findMatchingTableRow("tumor_size_clinical_60979", "size_clin", "999"));
        }


        [TestMethod]
        public void testStagePancreas()
        {
            EodStagingData data = new EodStagingData.EodStagingInputBuilder()
                    .withInput(EodInput.DX_YEAR, "2018")
                    .withInput(EodInput.PRIMARY_SITE, "C250")
                    .withInput(EodInput.HISTOLOGY, "8154")
                    .withInput(EodInput.EOD_PRIMARY_TUMOR, "500")
                    .withInput(EodInput.EOD_REGIONAL_NODES, "300")
                    .withInput(EodInput.EOD_METS, "10")
                    .withInput(EodInput.NODES_POS, "03").build();

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("pancreas", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(13, data.getPath().Count);
            Assert.AreEqual(8, data.getOutput().Count);

            // check outputs
            Assert.AreEqual(EodVersion.LATEST.getVersion(), data.getOutput(EodOutput.DERIVED_VERSION));
            Assert.AreEqual("7", data.getOutput(EodOutput.SS_2018_DERIVED));
            Assert.AreEqual("00280", data.getOutput(EodOutput.NAACCR_SCHEMA_ID));
            Assert.AreEqual("4", data.getOutput(EodOutput.EOD_2018_STAGE_GROUP));
            Assert.AreEqual("TX", data.getOutput(EodOutput.EOD_2018_T));
            Assert.AreEqual("N1", data.getOutput(EodOutput.EOD_2018_N));
            Assert.AreEqual("M1", data.getOutput(EodOutput.EOD_2018_M));
        }

        [TestMethod]
        public void testStageDefaultSsdi()
        {
            EodStagingData data = new EodStagingData.EodStagingInputBuilder()
                    .withInput(EodInput.PRIMARY_SITE, "C502")
                    .withInput(EodInput.HISTOLOGY, "8500")
                    .withInput(EodInput.BEHAVIOR, "3")
                    .withInput(EodInput.DX_YEAR, "2020")
                    .withInput(EodInput.TUMOR_SIZE_SUMMARY, "025")
                    .withInput(EodInput.EOD_PRIMARY_TUMOR, "100")
                    .withInput(EodInput.EOD_REGIONAL_NODES, "200")
                    .withInput(EodInput.EOD_METS, "00")
                    .withInput(EodInput.GRADE_CLIN, "1")
                    .withInput(EodInput.GRADE_PATH, "1")
                    .build();

            // add SSDIs
            // - Lymph Nodes Pos Axillary Level I-II: leave blank, should default to X8)
            // - Oncotype DX Recur Score: leave blank, should default to XX9
            data.setInput("er", "1");
            data.setInput("pr", "0");
            data.setInput("her2_summary", "0");

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("breast", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(16, data.getPath().Count);
            Assert.AreEqual(8, data.getOutput().Count);

            // check outputs
            Assert.AreEqual(EodDataProvider.getInstance().getVersion(), data.getOutput(EodOutput.DERIVED_VERSION));
            Assert.AreEqual("3", data.getOutput(EodOutput.SS_2018_DERIVED));
            Assert.AreEqual("00480", data.getOutput(EodOutput.NAACCR_SCHEMA_ID));
            Assert.AreEqual("2B", data.getOutput(EodOutput.EOD_2018_STAGE_GROUP));
            Assert.AreEqual("T2", data.getOutput(EodOutput.EOD_2018_T));
            Assert.AreEqual("N1", data.getOutput(EodOutput.EOD_2018_N));
            Assert.AreEqual("M0", data.getOutput(EodOutput.EOD_2018_M));
        }

        [TestMethod]
        public void testBadLookupInStage()
        {
            EodStagingData data = new EodStagingData();

            // if site/hist are not supplied, no lookup
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY, data.getResult());

            // add hist only and it should fail with same result
            data.setInput(EodInput.PRIMARY_SITE, "C489");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY, data.getResult());

            // put a site/hist combo that doesn't match a schema
            data.setInput(EodInput.HISTOLOGY, "9898");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_NO_MATCHING_SCHEMA, data.getResult());

            // now a site/hist that returns multiple results
            data.setInput(EodInput.PRIMARY_SITE, "C111");
            data.setInput(EodInput.HISTOLOGY, "8200");
            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS, data.getResult());
        }

        [TestMethod]
        public void testInvolvedTables()
        {
            HashSet<String> tables = _STAGING.getInvolvedTables("adnexa_uterine_other");
            HashSet<String> hash1 = new HashSet<String>() {
                "seer_mets_48348", "nodes_dcc", "grade_clinical_standard_non_ajcc_32473", "grade_pathological_standard_non_ajcc_5627",
                "adnexa_uterine_other_97891", "nodes_pos_fpa", "tumor_size_pathological_25597", "tumor_size_clinical_60979", "primary_site", "histology",
                "nodes_exam_76029", "grade_post_therapy_clin_69737", "grade_post_therapy_path_75348", "schema_selection_adnexa_uterine_other",
                "year_dx_validation", "summary_stage_rpa", "tumor_size_summary_63115", "extension_bcn", "combined_grade_56638", "neoadjuvant_therapy_37302",
                "derived_grade_standard_non_ajcc_63932", "neoadj_tx_treatment_effect_18122", "neoadj_tx_clinical_response_31723", "ss2018_adnexa_uterine_other_values_44976"};

            Assert.IsTrue(tables.SetEquals(hash1));
        }

        [TestMethod]
        public void testInvolvedSchemas()
        {
            HashSet<String> schemas = _STAGING.getInvolvedSchemas("her2_summary_30512");
            HashSet<String> hash1 = new HashSet<String>() { "breast" };
            Assert.IsTrue(hash1.SetEquals(schemas));
        }

        [TestMethod]
        public void testGetInputs()
        {
            HashSet<String> test1 = new HashSet<String>() { "eod_mets", "site", "hist", "eod_primary_tumor", "eod_regional_nodes", "eod_regional_nodes", "grade_path", "grade_clin" };
            HashSet<String> test2 = _STAGING.getInputs(_STAGING.getSchema("adnexa_uterine_other"));
            Assert.IsTrue(test1.SetEquals(test2));

            test1 = new HashSet<String>() { "eod_mets", "site", "hist", "nodes_pos", "s_category_path", "eod_primary_tumor", "s_category_clin", "eod_regional_nodes", "grade_path", "grade_clin" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("testis"));
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
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "year_dx", null));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "200"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "2003"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "year_dx", "2145"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "year_dx", "2018"));

            // test valid and invalid fields
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "eod_primary_tumor", "000"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "eod_primary_tumor", "001"));
            Assert.IsTrue(_STAGING.isCodeValid("urethra", "discriminator_1", "1"));
            Assert.IsFalse(_STAGING.isCodeValid("urethra", "discriminator_1", "9"));
        }

        [TestMethod]
        public void testIsContextValid()
        {
            EodStagingData data = new EodStagingData();

            data.setInput(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT, "2018");

            // test valid year
            data.setInput(EodInput.DX_YEAR, "2018");
            Assert.IsTrue(_STAGING.isContextValid("urethra", StagingData.YEAR_DX_KEY, data.getInput()));

            // test invalid year
            data.setInput(EodInput.DX_YEAR, "2016");
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
            Assert.IsTrue(tables.Contains("urethra_prostatic_urethra_30106"));
        }

        [TestMethod]
        public void testGetSchema()
        {
            Assert.IsNull(_STAGING.getSchema("bad_schema_name"));
            Assert.IsNotNull(_STAGING.getSchema("brain"));
            Assert.AreEqual("Brain [8th: 2018-2022]", _STAGING.getSchema("brain").getName());
        }

        [TestMethod]
        public void testLookupOutputs()
        {
            EodSchemaLookup lookup = new EodSchemaLookup("C680", "8590");
            List<Schema> lookups = _STAGING.lookupSchema(lookup);
            Assert.AreEqual(2, lookups.Count);

            Schema schema = _STAGING.getSchema(lookups[0].getId());
            Assert.AreEqual("urethra", schema.getId());

            // build list of output keys
            List<IOutput> outputs = schema.getOutputs();
            HashSet<String> definedOutputs = new HashSet<String>();
            foreach (IOutput o in outputs)
            {
                definedOutputs.Add(o.getKey());
            }

            // test without context
            Assert.IsTrue(definedOutputs.SetEquals(_STAGING.getOutputs(schema)));

            // test with context
            Assert.IsTrue(definedOutputs.SetEquals(_STAGING.getOutputs(schema, lookup.getInputs())));
        }

        [TestMethod]
        public void testEncoding()
        {
            ITable table = _STAGING.getTable("serum_alb_pretx_level_58159");

            Assert.IsNotNull(table);

            // the notes of this table contain UTF-8 characters, specifically the single quote character in this phrase: "You may use a physician’s statement"

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
        public void testContentReturnedForInvalidInput()
        {
            EodStagingData data = new EodStagingData.EodStagingInputBuilder()
                .withInput(EodInput.PRIMARY_SITE, "C713")
                .withInput(EodInput.HISTOLOGY, "8020")
                .withInput(EodInput.BEHAVIOR, "3")
                .withInput(EodInput.DX_YEAR, "2018")
                .withInput(EodInput.EOD_PRIMARY_TUMOR, "200")
                .withInput(EodInput.EOD_REGIONAL_NODES, "300")
                .withInput(EodInput.EOD_METS, "00").build();

            // perform the staging
            _STAGING.stage(data);

            if (data.getResult() != StagingData.Result.STAGED)
            {
                Assert.IsTrue(false);
            }
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("brain", data.getSchemaId());
            Assert.AreEqual(5, data.getErrors().Count);
            Assert.AreEqual(6, data.getPath().Count);
            Assert.AreEqual(8, data.getOutput().Count);
            Assert.AreEqual("3.2", data.getOutput(EodOutput.DERIVED_VERSION.toString()));
        }

        [TestMethod]
        public void testContentNotReturnedForInvalidYear()
        {
            EodStagingData data = new EodStagingData.EodStagingInputBuilder()
                .withInput(EodInput.PRIMARY_SITE, "C670")
                .withInput(EodInput.HISTOLOGY, "8000")
                .withInput(EodInput.BEHAVIOR, "3")
                .withInput(EodInput.DX_YEAR, "2010")
                .withInput(EodInput.EOD_PRIMARY_TUMOR, "200")
                .withInput(EodInput.EOD_REGIONAL_NODES, "300")
                .withInput(EodInput.EOD_METS, "00").build();

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.FAILED_INVALID_YEAR_DX, data.getResult());
            Assert.AreEqual("bladder", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(0, data.getPath().Count);
            Assert.AreEqual(0, data.getOutput().Count);
        }

        [TestMethod]
        public void testGlossary()
        {
            Assert.AreEqual(23, _STAGING.getGlossaryTerms().Count);
            GlossaryDefinition entry = _STAGING.getGlossaryDefinition("Medulla");
            Assert.IsNotNull(entry);
            Assert.AreEqual("Medulla", entry.getName());
            Assert.IsTrue(entry.getDefinition().StartsWith("The central portion of an organ, in contrast to the outer layer"));
            CollectionAssert.AreEqual(new List<string>() { "Medullary" }, entry.getAlternateNames());
            Assert.IsNotNull(entry.getLastModified());

            HashSet<String> hits = _STAGING.getSchemaGlossary("urethra");
            Assert.AreEqual(1, hits.Count);
            hits = _STAGING.getTableGlossary("extension_baj");
            Assert.AreEqual(3, hits.Count);
        }

        [TestMethod]
        public void testMetadata()
        {
            Schema urethra = _STAGING.getSchema("urethra");
            Assert.IsNotNull(urethra);

            IInput gradeClin = urethra.getInputMap()["grade_clin"];
            Assert.IsNotNull(gradeClin);

            Assert.AreEqual(gradeClin.getMetadata().Count, 5);
            Assert.IsTrue(gradeClin.getMetadata().Contains(new StagingMetadata("COC_REQUIRED")));
            Assert.IsTrue(gradeClin.getMetadata().Contains(new StagingMetadata("CCCR_REQUIRED")));
            Assert.IsTrue(gradeClin.getMetadata().Contains(new StagingMetadata("SEER_REQUIRED")));
            Assert.IsTrue(gradeClin.getMetadata().Contains(new StagingMetadata("NPCR_REQUIRED")));
            Assert.IsTrue(gradeClin.getMetadata().Contains(new StagingMetadata("SSDI")));
        }

        [TestMethod]
        public virtual void testCachedSiteAndHistology()
        {
            StagingDataProvider provider = getProvider();
            Assert.IsTrue(provider.getValidSites().Count > 0);
            Assert.IsTrue(provider.getValidHistologies().Count > 0);

            // site tests
            List<String> validSites = new List<String>() { "C000", "C809" };
            List<String> invalidSites = new List<String>() { "C727", "C810" };
            foreach (String site in validSites)
                Assert.IsTrue(provider.getValidSites().Contains(site));
            foreach (String site in invalidSites)
                Assert.IsFalse(provider.getValidSites().Contains(site));

            // hist tests
            List<String> validHist = new List<String>() { "8000", "8002", "8005", "8290", "9992" };
            List<String> invalidHist = new List<String>() { "8006", "9990" };
            foreach (String hist in validHist)
                Assert.IsTrue(provider.getValidHistologies().Contains(hist), "The histology '" + hist + "' is not in the valid histology list");
            foreach (String hist in invalidHist)
                Assert.IsFalse(provider.getValidHistologies().Contains(hist), "The histology '" + hist + "' is not supposed to be in the valid histology list");
        }
    }
}

