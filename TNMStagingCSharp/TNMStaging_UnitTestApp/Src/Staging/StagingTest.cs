using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using static TNMStagingCSharp.Src.Staging.InMemoryDataProvider;

namespace TNMStaging_UnitTestApp.Src.Staging
{
    /**
     * Base class for all algorithm-specific testing
     */
    public abstract class StagingTest
    {
        protected static TNMStagingCSharp.Src.Staging.Staging _STAGING = null;
        protected static StagingDataProvider _PROVIDER = null;
        protected static String _basePath = string.Empty;

        //* Return the algorithm name
        public abstract String getAlgorithm();

        // * Return the algorithm version
        public abstract String getVersion();

        // * Return the staging data provider
        //public abstract StagingFileDataProvider getProvider();

        // Return the full path of specified algorithm
        public static string getAlgorithmPath(String algorithm) 
        {
            string retval = string.Empty;

            String relPath = "\\..\\..\\..\\";
            if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) relPath += "\\..\\";

            _basePath = Path.GetFullPath(System.IO.Directory.GetCurrentDirectory() + relPath);

            String algorithmsDir = _basePath + "Resources\\Test\\algorithms\\";


            //string algorithmsDir = Paths.get(Objects.requireNonNull(Thread.currentThread()
            //    .getContextClassLoader()
            //    .getResource("algorithms")).toURI());

            //try (Stream<Path> files = Files.list(algorithmsDir)) 
            //{
            //    return files
            //        .filter(Files::isRegularFile)
            //        .filter(path -> path.getFileName().toString().startsWith(algorithm + "-"))
            //        .findFirst()
            //        .orElseThrow(() -> new IllegalStateException("No " + algorithm + "  file found in algorithms directory"));
            //}

            string[] files = Directory.GetFiles(algorithmsDir, "*.zip");
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    string thisFilename = Path.GetFileName(file);
                    if (thisFilename.StartsWith(algorithm))
                    {
                        retval = file;
                    }
                }
            }
            if (retval.Length == 0)
            {
                throw new Exception("No " + algorithm + "  file found in algorithms directory");
            }
            return retval;
        }

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
                ITable table = _STAGING.getTable(id);

                Assert.IsNotNull(table);
                Assert.IsNotNull(table.getAlgorithm());
                Assert.IsNotNull(table.getVersion());
                Assert.IsNotNull(table.getName());
            }
        }

        [TestMethod]
        public virtual void testValidCode()
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
                Schema schema = _STAGING.getSchema(id);
                foreach (IInput input in schema.getInputs())
                {
                    Assert.IsNull(input.getUnit(), "No schemas should have units");
                    Assert.IsTrue(input.getDecimalPlaces() == 0, "No schemas should have decimal places");
                }
            }
        }

        [TestMethod]
        public void testInputDefault()
        {
            Dictionary<String, String> context = new Dictionary<String, String>();

            // error conditions
            String schemaId = null;
            HashSet<string> schemas = _STAGING.getSchemaIds();
            if (schemas.Count > 0)
            {
                schemaId = schemas.First();
            }

            Assert.IsNotNull(schemaId);
            Assert.AreEqual(string.Empty, _STAGING.getInputDefault(_STAGING.getSchema(schemaId), "i_do_not_exist", context));

            foreach (String id in _STAGING.getSchemaIds())
            {
                Schema schema = _STAGING.getSchema(id);
                foreach (IInput input in schema.getInputs())
                {
                    if (input.getDefault() != null && input.getDefaultTable() != null)
                        Assert.Fail("In " + getAlgorithm() + ", schema " + schema.getId() + " and input " + input.getKey() + " there is a default and default_table. That is not allowed.");

                    if (input.getDefault() != null)
                        Assert.AreEqual(input.getDefault(), _STAGING.getInputDefault(schema, input.getKey(), context));
                    else if (input.getDefaultTable() != null)
                        Assert.IsFalse(_STAGING.getInputDefault(schema, input.getKey(), context).Length == 0);
                    else
                        Assert.IsTrue(_STAGING.getInputDefault(schema, input.getKey(), context).Length == 0);
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
                if (!usedTables.Contains(id) && !id.StartsWith("conversion_"))
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
                Schema schema = _STAGING.getSchema(schemaId);

                // build a list of input tables that should be excluded
                foreach (IInput input in schema.getInputs())
                {
                    if (input.getTable() != null)
                    {
                        HashSet<String> inputKeys = new HashSet<String>();
                        ITable table = _STAGING.getTable(input.getTable());
                        foreach (IColumnDefinition def in table.getColumnDefinitions())
                            if (ColumnType.INPUT == def.getType())
                                inputKeys.Add(def.getKey());

                        // make sure the input key matches an input column
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
                Schema schema = _STAGING.getSchema(id);

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
                Schema schema = _STAGING.getSchema(schemaId);

                // build a list of input tables that should be excluded
                HashSet<String> ids = new HashSet<String>();
                List<IMapping> mappings = schema.getMappings();
                if (mappings != null)
                    foreach (IMapping mapping in mappings)
                    {
                        if (ids.Contains(mapping.getId()))
                            errors.Add("The mapping id " + schemaId + ":" + mapping.getId() + " is duplicated.  This should never happen");
                        ids.Add(mapping.getId());
                    }
            }

            assertNoErrors(errors, "input values and their assocated validation tables");
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

        // Return the input length from a specified table
        // @param tableId table indentifier
        // @param key input key
        // @return null if no length couild be determined, or the length
        protected int getInputLength(String tableId, String key)
        {
            ITable table = _STAGING.getTable(tableId);
            int length = 0;

            // loop over each row
            foreach (ITableRow row in table.getTableRows())
            {
                foreach (Range range in row.getColumnInput(key))
                {
                    String low = range.getLow();
                    String high = range.getHigh();

                    if (range.matchesAll() || low.Length == 0)
                        continue;

                    if (low.StartsWith("{{") && low.Contains(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT))
                        //low = String.valueOf(Calendar.getInstance().get(Calendar.YEAR));
                        low = DateTime.Now.Year.ToString();
                    if (high.StartsWith("{{") && high.Contains(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT))
                        //high = String.valueOf(Calendar.getInstance().get(Calendar.YEAR));
                        high = DateTime.Now.Year.ToString();

                    if (length > 0 && (low.Length != length || high.Length != length))
                        throw new System.InvalidOperationException("Inconsistent lengths in table " + tableId + " for key " + key);

                    length = low.Length;
                }
            }

            return length;
        }
    }
}
