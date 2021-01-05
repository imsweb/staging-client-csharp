﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.DecisionEngine;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.EOD;

namespace TNMStaging_UnitTestApp.Src
{
    /**
     * Base class for all algorithm-specific testing
     */
    public abstract class StagingTest
    {

        protected static TNMStagingCSharp.Src.Staging.Staging _STAGING;

        //* Return the algorithm name
        public abstract String getAlgorithm();

        // * Return the algorithm version
        public abstract String getVersion();

        // * Return the staging data provider
        public abstract StagingFileDataProvider getProvider();

        [TestMethod]
        public void testInitialization()
        {
            Assert.AreEqual(getAlgorithm(), _STAGING.getAlgorithm());
            Assert.AreEqual(getVersion(), _STAGING.getVersion());
        }

        [TestMethod]
        public void testInitAllTables()
        {
            foreach (String id in _STAGING.getTableIds())
            {
                StagingTable table = _STAGING.getTable(id);

                Assert.IsNotNull(table);
                Assert.IsNotNull(table.getAlgorithm());
                Assert.IsNotNull(table.getVersion());
                Assert.IsNotNull(table.getName());
            }
        }

        [TestMethod]
        public void testValidCode()
        {
            Dictionary<String, String> context = new Dictionary<String, String>();
            context["hist"] = "7000";
            Assert.IsFalse(_STAGING.isContextValid("prostate", "hist", context));
            context["hist"] = "8000";
            Assert.IsTrue(_STAGING.isContextValid("prostate", "hist", context));
            context["hist"] = "8542";
            Assert.IsTrue(_STAGING.isContextValid("prostate", "hist", context));

            // make sure null is handled
            context["hist"] = null;
            Assert.IsFalse(_STAGING.isContextValid("prostate", "hist", context));

            // make sure blank is handled
            context["hist"] = "";
            Assert.IsFalse(_STAGING.isContextValid("prostate", "hist", context));
        }

        [TestMethod]
        public void testBasicInputs()
        {
            // all inputs for all schemas will have null unit and decimal places
            foreach (String id in _STAGING.getSchemaIds())
            {
                StagingSchema schema = _STAGING.getSchema(id);
                foreach (StagingSchemaInput input in schema.getInputs())
                {
                    Assert.IsNull(input.getUnit(), "No schemas should have units");
                    Assert.IsTrue(input.getDecimalPlaces() == 0, "No schemas should have decimal places");
                }
            }
        }

        [TestMethod]
        public void testValidSite()
        {
            Assert.IsFalse(_STAGING.isValidSite(null));
            Assert.IsFalse(_STAGING.isValidSite(""));
            Assert.IsFalse(_STAGING.isValidSite("C21"));
            Assert.IsFalse(_STAGING.isValidSite("C115"));

            Assert.IsTrue(_STAGING.isValidSite("C509"));
        }

        [TestMethod]
        public void testValidHistology()
        {
            Assert.IsFalse(_STAGING.isValidHistology(null));
            Assert.IsFalse(_STAGING.isValidHistology(""));
            Assert.IsFalse(_STAGING.isValidHistology("810"));
            Assert.IsFalse(_STAGING.isValidHistology("8176"));

            Assert.IsTrue(_STAGING.isValidHistology("8000"));
            Assert.IsTrue(_STAGING.isValidHistology("8201"));
        }

        [TestMethod] //(expected = IllegalStateException.class)
        public void testGetTable()
        {
            Assert.IsNull(_STAGING.getTable("bad_table_name"));
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
            List<String> invalidHist = new List<String>() { "8006", "9993" };
            foreach (String hist in validHist)
                Assert.IsTrue(provider.getValidHistologies().Contains(hist), "The histology '" + hist + "' is not in the valid histology list");
            foreach (String hist in invalidHist)
                Assert.IsFalse(provider.getValidHistologies().Contains(hist), "The histology '" + hist + "' is not supposed to be in the valid histology list");
        }

        [TestMethod]
        public void testForUnusedTables()
        {
            HashSet<String> usedTables = new HashSet<String>();
            HashSet<String> theseInvolvedTables;
            foreach (String id in _STAGING.getSchemaIds())
            {
                theseInvolvedTables = _STAGING.getSchema(id).getInvolvedTables();
                foreach (String s in theseInvolvedTables)
                {
                    usedTables.Add(s);
                }
            }

            HashSet<String> unusedTables = new HashSet<String>();
            HashSet<String> tablesIds = _STAGING.getTableIds();
            foreach (String id in tablesIds)
            {
                if (!usedTables.Contains(id))
                {
                    usedTables.Add(id);
                }
            }

            if (!(unusedTables.Count == 0))
                Assert.Fail("There are " + unusedTables.Count + " tables that are not used in any schema: " + unusedTables);
        }

