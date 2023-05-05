/*
 * Copyright (C) 2015 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Toronto;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;

namespace TNMStaging_UnitTestApp.Src.Staging.Toronto
{
    [TestClass]
    public class TorontoStagingTest : StagingTest
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
            return "toronto";
        }

        public override string getVersion()
        {
            return TorontoVersion.V0_5.getVersion();
        }

        public override StagingFileDataProvider getProvider()
        {
            return TorontoDataProvider.getInstance(TorontoVersion.LATEST);
        }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(TorontoDataProvider.getInstance(TorontoVersion.LATEST));
        }

        [TestMethod]
        public void testBasicInitialization()
        {
            Assert.AreEqual(_STAGING.getSchemaIds().Count, 33);
            Assert.IsTrue(_STAGING.getTableIds().Count > 0);

            Assert.IsNotNull(_STAGING.getSchema("ependymoma"));
            Assert.IsNotNull(_STAGING.getTable("st_jude_murphy_staging_system_35179"));
        }

        [TestMethod]
        public void testVersionInitializationTypes()
        {
            TNMStagingCSharp.Src.Staging.Staging staging10 = TNMStagingCSharp.Src.Staging.Staging.getInstance(TorontoDataProvider.getInstance(TorontoVersion.LATEST));
            Assert.AreEqual(TorontoVersion.LATEST.getVersion(), staging10.getVersion());

            TNMStagingCSharp.Src.Staging.Staging stagingLatest = TNMStagingCSharp.Src.Staging.Staging.getInstance(TorontoDataProvider.getInstance());
            Assert.AreEqual(TorontoVersion.LATEST.getVersion(), stagingLatest.getVersion());
        }

        [TestMethod]
        public void testDescriminatorKeys()
        {
            HashSet<String> hash1 = new HashSet<String>() { "age_dx" };
            HashSet<String> hash2 = _STAGING.getSchema("acute_lymphoblastic_leukemia").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "age_dx", "behavior" };
            hash2 = _STAGING.getSchema("ovarian").getSchemaDiscriminators();
            Assert.IsTrue(hash1.SetEquals(hash2));

            // check all schema discriminators
            HashSet<string> allSchemaIds = _STAGING.getSchemaIds();
            HashSet<string> checkDiscrim = new HashSet<String>() { "age_dx", "behavior" };
            foreach (string schemaId in allSchemaIds)
            {
                HashSet<string> discrim = _STAGING.getSchema(schemaId).getSchemaDiscriminators();
                if (discrim != null)
                {
                    Assert.IsTrue(discrim.IsSubsetOf(checkDiscrim));
                }
            }
        }

        [TestMethod]
        public override void testValidCode()
        {
            Dictionary<string, string> context = new Dictionary<string, string>();
            context.Add("hist", "7000");
            Assert.IsFalse(_STAGING.isContextValid("ovarian", "hist", context));
            context["hist"] = "8000";
            Assert.IsTrue(_STAGING.isContextValid("ovarian", "hist", context));
            context["hist"] = "8542";
            Assert.IsTrue(_STAGING.isContextValid("ovarian", "hist", context));

            // make sure null is handled
            context["hist"] = null;
            Assert.IsFalse(_STAGING.isContextValid("ovarian", "hist", context));

            // make sure blank is handled
            context["hist"] = "";
            Assert.IsFalse(_STAGING.isContextValid("ovarian", "hist", context));
        }

        [TestMethod]
        public void testInputAndOutput()
        {
            HashSet<string> inputs = new HashSet<string>();
            HashSet<string> outputs = new HashSet<string>();
            HashSet<string> allSchemaIds = _STAGING.getSchemaIds();
            foreach (string schemaId in allSchemaIds)
            {
                Schema schema = _STAGING.getSchema(schemaId);
                inputs.UnionWith(_STAGING.getInputs(schema));
                outputs.UnionWith(_STAGING.getOutputs(schema));
            }

            // note that while year_dx is not "used for staging" it is validated at the start of the process so it kind of is
            inputs.Add("year_dx");

            // this verified that the inputs/outputs are an exact match to the Input and Output enums
            int count = 0;
            foreach (TorontoInput input in TorontoInput.Values)
            {
                Assert.IsTrue(inputs.Contains(input.toString()));
                count++;
            }
            Assert.AreEqual(inputs.Count, count);

            count = 0;
            foreach (TorontoOutput output in TorontoOutput.Values)
            {
                Assert.IsTrue(outputs.Contains(output.toString()));
                count++;
            }
            Assert.AreEqual(outputs.Count, count);
        }

        [TestMethod]
        public void testSchemaSelection()
        {
            // test bad values
            List<Schema> lookup = _STAGING.lookupSchema(new SchemaLookup());
            Assert.AreEqual(0, lookup.Count);

            lookup = _STAGING.lookupSchema(new TorontoSchemaLookup("XXX", "YYY"));
            Assert.AreEqual(0, lookup.Count);

            // test valid combinations that do not require a discriminator
            TorontoSchemaLookup schemaLookup = new TorontoSchemaLookup("C220", "8970");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("hepatoblastoma", lookup[0].getId());

            // test valid combination that requires a discriminator but is not supplied one
            lookup = _STAGING.lookupSchema(new TorontoSchemaLookup("C723", "9384"));
            Assert.AreEqual(2, lookup.Count);
            HashSet<String> hash1 = new HashSet<String>() { "astrocytoma", "adult_other_non_pediatric" };
            HashSet<String> hash2 = new HashSet<String>();
            foreach (Schema schema in lookup)
            {
                hash2.Add(schema.getId());
            }
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "age_dx", "behavior" };
            foreach (Schema schema in lookup)
            {
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash2.IsSubsetOf(hash1));
            }

            // test valid combination that requires discriminator and a good discriminator is supplied
            schemaLookup = new TorontoSchemaLookup("C723", "9384");
            schemaLookup.setInput(TorontoInput.AGE_DX.toString(), "11");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(1, lookup.Count);
            hash1 = new HashSet<String>() { "age_dx" };
            foreach (Schema schema in lookup)
            {
                hash2 = schema.getSchemaDiscriminators();
                Assert.IsTrue(hash1.SetEquals(hash2));
            }
            Assert.AreEqual("astrocytoma", lookup[0].getId());

            // test valid combination that requires a discriminator but is supplied a bad disciminator value
            schemaLookup = new TorontoSchemaLookup("C723", "9384");
            schemaLookup.setInput(TorontoInput.AGE_DX.toString(), "XX");
            lookup = _STAGING.lookupSchema(schemaLookup);
            Assert.AreEqual(0, lookup.Count);

            // test searching on only site
            lookup = _STAGING.lookupSchema(new TorontoSchemaLookup("C401", null));
            Assert.AreEqual(16, lookup.Count);

            // test searching on only hist
            lookup = _STAGING.lookupSchema(new TorontoSchemaLookup(null, "9702"));
            Assert.AreEqual(2, lookup.Count);
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
            test1.Add("age_dx");
            test1.Add("behavior");

            Assert.IsTrue(test1.SetEquals(allDiscriminators));
        }

        [TestMethod]
        public void testLookupCache()
        {
            string site = "C710";
            string hist = "9392";
            string schemaId = "ependymoma";

            // do the same lookup twice
            List<Schema> lookup = _STAGING.lookupSchema(new TorontoSchemaLookup(site, hist));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual(schemaId, lookup[0].getId());

            lookup = _STAGING.lookupSchema(new TorontoSchemaLookup(site, hist));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual(schemaId, lookup[0].getId());

            // now invalidate the cache
            TorontoDataProvider.getInstance(TorontoVersion.V0_5).invalidateCache();

            // try the lookup again
            lookup = _STAGING.lookupSchema(new TorontoSchemaLookup(site, hist));
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual(schemaId, lookup[0].getId());
        }

        [TestMethod]
        public void testFindTableRow()
        {
            string tableId = "toronto_m_65862";
            string colId = "eod_mets";

            Assert.IsNotNull(_STAGING.getTable(tableId));

            Assert.AreEqual(-1, _STAGING.findMatchingTableRow(tableId, colId, "XX"));

            // null maps to blank
            Assert.AreEqual(0, _STAGING.findMatchingTableRow(tableId, colId, "00"));
            Assert.AreEqual(0, _STAGING.findMatchingTableRow(tableId, colId, "99"));
            Assert.AreEqual(1, _STAGING.findMatchingTableRow(tableId, colId, "10"));
            Assert.AreEqual(1, _STAGING.findMatchingTableRow(tableId, colId, "30"));
            Assert.AreEqual(1, _STAGING.findMatchingTableRow(tableId, colId, "70"));
        }

        [TestMethod]
        public void testStageOvarian()
        {
            TorontoStagingData data = new TorontoStagingData.TorontoStagingInputBuilder()
                    .withInput(TorontoInput.PRIMARY_SITE, "C569")
                    .withInput(TorontoInput.HISTOLOGY, "9081")
                    .withInput(TorontoInput.YEAR_DX, "2021")
                    .withInput(TorontoInput.AGE_DX, "16")
                    .withInput(TorontoInput.BEHAVIOR, "3")
                    .withInput(TorontoInput.SCHEMA_ID, "00459")
                    .withInput(TorontoInput.EOD_PRIMARY_TUMOR, "200")
                    .withInput(TorontoInput.EOD_REGIONAL_NODES, "300")
                    .withInput(TorontoInput.EOD_METS, "30").build();

            // perform the staging
            _STAGING.stage(data);

            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
            Assert.AreEqual("ovarian", data.getSchemaId());
            Assert.AreEqual(0, data.getErrors().Count);
            Assert.AreEqual(4, data.getPath().Count);
            Assert.IsTrue(data.getPath().Contains("toronto_stage.toronto_t_78796"));
            Assert.IsTrue(data.getPath().Contains("toronto_stage.toronto_n_26872"));
            Assert.IsTrue(data.getPath().Contains("toronto_stage.toronto_m_81745"));
            Assert.IsTrue(data.getPath().Contains("toronto_stage.toronto_stage_43114"));
            Assert.AreEqual(7, data.getOutput().Count);

            // check outputs
            Assert.AreEqual(TorontoVersion.LATEST.getVersion(), data.getOutput(TorontoOutput.DERIVED_VERSION));

            Assert.AreEqual(7, data.getOutput().Count);
            Assert.AreEqual(TorontoVersion.LATEST.getVersion(), data.getOutput(TorontoOutput.DERIVED_VERSION));
            Assert.AreEqual("1", data.getOutput(TorontoOutput.TORONTO_VERSION_NUMBER));
            Assert.AreEqual("10c2", data.getOutput(TorontoOutput.TORONTO_ID));
            Assert.AreEqual("4", data.getOutput(TorontoOutput.TORONTO_GROUP));
            Assert.AreEqual("T2", data.getOutput(TorontoOutput.TORONTO_T));
            Assert.AreEqual("N1", data.getOutput(TorontoOutput.TORONTO_N));
            Assert.AreEqual("M1", data.getOutput(TorontoOutput.TORONTO_M));
        }

        [TestMethod]
        public void testBadLookupInStage()
        {
            TorontoStagingData data = new TorontoStagingData();
            data.setInput(TorontoInput.AGE_DX, "15");

            // if site/hist are not supplied, no lookup
            _STAGING.stage(data);
            Assert.AreEqual(data.getResult(), StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY);

            // add hist only and it should fail with same result
            data.setInput(TorontoInput.PRIMARY_SITE, "C489");
            _STAGING.stage(data);
            Assert.AreEqual(data.getResult(), StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY);

            // put a site/hist combo that doesn't match a schema
            data.setInput(TorontoInput.HISTOLOGY, "9898");
            _STAGING.stage(data);
            Assert.AreEqual(data.getResult(), StagingData.Result.FAILED_NO_MATCHING_SCHEMA);

            // now a site/hist that returns multiple results
            data.setInput(TorontoInput.PRIMARY_SITE, "C699");
            data.setInput(TorontoInput.HISTOLOGY, "9500");
            _STAGING.stage(data);
            Assert.AreEqual(data.getResult(), StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS);

            // test other constructors
            _STAGING.stage(new TorontoStagingData("C699", "9500", "15"));
            Assert.AreEqual(data.getResult(), StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS);

            data = new TorontoStagingData("C699", "9500");
            data.setInput(TorontoInput.AGE_DX, "15");
            _STAGING.stage(data);
            Assert.AreEqual(data.getResult(), StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS);

            Assert.AreEqual(data.getInput(TorontoInput.PRIMARY_SITE), "C699");
            Assert.AreEqual(data.getInput(TorontoInput.HISTOLOGY), "9500");
            Assert.AreEqual(data.getInput(TorontoInput.AGE_DX), "15");
        }

        [TestMethod]
        public void testInvolvedTables()
        {
            HashSet<String> tables = _STAGING.getInvolvedTables("testicular");

            HashSet<String> hash1 = new HashSet<String>() {
                "toronto_n_21728",
                "age_at_diagnosis_validation_3881",
                "toronto_t_57044",
                "toronto_m_65862",
                "combined_s_category_15139",
                "schema_selection_testicular",
                "schema_id_42744",
                "s_category_clinical_11368",
                "nodes_pos_fpa",
                "toronto_stage_81706",
                "primary_site",
                "s_category_pathological_46197",
                "eod_primary_tumor_63650",
                "histology",
                "year_dx_validation",
                "eod_mets_68192",
                "behavior",
                "eod_regional_nodes_4689" };

            Assert.IsTrue(tables.SetEquals(hash1));
        }

        [TestMethod]
        public void testInvolvedSchemas()
        {
            HashSet<String> schemas = _STAGING.getInvolvedSchemas("s_category_clinical_11368");
            HashSet<String> hash1 = new HashSet<String>() { "testicular" };
            Assert.IsTrue(hash1.SetEquals(schemas));
        }

        [TestMethod]
        public void testGetInputs()
        {
            HashSet<String> test1 = new HashSet<String>() { "site", "hist", "age_dx", "behavior" };
            HashSet<String> test2 = _STAGING.getInputs(_STAGING.getSchema("nhl_nos"));
            Assert.IsTrue(test1.SetEquals(test2));

            test1 = new HashSet<String>() { "eod_mets", "site", "hist", "nodes_pos", "age_dx", "s_category_path", "schema_id", "eod_primary_tumor", "s_category_clin", "behavior", "eod_regional_nodes" };
            test2 = _STAGING.getInputs(_STAGING.getSchema("testicular"));
            Assert.IsTrue(test1.SetEquals(test2));
        }

        [TestMethod]
        public void testIsCodeValid()
        {
            // test bad parameters for schema or field
            Assert.IsFalse(_STAGING.isCodeValid("bad_schema_name", "site", "C509"));
            Assert.IsFalse(_STAGING.isCodeValid("testicular", "bad_field_name", "C509"));

            string schemaId = "astrocytoma";

            // test null values
            Assert.IsFalse(_STAGING.isCodeValid(null, null, null));
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, null, null));
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, "site", null));

            // test fields that have a "value" specified
            Assert.IsTrue(_STAGING.isCodeValid(schemaId, "year_dx", null)); // year_dx is now allowed to be null
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, "year_dx", "200"));
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, "year_dx", "2003"));
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, "year_dx", "2145"));
            Assert.IsTrue(_STAGING.isCodeValid(schemaId, "year_dx", "2018"));

            // test valid and invalid fields
            Assert.IsTrue(_STAGING.isCodeValid(schemaId, "braf_mutational_analysis", "2"));
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, "braf_mutational_analysis", "5"));
            Assert.IsTrue(_STAGING.isCodeValid(schemaId, "eod_mets", "10"));
            Assert.IsFalse(_STAGING.isCodeValid(schemaId, "eod_mets", "20"));
        }

        [TestMethod]
        public void testGetSchema()
        {
            Assert.IsNull(_STAGING.getSchema("bad_schema_name"));
            Assert.IsNotNull(_STAGING.getSchema("hepatoblastoma"));
            Assert.AreEqual("Hepatoblastoma", _STAGING.getSchema("hepatoblastoma").getName());
        }

        [TestMethod]
        public void testLookupOutputs()
        {
            TorontoSchemaLookup lookup = new TorontoSchemaLookup("C569", "9091");
            List<Schema> lookups = _STAGING.lookupSchema(lookup);
            Assert.AreEqual(2, lookups.Count);

            HashSet<string> testSet = new HashSet<string>() { "ovarian", "adult_other_non_pediatric" };
            HashSet<string> lookupSet = new HashSet<string>();
            foreach (Schema s in lookups)
            {
                lookupSet.Add(s.getId());
            }
            Assert.IsTrue(lookupSet.SetEquals(testSet));
            //assertThat(lookups).extracting("id").containsExactlyInAnyOrder("ovarian", "adult_other_non_pediatric");

            Schema schema = _STAGING.getSchema("ovarian");

            // build list of output keys
            List<IOutput> outputs = schema.getOutputs();
            HashSet<String> definedOutputs = new HashSet<String>();
            foreach (IOutput o in outputs)
            {
                definedOutputs.Add(o.getKey());
            }

            // test without context
            Assert.IsTrue(_STAGING.getOutputs(schema).SetEquals(definedOutputs));

            // test with context
            Assert.IsTrue(_STAGING.getOutputs(schema, lookup.getInputs()).SetEquals(definedOutputs));
        }

        [TestMethod]
        public void testGlossary()
        {
            Assert.IsNotNull(_STAGING.getGlossaryTerms());
            Assert.IsTrue(_STAGING.getGlossaryTerms().Count > 0);
            GlossaryDefinition entry = _STAGING.getGlossaryDefinition("Cortex");
            Assert.IsNotNull(entry);
            Assert.AreEqual("Cortex", entry.getName());
            Assert.IsTrue(entry.getDefinition().StartsWith("The external or outer surface layer of an organ"));
            Assert.IsTrue(entry.getAlternateNames().Contains("Cortical"));
            Assert.IsNotNull(entry.getLastModified());
        }


        [TestMethod]
        public void testMetadata()
        {
            Schema schema = _STAGING.getSchema("testicular");
            Assert.IsNotNull(schema);

            IInput input = schema.getInputMap()["s_category_clin"];
            Assert.IsNotNull(input);

            Assert.AreEqual(input.getMetadata().Count, 2);
            Assert.IsTrue(input.getMetadata().Contains(new StagingMetadata("SEER_REQUIRED")));
            Assert.IsTrue(input.getMetadata().Contains(new StagingMetadata("SSDI")));
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
            List<String> validHist = new List<String>() { "8000", "8002", "8005", "8290", "9992", "9993" };
            List<String> invalidHist = new List<String>() { "8006", "9990" };

            foreach (String hist in validHist)
                Assert.IsTrue(provider.getValidHistologies().Contains(hist), "The histology '" + hist + "' is not in the valid histology list");
            foreach (String hist in invalidHist)
                Assert.IsFalse(provider.getValidHistologies().Contains(hist), "The histology '" + hist + "' is not supposed to be in the valid histology list");
        }
    }
}