        [TestMethod]
        public void testInputTables()
        {
            HashSet<String> errors = new HashSet<String>();

            foreach (String schemaId in _STAGING.getSchemaIds())
            {
                StagingSchema schema = _STAGING.getSchema(schemaId);

                // build a list of input tables that should be excluded
                foreach (StagingSchemaInput input in schema.getInputs())
                {
                    if (input.getTable() != null)
                    {
                        HashSet<String> inputKeys = new HashSet<String>();
                        StagingTable table = _STAGING.getTable(input.getTable());
                        foreach (StagingColumnDefinition def in table.getColumnDefinitions())
                            if (ColumnType.INPUT == def.getType())
                                inputKeys.Add(def.getKey());

                        // make sure the input key matches the an input column
                        if (!inputKeys.Contains(input.getKey()))
                            errors.Add("Input key " + schemaId + ":" + input.getKey() + " does not match validation table " + table.getId() + ": " + inputKeys.ToString());
                    }
                }
            }

            assertNoErrors(errors, "input values and their assocated validation tables");
        }

        [TestMethod]
        public void verifyInputs()
        {
            HashSet<String> errors = new HashSet<String>();

            foreach (String id in _STAGING.getSchemaIds())
            {
                StagingSchema schema = _STAGING.getSchema(id);

                // loop over all the inputs returned by processing the schema and make sure they are all part of the main list of inputs on the schema
                foreach (String input in _STAGING.getInputs(schema))
                    if (!schema.getInputMap().ContainsKey(input))
                        errors.Add("Error processing schema " + schema.getId() + ": Table input '" + input + "' not in master list of inputs");
            }

            assertNoErrors(errors, "input values");
        }

        [TestMethod]
        public void testMappingIdUniqueness()
        {
            HashSet<String> errors = new HashSet<String>();

            foreach (String schemaId in _STAGING.getSchemaIds())
            {
                StagingSchema schema = _STAGING.getSchema(schemaId);

                // build a list of input tables that should be excluded
                HashSet<String> ids = new HashSet<String>();
                List<IMapping> mappings = schema.getMappings();
                if (mappings != null)
                    foreach (StagingMapping mapping in mappings)
                    {
                        if (ids.Contains(mapping.getId()))
                            errors.Add("The mapping id " + schemaId + ":" + mapping.getId() + " is duplicated.  This should never happen");
                        ids.Add(mapping.getId());
                    }
            }

            assertNoErrors(errors, "input values and their assocated validation tables");
        }

        [TestMethod]
        public void testBehaviorDescriminator()
        {
            // test valid combination that requires discriminator and a good discriminator is supplied
            if (_STAGING.getAlgorithm().Equals("eod_public"))
            {
                EodSchemaLookup lookup = new EodSchemaLookup("C717", "9591");
                List<StagingSchema> lookups = _STAGING.lookupSchema(lookup);
                Assert.AreEqual(lookups.Count, 3);
                lookup.setInput(EodInput.DISCRIMINATOR_1.toString(), "1");
                lookup.setInput(EodInput.BEHAVIOR.toString(), "3");
                lookups = _STAGING.lookupSchema(lookup);
                Assert.AreEqual(lookups.Count, 1);
                Assert.AreEqual(lookups[0].getId(), "hemeretic");
            }
        }

        [TestMethod]
        public void testStagingEnums()
        {
            if (_STAGING.getAlgorithm().Equals("eod_public"))
            {
                HashSet<String> enumInput = new HashSet<String>();
                foreach (EodInput value in EodInput.Values)
                {
                    enumInput.Add(value.toString());
                }

                HashSet<String> enumOutput = new HashSet<String>();
                foreach (EodOutput value in EodOutput.Values)
                {
                    enumOutput.Add(value.toString());
                }

                // collect all input and output fields from all schemas
                HashSet<String> schemaInput = new HashSet<String>();
                HashSet<String> schemaOutput = new HashSet<String>();
                foreach (String schemaId in _STAGING.getSchemaIds())
                {
                    StagingSchema schema = _STAGING.getSchema(schemaId);

                    schemaInput.UnionWith(_STAGING.getInputs(schema));
                    schemaOutput.UnionWith(_STAGING.getOutputs(schema));
                }

                Assert.IsTrue(schemaInput.SetEquals(enumInput));
                Assert.IsTrue(schemaOutput.SetEquals(enumOutput));
            }
        }

        [TestMethod]
        public void testNaaccrXmlIds()
        {
            if (_STAGING.getAlgorithm().Equals("eod_public"))
            {
                List<String> errors = new List<String>();

                Dictionary<String, HashSet<String>> inputMappings = new Dictionary<String, HashSet<String>>();
                Dictionary<String, HashSet<String>> outputMappings = new Dictionary<String, HashSet<String>>();
                foreach (String schemaId in _STAGING.getSchemaIds())
                {
                    StagingSchema schema = _STAGING.getSchema(schemaId);

                    foreach (StagingSchemaInput input in schema.getInputs())
                    {
                        if (input.getNaaccrItem() > 0 && input.getNaaccrXmlId() == null)
                        {
                            errors.Add("Schema input " + schema.getId() + "." + input.getKey() + " has a NAACCR number but is missing NAACCR XML ID");
                        }

                        if (input.getNaaccrXmlId() != null)
                        {
                            if (inputMappings.ContainsKey(input.getNaaccrXmlId()))
                                inputMappings[input.getNaaccrXmlId()].Add(input.getKey());
                            else
                                inputMappings[input.getNaaccrXmlId()] = new HashSet<String>() { input.getKey() };
                        }
                    }

                    foreach (StagingSchemaOutput output in schema.getOutputs())
                    {
                        if (output.getNaaccrItem() > 0 && output.getNaaccrXmlId() == null)
                        {
                            errors.Add("Schema output " + schema.getId() + "." + output.getKey() + " has a NAACCR number but is missing NAACCR XML ID");
                        }

                        if (output.getNaaccrXmlId() != null)
                        {
                            if (outputMappings.ContainsKey(output.getNaaccrXmlId()))
                                outputMappings[output.getNaaccrXmlId()].Add(output.getKey());
                            else
                                outputMappings[output.getNaaccrXmlId()] = new HashSet<String>() { output.getKey() };
                        }
                    }
                }

                // verify that if a field has a given NAACCR XML ID, then all fields with that same XML ID have the same key.
                foreach (KeyValuePair<String, HashSet<String>> entry in inputMappings)
                {
                    if (entry.Value.Count > 1)
                    {
                        errors.Add("NAACCR XML Id " + entry.Key + " is listed for multiple inputs: " + entry.Value);
                    }
                }
                foreach (KeyValuePair<String, HashSet<String>> entry in outputMappings)
                {
                    if (entry.Value.Count > 1)
                    {
                        errors.Add("NAACCR XML Id " + entry.Key + " is listed for multiple outputs: " + entry.Value);
                    }
                }

                //assertThat(errors).overridingErrorMessage(()-> "\n" + String.join("\n", errors)).isEmpty();
                Assert.AreEqual(0, errors.Count);
            }
        }

        // * Helper method to assert failures when tracked errors exist
        // * @param errors
        // * @param description
        public void assertNoErrors(HashSet<String> errors, String description)
        {
            if (errors.Count > 0)
            {
                System.Diagnostics.Trace.WriteLine("There were " + errors.Count + " issues with " + description + ".");
                foreach (String s in errors)
                {
                    System.Diagnostics.Trace.WriteLine(s);
                }
                Assert.Fail();
            }
        }

        // * Return the input length from a specified table
        // * @param tableId table indentifier
        // * @param key input key
        // * @return null if no length couild be determined, or the length
        protected int getInputLength(String tableId, String key)
        {
            StagingTable table = _STAGING.getTable(tableId);
            int length = -1;

            // loop over each row
            foreach (StagingTableRow row in table.getTableRows())
            {
                List<Range> ranges = row.getInputs()[key];

                foreach (StagingRange range in ranges)
                {
                    String low = range.getLow();
                    String high = range.getHigh();

                    if (range.matchesAll() || low.Length == 0)
                        continue;

                    if (low.StartsWith("{{") && low.Contains(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT))
                        low = DateTime.Now.Year.ToString();
                    if (high.StartsWith("{{") && high.Contains(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT))
                        high = DateTime.Now.Year.ToString();

                    if (length >= 0 && (low.Length != length || high.Length != length))
                        throw new System.InvalidOperationException("Inconsistent lengths in table " + tableId + " for key " + key);

                    length = low.Length;
                }
            }

            return length;
        }
    }
}


