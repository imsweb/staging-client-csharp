using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStaging_UnitTestApp.Src.DecisionEngine.Basic;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.Staging.Engine
{
    /**
     * Test class for DecisionEngine
     */

    [TestClass]
    public class DecisionEngineTest
    {
        private static DecisionEngineClass _ENGINE;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            BasicDataProvider provider = new BasicDataProvider();

            BasicTable table = new BasicTable("table_lookup_sample");
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addRawRow(new List<String>() { "00-09", "First 10" });
            table.addRawRow(new List<String>() { "10-19", "Second 10" });
            table.addRawRow(new List<String>() { "20-29", "Third 10" });
            table.addRawRow(new List<String>() { "30", "Thirty" });
            table.addRawRow(new List<String>() { "99", "Ninety-nine" });
            table.addRawRow(new List<String>() { "", "blank" });
            provider.addTable(table);

            table = new BasicTable("table_jump_sample");
            table.addColumnDefinition("c", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"A", "Line1", "VALUE:A" });
            table.addRawRow(new List<String>() { "B", "Line2", "VALUE:B" });
            table.addRawRow(new List<String>() {"C", "Line3", "ERROR:Bad C value"});
            table.addRawRow(new List<String>() {"Z", "Line4", "STOP" });
            provider.addTable(table);

            table = new BasicTable("table_sample_first");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1,2,5-9", "00-12", "Line1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() {"3,4", "17,19,22", "Line2", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() {"0", "23-30", "Line3", "VALUE:LINE3" });
            table.addRawRow(new List<String>() {"5", "20", "Line4", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() {"*", "55", "Line5", "VALUE:LINE5" });
            table.addRawRow(new List<String>() {"8", "99", "Line6", "ERROR:" });
            table.addRawRow(new List<String>() {"9", "99", "Line6", "ERROR:999" });
            provider.addTable(table);

            table = new BasicTable("table_multiple_inputs");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("r1", ColumnType.ENDPOINT);
            table.addColumnDefinition("r2", ColumnType.ENDPOINT);
            table.addColumnDefinition("r3", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1,2,5-9", "00-12", "Line1", "VALUE:1_LINE1", "VALUE:2_LINE1", "VALUE:3_LINE1" });
            table.addRawRow(new List<String>() {"0", "23-30", "Line2", "VALUE:1_LINE2", "ERROR:2_LINE2", "VALUE:3_LINE2" });
            table.addRawRow(new List<String>() {"5", "20", "Line3", "JUMP:table_jump_sample", "VALUE:2_LINE3", "VALUE:3_LINE3" });
            table.addRawRow(new List<String>() {"4", "44", "Line5", "JUMP:table_jump_sample", "VALUE:2_LINE5", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() {"9", "99", "Line4", "ERROR:1_LINE4", "ERROR:2_LINE4", "ERROR:3_LINE4" });
            provider.addTable(table);

            table = new BasicTable("table_sample_second");
            table.addColumnDefinition("e", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("shared_result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"X", "Line1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() {"Y", "Line2", "VALUE:LINE1" });
            table.addRawRow(new List<String>() {"Z", "Line3", "VALUE:LINE3" });
            provider.addTable(table);

            table = new BasicTable("table_recursion");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1,2,5-9", "Line1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() {"3,4", "Line2", "JUMP:table_recursion" });
            provider.addTable(table);

            table = new BasicTable("table_inclusion1");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addRawRow(new List<String>() {"0-3,5" });
            provider.addTable(table);

            table = new BasicTable("table_inclusion2");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addRawRow(new List<String>() {"4,6-8" });
            provider.addTable(table);

            table = new BasicTable("table_inclusion3");
            table.addColumnDefinition("not_in_input_list", ColumnType.INPUT);
            table.addRawRow(new List<String>() {"4,6-8" });
            provider.addTable(table);

            table = new BasicTable("table_exclusion1");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addRawRow(new List<String>() {"1" });
            provider.addTable(table);

            table = new BasicTable("table_part1");
            table.addColumnDefinition("val", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1", "1", "VALUE:1" });
            table.addRawRow(new List<String>() {"2", "2", "VALUE:2" });
            table.addRawRow(new List<String>() { "3", "3", "VALUE:3" });
            provider.addTable(table);

            table = new BasicTable("table_part2");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("special", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"*", "", "VALUE:SUCCESS" });
            provider.addTable(table);

            table = new BasicTable("table_create_intermediate");
            table.addColumnDefinition("main_input", ColumnType.INPUT);
            table.addColumnDefinition("intermediate_output", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1", "VALUE:1" });
            provider.addTable(table);

            table = new BasicTable("table_use_intermediate");
            table.addColumnDefinition("intermediate_output", ColumnType.INPUT);
            table.addColumnDefinition("final_output", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1", "VALUE:1" });
            provider.addTable(table);

            table = new BasicTable("table_blank_matching");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.ENDPOINT);
            table.addColumnDefinition("c", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"*", "", "VALUE:LINE1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() {"NA", "*", "VALUE:LINE2", "VALUE:LINE2" });
            table.addRawRow(new List<String>() {"", "*", "VALUE:LINE3", "VALUE:LINE3" });
            table.addRawRow(new List<String>() {"*", "*", "MATCH", "MATCH" });
            provider.addTable(table);

            table = new BasicTable("table_multiple_input");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() {"1", "0", "VALUE:LINE1" });
            table.addRawRow(new List<String>() {"0", "1", "VALUE:LINE2" });
            table.addRawRow(new List<String>() {"1", "1", "VALUE:LINE3" });
            provider.addTable(table);

            BasicDefinition def = new BasicDefinition("starting_sample");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);
            def.addInput("a");
            BasicInput input = new BasicInput("b", "table_lookup_sample");
            def.addInput(input);
            def.addInput("c");
            def.addInitialContext("d", "HARD-CODE");
            BasicMapping mapping = new BasicMapping("m1");
            mapping.setTablePaths(new List<ITablePath>() { new BasicTablePath("table_sample_first"), new BasicTablePath("table_sample_second") });
            def.addMapping(mapping);
            provider.addDefinition(def);

            def = new BasicDefinition("starting_min");
            def.addInitialContext("foo", "bar");
            provider.addDefinition(def);

            def = new BasicDefinition("starting_multiple_endpoints");
            def.addInput("a");
            def.addInput("b");
            def.addInput("c");
            def.addMapping(new BasicMapping("m1", new List<ITablePath>() { new BasicTablePath("table_multiple_inputs") }));
            provider.addDefinition(def);

            def = new BasicDefinition("starting_recursion");
            def.addInput("a");
            def.addMapping(new BasicMapping("m1", new List<ITablePath>() { new BasicTablePath("table_recursion") }));
            provider.addDefinition(def);

            def = new BasicDefinition("starting_inclusions");
            def.addInput("a");
            def.addInput("b");
            def.addInput("c");
            mapping = new BasicMapping("m1");
            mapping.setInclusionTables(new List<ITablePath>() { new BasicTablePath("table_inclusion1") });
            BasicTablePath path = new BasicTablePath("table_part1");
            path.addInputMapping("b", "val");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            mapping = new BasicMapping("m2");
            mapping.setInclusionTables(new List<ITablePath>() { new BasicTablePath("table_inclusion2") });
            path = new BasicTablePath("table_part1");
            path.addInputMapping("c", "val");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            mapping = new BasicMapping("m3");
            mapping.setExclusionTables(new List<ITablePath>() { new BasicTablePath("table_exclusion1") });
            path = new BasicTablePath("table_part2");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            def = new BasicDefinition("starting_intermediate_values");
            def.addInput("main_input");
            mapping = new BasicMapping("m1");
            mapping.addTablePath(new BasicTablePath("table_create_intermediate"));
            mapping.addTablePath(new BasicTablePath("table_use_intermediate"));
            def.addMapping(mapping);
            provider.addDefinition(def);

            def = new BasicDefinition("starting_inclusions_extra_inputs");
            def.addInput("a");
            def.addInput("b");
            def.addInput("c");
            mapping = new BasicMapping("m1");
            mapping.setInclusionTables(new List<ITablePath>() { new BasicTablePath("table_inclusion3") });
            path = new BasicTablePath("table_part1");
            path.addInputMapping("b", "val");
            path.addOutputMapping("result", "mapped_result");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            def = new BasicDefinition("starting_blank");
            def.addInput("a");
            def.addInput("b");
            mapping = new BasicMapping("m1");
            path = new BasicTablePath("table_blank_matching");
            path.addInputMapping("x", "a");
            path.addInputMapping("y", "b");
            path.addOutputMapping("b", "y");
            path.addOutputMapping("c", "z");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            def = new BasicDefinition("starting_double_input");
            def.addInput("x");
            mapping = new BasicMapping("m1");
            path = new BasicTablePath("table_multiple_input");
            path.addInputMapping("x", "a");
            path.addInputMapping("x", "b");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            def = new BasicDefinition("starting_double_output");
            def.addInput("a");
            def.addInput("b");
            mapping = new BasicMapping("m1");
            path = new BasicTablePath("table_sample_first");
            path.addOutputMapping("result", "output1");
            path.addOutputMapping("result", "output2");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            _ENGINE = new DecisionEngineClass(provider);
        }

        [TestMethod]
        public void testMatch()
        {
            List<Range> range = new List<Range>();
            range.Add(new BasicRange("1", "1"));
            range.Add(new BasicRange("4", "4"));
            range.Add(new BasicRange("9", "9"));
            Assert.IsTrue(DecisionEngineFuncs.testMatch(range, "9", new Dictionary<String, String>()));
            Assert.IsFalse(DecisionEngineFuncs.testMatch(range, "7", new Dictionary<String, String>()));

            range = new List<Range>();
            range.Add(new BasicRange("11", "54"));
            range.Add(new BasicRange("99", "99"));
            Assert.IsTrue(DecisionEngineFuncs.testMatch(range, "23", new Dictionary<String, String>()));
        }

        [TestMethod]
        public void testEmptyTable() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("basic_test_table");
            table.addColumnDefinition("size", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("size_result", ColumnType.ENDPOINT);
            provider.addTable(table);

            table = (BasicTable)provider.getTable("basic_test_table");
            Assert.IsTrue(table.getRawRows().Count == 0);
            Assert.IsTrue(table.getTableRows().Count == 0);
        }

        [TestMethod]
        public void testMatchTable() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("basic_test_table");
            table.addColumnDefinition("size", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("size_result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "001-003,004,006", "Little", "JUMP:some_crazy_table" });
            table.addRawRow(new List<String>() { "007-019,022", "Medium", "VALUE:medium_stuff" });
            table.addRawRow(new List<String>() { "076-099,999", "HUGE!", "ERROR:Get that huge stuff out of here!" });
            table.addRawRow(new List<String>() { "030", "Match endpoint", "MATCH" });
            provider.addTable(table);

            ITable matchTable = provider.getTable("basic_test_table");
            Assert.IsNotNull(matchTable);

            // create context of input fields
            Dictionary<String, String> input = new Dictionary<String, String>();

            // first try it with missing input
            Assert.IsNull(DecisionEngineFuncs.matchTable(matchTable, input));

            input["size"] = "003";
            List<IEndpoint> lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.JUMP, "some_crazy_table", "size_result") };
            List<IEndpoint> lst2 = DecisionEngineFuncs.matchTable(matchTable, input);
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));

            input["size"] = "014";
            List<IEndpoint> results = DecisionEngineFuncs.matchTable(matchTable, input);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(EndpointType.VALUE, results[0].getType());
            Assert.AreEqual("medium_stuff", results[0].getValue());
            Assert.AreEqual("size_result", results[0].getResultKey());

            input["size"] = "086";
            lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.ERROR, "Get that huge stuff out of here!", "size_result") };
            lst2 = DecisionEngineFuncs.matchTable(matchTable, input);
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));

            // try with a value not in the table
            input["size"] = "021";
            Assert.IsNull(DecisionEngineFuncs.matchTable(matchTable, input));
        }

        [TestMethod]
        public void testMatchTableWithBlankOrMissingInput() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("basic_test_table");
            table.addColumnDefinition("size", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "", "BLANK", "MATCH" });
            table.addRawRow(new List<String>() { "1", "ONE", "MATCH" });
            table.addRawRow(new List<String>() { "2", "TWO", "MATCH" });
            provider.addTable(table);

            ITable matchTable = provider.getTable("basic_test_table");
            Assert.IsNotNull(matchTable);

            // create context of input fields
            Dictionary<String, String> input = new Dictionary<String, String>();

            // first try it with missing input (null should match just like blank))
            Assert.IsNotNull(DecisionEngineFuncs.matchTable(matchTable, input));

            // now add blank input
            input["size"] = "";
            Assert.IsNotNull(DecisionEngineFuncs.matchTable(matchTable, input));

            // test matching on multiple mising values
            table = new BasicTable("basic_test_table_multi");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("c", ColumnType.INPUT);
            table.addColumnDefinition("d", ColumnType.INPUT);
            table.addColumnDefinition("e", ColumnType.INPUT);
            table.addRawRow(new List<String>() { "1", "", "", "", "" });
            table.addRawRow(new List<String>() { "2", "", "", "", "" });
            provider.addTable(table);

            matchTable = provider.getTable("basic_test_table_multi");
            Assert.IsNotNull(matchTable);

            // first try it with missing input (null should match just like blank))
            Assert.IsNull(DecisionEngineFuncs.matchTable(matchTable, new Dictionary<String, String>()));

            input["a"] = "2";
            Assert.IsNotNull(DecisionEngineFuncs.matchTable(matchTable, input));
        }

        [TestMethod]
        public void testMatchOnSpecificKeys()
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("basic_test_table_keytest");
            table.addColumnDefinition("key1", ColumnType.INPUT);
            table.addColumnDefinition("key2", ColumnType.INPUT);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "001-099", "1-3", "MATCH:LINE1" });
            table.addRawRow(new List<String>() { "001-099", "5-9", "MATCH:LINE2" });
            table.addRawRow(new List<String>() { "100,106", "1", "MATCH:LINE3" });
            provider.addTable(table);

            ITable matchTable = provider.getTable("basic_test_table_keytest");
            Assert.IsNotNull(matchTable);

            // create context of input fields
            Dictionary<String, String> input = new Dictionary<String, String>();

            // first try it with missing input
            Assert.IsNull(DecisionEngineFuncs.matchTable(matchTable, input));

            // if searching all keys and only supplying key1, not match will be found
            input["key1"] = "050";
            Assert.IsNull(DecisionEngineFuncs.matchTable(matchTable, input));

            // specify to only match on key1, there should be a match to the first line
            List<IEndpoint> lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.MATCH, "LINE1", "result") };
            List<IEndpoint> lst2 = DecisionEngineFuncs.matchTable(matchTable, input, new HashSet<String>() { "key1" });
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));

            // add key2 to the input map and there should be a match
            input["key2"] = "7";
            lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.MATCH, "LINE2", "result") };
            lst2 = DecisionEngineFuncs.matchTable(matchTable, input);
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));

            // if searching on key1 only, even though key2 was supplied should still match to first line
            lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.MATCH, "LINE1", "result") };
            lst2 = DecisionEngineFuncs.matchTable(matchTable, input, new HashSet<String>() { "key1" });
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));

            // supply an empty set of keys (the same meaning as not passing any keys
            lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.MATCH, "LINE1", "result") };
            lst2 = DecisionEngineFuncs.matchTable(matchTable, input, new HashSet<String>());
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));

            // supply an invalid key.  I think this should find nothing, but for the moment finds a match to the first row since none of the cells were compared to.  It
            // is the same as matching to a table with no INPUTS which would currently find a match to the first row.
            lst1 = new List<IEndpoint>() { new BasicEndpoint(EndpointType.MATCH, "LINE1", "result") };
            lst2 = DecisionEngineFuncs.matchTable(matchTable, input, new HashSet<String>() { "bad_key" });
            Assert.IsTrue(CompareListsOfBasicEndpoint(lst1, lst2));
        }

        public bool CompareListsOfBasicEndpoint(List<IEndpoint> lst1, List<IEndpoint> lst2)
        {
            bool bRetval = true;

            if (lst1.Count != lst2.Count)
            {
                bRetval = false;
            }
            else
            {
                BasicEndpoint endpoint1 = null;
                BasicEndpoint endpoint2 = null;
                for (int i=0; (i < lst1.Count) && (bRetval); i++)
                {
                    endpoint1 = (BasicEndpoint)lst1[i];
                    endpoint2 = (BasicEndpoint)lst2[i];

                    if ((endpoint1.getType() != endpoint2.getType()) ||
                        (endpoint1.getValue() != endpoint2.getValue()) ||
                        (endpoint1.getResultKey() != endpoint2.getResultKey()))
                    {
                        bRetval = false;
                    }

                }
            }

            return bRetval;
        }

        [TestMethod]
        public void testMatchMissingVsAll()
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("testMissingAndAll");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "", "VALUE:missing" });
            table.addRawRow(new List<String>() { "*", "VALUE:all" });
            provider.addTable(table);

            ITable tableMissing = provider.getTable("testMissingAndAll");

            List<IEndpoint> endpoints;
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "";
            endpoints = DecisionEngineFuncs.matchTable(tableMissing, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("missing", endpoints[0].getValue());

            input["a"] = "1";
            endpoints = DecisionEngineFuncs.matchTable(tableMissing, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("all", endpoints[0].getValue());
        }

        [TestMethod]
        public void testValueKeyReferences()
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("table_key_references");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "1", "*", "VALUE:b" });
            table.addRawRow(new List<String>() { "2", "*", "VALUE:{b}" });
            table.addRawRow(new List<String>() { "3", "*", "VALUE:{{b" });
            table.addRawRow(new List<String>() { "4", "*", "VALUE:b}}" });
            table.addRawRow(new List<String>() { "10", "*", "VALUE:{{b}}" });
            table.addRawRow(new List<String>() { "11", "*", "VALUE:{{bad_key}}" });
            provider.addTable(table);
            BasicDefinition def = new BasicDefinition("alg_key_references");
            def.addInput("a");
            def.addInput("b");
            def.addMapping(new BasicMapping("m1", new List<ITablePath> { new BasicTablePath("table_key_references") }));
            provider.addDefinition(def);
            DecisionEngineClass engine = new DecisionEngineClass(provider);

            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "1";
            input["b"] = "B VALUE";
            Assert.IsFalse(engine.process("alg_key_references", input).hasErrors());
            Assert.AreEqual("b", input["result"]);

            input["a"] = "2";
            Assert.IsFalse(engine.process("alg_key_references", input).hasErrors());
            Assert.AreEqual("{b}", input["result"]);

            input["a"] = "3";
            Assert.IsFalse(engine.process("alg_key_references", input).hasErrors());
            Assert.AreEqual("{{b", input["result"]);

            input["a"] = "4";
            Assert.IsFalse(engine.process("alg_key_references", input).hasErrors());
            Assert.AreEqual("b}}", input["result"]);

            input["a"] = "10";
            Assert.IsFalse(engine.process("alg_key_references", input).hasErrors());
            Assert.AreEqual("B VALUE", input["result"]);

            input["a"] = "11";
            Assert.IsFalse(engine.process("alg_key_references", input).hasErrors());
            Assert.AreEqual("", input["result"]);
        }

        [TestMethod]
        public void testMatchTableMissingCell() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("table_sample_first");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "1,2,5-9", "00-12", "Line1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() { "3,4", "17,19,22", "Line2", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() { "0", "23-30", "Line3", "VALUE:LINE3" });
            table.addRawRow(new List<String>() { "5", "20", "Line4", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() { "*", "55", "Line5", "VALUE:LINE5" });
            table.addRawRow(new List<String>() { "9", "99", "Line6", "ERROR:999" });
            provider.addTable(table);

            ITable tableSample = provider.getTable("table_sample_first");
            Assert.IsNotNull(tableSample);

            // first test with no "a"
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["b"] = "55";
            List<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(tableSample, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("LINE5", endpoints[0].getValue());

            // then test with a random "a"
            input["a"] = "982";
            endpoints = DecisionEngineFuncs.matchTable(tableSample, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("LINE5", endpoints[0].getValue());
        }

        [TestMethod]
        public void testBlankMatching()
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("table_blank_matching");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.ENDPOINT);
            table.addColumnDefinition("c", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "*", "", "VALUE:LINE1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() { "NA", "*", "VALUE:LINE2", "VALUE:LINE2" });
            table.addRawRow(new List<String>() { "", "*", "VALUE:LINE3", "VALUE:LINE3" });
            table.addRawRow(new List<String>() { "*", "*", "MATCH", "MATCH" });
            provider.addTable(table);

            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "X";
            input["b"] = "";
            List<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(table, input);
            Assert.AreEqual(2, endpoints.Count);
            Assert.AreEqual("LINE1", endpoints[0].getValue());
            Assert.AreEqual("LINE1", endpoints[1].getValue());

            input.Clear();
            input["a"] = "NA";
            input["b"] = "99";
            endpoints = DecisionEngineFuncs.matchTable(table, input);
            Assert.AreEqual(2, endpoints.Count);
            Assert.AreEqual("LINE2", endpoints[0].getValue());
            Assert.AreEqual("LINE2", endpoints[1].getValue());
        }

        [TestMethod]
        public void testLookupTable() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("site_table");
            table.addColumnDefinition("site", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addRawRow(new List<String>() { "C000", "External upper lip" });
            table.addRawRow(new List<String>() { "C001", "External lower lip" });
            table.addRawRow(new List<String>() { "C002", "External lip, NOS" });
            table.addRawRow(new List<String>() { "C003", "Inner aspect, upper lip" });
            table.addRawRow(new List<String>() { "C004", "Inner aspect, lower lip" });
            table.addRawRow(new List<String>() { "C005", "Inner aspect of lip, NOS" });
            table.addRawRow(new List<String>() { "C006", "Commissure of lip" });
            table.addRawRow(new List<String>() { "C008", "Overlapping lesion of lip" });
            table.addRawRow(new List<String>() { "C009", "Lip, NOS" });
            table.addRawRow(new List<String>() { "C809", "Unknown primary site" });
            provider.addTable(table);

            ITable siteTable = provider.getTable("site_table");
            Assert.IsNotNull(siteTable);

            Dictionary<String, String> input = new Dictionary<String, String>();
            input["site"] = "C809";

            // a lookup table in this case has no ENDPOINT column.  In those cases, an endpoint type of MATCH should be returned
            List<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(siteTable, input);
            Assert.AreEqual(0, endpoints.Count);
        }

        [TestMethod]
        public void testAllValuesMatching() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicTable table = new BasicTable("all_values_test");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "1", "1", "", "VALUE:RESULT0" });
            table.addRawRow(new List<String>() { "1", "2", "", "VALUE:RESULT1" });
            table.addRawRow(new List<String>() { "1", "3", "", "VALUE:RESULT2" });
            table.addRawRow(new List<String>() { "2", "1", "", "VALUE:RESULT3" });
            table.addRawRow(new List<String>() { "2", "2", "", "VALUE:RESULT4" });
            table.addRawRow(new List<String>() { "2", "3", "", "VALUE:RESULT5" });
            table.addRawRow(new List<String>() { "3", "*", "", "VALUE:3A,ANY B" });
            table.addRawRow(new List<String>() { "*", "4", "", "VALUE:ANY A,4B" });
            table.addRawRow(new List<String>() { "*", "*", "", "VALUE:CATCHALL" });
            provider.addTable(table);

            ITable allValuesTable = provider.getTable("all_values_test");
            Assert.IsNotNull(table);

            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "1";
            input["b"] = "3";
            List<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(allValuesTable, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("RESULT2", endpoints[0].getValue());

            input = new Dictionary<String, String>();
            input["a"] = "3";
            endpoints = DecisionEngineFuncs.matchTable(allValuesTable, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("3A,ANY B", endpoints[0].getValue());

            input = new Dictionary<String, String>();
            input["a"] = "3";
            input["b"] = "9";
            endpoints = DecisionEngineFuncs.matchTable(allValuesTable, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("3A,ANY B", endpoints[0].getValue());

            input = new Dictionary<String, String>();
            input["a"] = "6";
            input["b"] = "4";
            endpoints = DecisionEngineFuncs.matchTable(allValuesTable, input);
            Assert.AreEqual(1, endpoints.Count);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("ANY A,4B", endpoints[0].getValue());

            input = new Dictionary<String, String>();
            Assert.AreEqual(1, endpoints.Count);
            endpoints = DecisionEngineFuncs.matchTable(allValuesTable, input);
            Assert.AreEqual(EndpointType.VALUE, endpoints[0].getType());
            Assert.AreEqual("CATCHALL", endpoints[0].getValue());
        }

        [TestMethod]
        public void testMinimumAlgorithm()
        {
            IDefinition minDefinition = _ENGINE.getProvider().getDefinition("starting_min");
            Assert.IsNotNull(minDefinition);
            Assert.AreEqual("starting_min", minDefinition.getId());
            Assert.IsNotNull(minDefinition.getInitialContext());

            Dictionary<String, String> input = new Dictionary<String, String>();
            Result result = _ENGINE.process(minDefinition, input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual("bar", input["foo"]);
        }

        [TestMethod]
        public void testAlgorithm()
        {
            IDefinition starting = _ENGINE.getProvider().getDefinition("starting_sample");
            Assert.IsNotNull(starting);
            Assert.AreEqual("starting_sample", starting.getId());
            Assert.IsNotNull(starting.getInitialContext());
        }

        [TestMethod]
        public void testMissingParameters()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            Result result = _ENGINE.process("starting_sample", input);

            Assert.IsTrue(result.hasErrors());

            // even though there were no inputs, both tables were still processed
            Assert.AreEqual(2, result.getPath().Count);
            Assert.AreEqual("m1.table_sample_first", result.getPath()[0]);
            Assert.AreEqual("m1.table_sample_second", result.getPath()[1]);
        }

        [TestMethod]
        public void testParameterLookupValidation()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "3";
            input["b"] = "31";  // value is not in lookup table

            Result result = _ENGINE.process("starting_sample", input);

            Assert.AreEqual(Result.Type.FAILED_INPUT, result.getType());

            // since input "b" is fail_on_invalud, table processing should not continue
            Assert.AreEqual(0, result.getPath().Count);

            // one error for input, and one error each of the two tables because of no match
            Assert.AreEqual(1, result.getErrors().Count);

            // make "b" a valid value
            input["b"] = "30";

            result = _ENGINE.process("starting_sample", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());

            // one error for input, and one error each of the two tables because of no match
            Assert.AreEqual(2, result.getErrors().Count);
        }

        [TestMethod]
        public void testSingleTableProcess()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "7";
            input["b"] = "03";  // should map to "hemeretic" without using second table
            input["e"] = "X";
            Result result = _ENGINE.process("starting_sample", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(2, result.getPath().Count);

            Assert.AreEqual("LINE1", input["result"]);
            Assert.AreEqual("HARD-CODE", input["d"]);
        }

        [TestMethod]
        public void testOneJumpProcess()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "5";
            input["b"] = "20";
            input["c"] = "A";
            input["e"] = "X";
            Result result = _ENGINE.process("starting_sample", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(3, result.getPath().Count);

            Assert.AreEqual("A", input["result"]);

            // now test an error line in the jump table
            input = new Dictionary<String, String>();
            input["a"] = "5";
            input["b"] = "20";
            input["c"] = "C";
            input["e"] = "X";

            result = _ENGINE.process("starting_sample", input);

            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(3, result.getPath().Count);
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.AreEqual("Bad C value", result.getErrors()[0].getMessage());
            Assert.IsNull(result.getErrors()[0].getKey());
            Assert.AreEqual("table_jump_sample", result.getErrors()[0].getTable());

            // finally test that no match is found in the jump table
            input = new Dictionary<String, String>();
            input["a"] = "5";
            input["b"] = "20";
            input["c"] = "D";
            input["e"] = "X";

            result = _ENGINE.process("starting_sample", input);

            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(3, result.getPath().Count);
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.IsTrue(result.getErrors()[0].getMessage().StartsWith("Match not found"));
            Assert.IsNull(result.getErrors()[0].getKey());
            Assert.AreEqual("table_jump_sample", result.getErrors()[0].getTable());
        }

        [TestMethod]
        public void testProcessError()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "9";
            input["b"] = "99";
            input["e"] = "X";
            Result result = _ENGINE.process("starting_sample", input);

            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.AreEqual(2, result.getPath().Count);
            Assert.AreEqual("999", result.getErrors()[0].getMessage());
            Assert.IsNull(result.getErrors()[0].getKey());
            Assert.AreEqual("table_sample_first", result.getErrors()[0].getTable());
            CollectionAssert.AreEqual(new List<String> { "result" }, result.getErrors()[0].getColumns());

            // test case with generated error message (i.e. the column is "ERROR:" without a message
            input["a"] = "8";
            input["b"] = "99";
            input["e"] = "X";
            result = _ENGINE.process("starting_sample", input);
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.AreEqual(2, result.getPath().Count);
            Assert.AreEqual("Matching resulted in an error in table 'table_sample_first' for column 'result' (8,99)", result.getErrors()[0].getMessage());
            Assert.IsNull(result.getErrors()[0].getKey());
            Assert.AreEqual("table_sample_first", result.getErrors()[0].getTable());
            CollectionAssert.AreEqual(new List<String> { "result" }, result.getErrors()[0].getColumns());
        }

        [TestMethod]
        public void testProcessWithNullValues()
        {
            BasicDataProvider provider = new BasicDataProvider();

            BasicTable table = new BasicTable("table_null_values");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "1", "VALUE:FOUND1" });
            table.addRawRow(new List<String>() { "2", "VALUE" });
            table.addRawRow(new List<String>() { "3", "VALUE:" });
            table.addRawRow(new List<String>() { "*", "MATCH" });
            provider.addTable(table);

            BasicDefinition def = new BasicDefinition("starting_null_values");
            BasicInput inputKey = new BasicInput("a");
            inputKey.setDefault("0");
            def.addInput(inputKey);
            def.addInitialContext("result", "0");
            def.addMapping(new BasicMapping("m1", new List<ITablePath>() { new BasicTablePath("table_null_values") }));
            provider.addDefinition(def);
            DecisionEngineClass engine = new DecisionEngineClass(provider);

            Dictionary<String, String> input = new Dictionary<String, String>();
            Result result = engine.process("starting_null_values", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual("0", input["result"]);

            input.Clear();
            input["a"] = "1";
            result = engine.process("starting_null_values", input);
            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual("FOUND1", input["result"]);

            input.Clear();
            input["a"] = "2";
            result = engine.process("starting_null_values", input);
            Assert.IsFalse(result.hasErrors());
            Assert.IsFalse(input.ContainsKey("result"));

            input.Clear();
            input["a"] = "3";
            result = engine.process("starting_null_values", input);
            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual("", input["result"]);
        }

        [TestMethod]
        public void testProcessWithStop()
        {
            // first test that we get a result from the second table
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "5";
            input["b"] = "20";
            input["c"] = "A";
            input["e"] = "X";
            Result result = _ENGINE.process("starting_sample", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(3, result.getPath().Count);

            Assert.AreEqual("A", input["result"]);
            Assert.AreEqual("LINE1", input["shared_result"]);

            // next text then when STOP is encountered, the second table is not processed
            input.Clear();
            input["a"] = "5";
            input["b"] = "20";
            input["c"] = "Z";
            input["e"] = "X";
            result = _ENGINE.process("starting_sample", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(2, result.getPath().Count);

            Assert.IsFalse(input.ContainsKey("result"));
            Assert.IsFalse(input.ContainsKey("shared_result"));
        }

        [TestMethod]
        public void testProcessWithBlanks()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["x"] = "1";
            input["y"] = "";
            Result result = _ENGINE.process("starting_blank", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(1, result.getPath().Count);

            Assert.AreEqual("LINE1", input["y"]);
            Assert.AreEqual("LINE1", input["z"]);

            // verify that context blanks are trimmed
            input["x"] = "1";
            input["y"] = "  ";
            result = _ENGINE.process("starting_blank", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(1, result.getPath().Count);

            Assert.AreEqual("LINE1", input["y"]);
            Assert.AreEqual("LINE1", input["z"]);
        }

        [TestMethod]
        public void testProcessWithDoubleInputMapping()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["x"] = "1";
            Result result = _ENGINE.process("starting_double_input", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(1, result.getPath().Count);

            Assert.AreEqual("LINE3", input["result"]);
        }

        [TestMethod]
        public void testProcessWithDoubleOutputMapping()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "1";
            input["b"] = "00";
            Result result = _ENGINE.process("starting_double_output", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(1, result.getPath().Count);

            String newValue = "";
            Assert.IsFalse(input.TryGetValue("result", out newValue));
            Assert.AreEqual("LINE1", input["output1"]);
            Assert.AreEqual("LINE1", input["output2"]);
        }

        [TestMethod]
        public void testInvolvedAlgorithms()
        {
            List<IMapping> mappings;
            Dictionary<String, String> context = new Dictionary<String, String>();

            IDefinition definition = _ENGINE.getProvider().getDefinition("starting_min");
            mappings = _ENGINE.getInvolvedMappings(definition, context);
            Assert.AreEqual(0, mappings.Count);

            definition = _ENGINE.getProvider().getDefinition("starting_inclusions");

            mappings = _ENGINE.getInvolvedMappings(definition, context);
            Assert.AreEqual(1, mappings.Count);

            context["a"] = "1";
            mappings = _ENGINE.getInvolvedMappings(definition, context);
            Assert.AreEqual(1, mappings.Count);

            context["a"] = "2";
            mappings = _ENGINE.getInvolvedMappings(definition, context);
            Assert.AreEqual(2, mappings.Count);
        }

        [TestMethod]
        public void testInvolvedTables()
        {
            HashSet<String> tables;

            // test a case with no involved tables
            tables = _ENGINE.getInvolvedTables("starting_min");
            Assert.AreEqual(0, tables.Count);

            // test a case with a single table with one jump
            tables = _ENGINE.getInvolvedTables("starting_sample");
            Assert.AreEqual(4, tables.Count);
            Assert.IsTrue(tables.Contains("table_lookup_sample"));
            Assert.IsTrue(tables.Contains("table_sample_first"));
            Assert.IsTrue(tables.Contains("table_sample_second"));
            Assert.IsTrue(tables.Contains("table_jump_sample"));

            // test a case with inclusion/exclusion tables
            tables = _ENGINE.getInvolvedTables("starting_inclusions");
            Assert.AreEqual(5, tables.Count);
            Assert.IsTrue(tables.Contains("table_part1"));
            Assert.IsTrue(tables.Contains("table_part2"));
            Assert.IsTrue(tables.Contains("table_inclusion1"));
            Assert.IsTrue(tables.Contains("table_inclusion2"));
            Assert.IsTrue(tables.Contains("table_exclusion1"));
        }

        [TestMethod]
        public void testInvolvedTablesWhenEmpty()
        {
            BasicDataProvider provider = new BasicDataProvider();

            // add empty table
            BasicTable table = new BasicTable("table1");
            table.addColumnDefinition("a", ColumnType.INPUT);
            provider.addTable(table);

            // add another empty table
            table = new BasicTable("table2");
            table.addColumnDefinition("b", ColumnType.INPUT);
            provider.addTable(table);

            BasicDefinition def = new BasicDefinition("def1");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);
            def.addInput("a");
            def.addInput("b");
            BasicMapping mapping = new BasicMapping("m1");
            mapping.setTablePaths(new List<ITablePath>() { new BasicTablePath("table1"), new BasicTablePath("table2") });
            def.addMapping(mapping);
            provider.addDefinition(def);

            DecisionEngineClass engine = new DecisionEngineClass(provider);

            HashSet<String> tables = engine.getInvolvedTables("def1");
            Assert.AreEqual(2, tables.Count);
            Assert.IsTrue(tables.Contains("table1"));
            Assert.IsTrue(tables.Contains("table2"));
        }

        [TestMethod]
        public void testInvolvedTableRecursion()
        {
            HashSet<String> tables = _ENGINE.getInvolvedTables("starting_recursion");

            Assert.AreEqual(1, tables.Count);
            Assert.IsTrue(tables.Contains("table_recursion"));
        }

        [TestMethod]
        public void testProcessWithRecursion()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "4";
            Result result = _ENGINE.process("starting_recursion", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.AreEqual(1, result.getPath().Count);
            Assert.AreEqual("table_recursion", result.getErrors()[0].getTable());
            Assert.IsNull(result.getErrors()[0].getColumns());
        }

        [TestMethod]
        public void testProcessWithMultipleEndpoints()
        {
            // first test all 3 results get set
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "2";
            input["b"] = "12";
            Result result = _ENGINE.process("starting_multiple_endpoints", input);

            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.AreEqual(1, result.getPath().Count);

            Assert.AreEqual("1_LINE1", input["r1"]);
            Assert.AreEqual("2_LINE1", input["r2"]);
            Assert.AreEqual("3_LINE1", input["r3"]);

            // test 2 VALUEs and an ERROR
            input.Clear();
            input["a"] = "0";
            input["b"] = "25";
            result = _ENGINE.process("starting_multiple_endpoints", input);
            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.AreEqual("table_multiple_inputs", result.getErrors()[0].getTable());
            CollectionAssert.AreEqual(new List<String> { "r2" }, result.getErrors()[0].getColumns());
            Assert.AreEqual("2_LINE2", result.getErrors()[0].getMessage());
            Assert.AreEqual(1, result.getPath().Count);

            Assert.AreEqual("1_LINE2", input["r1"]);
            Assert.IsFalse(input.ContainsKey("r2"));
            Assert.AreEqual("3_LINE2", input["r3"]);

            // test 2 JUMPs and one VALUE and a missing jump table value
            input.Clear();
            input["a"] = "5";
            input["b"] = "20";
            result = _ENGINE.process("starting_multiple_endpoints", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsTrue(result.hasErrors());
            Assert.IsTrue(result.getErrors()[0].getMessage().StartsWith("Match not found"));
            Assert.IsNull(result.getErrors()[0].getKey());
            Assert.AreEqual("table_jump_sample", result.getErrors()[0].getTable());
            CollectionAssert.AreEqual(new List<String> { "result" }, result.getErrors()[0].getColumns());

            // test 1 JUMP and 2 VALUEs
            input.Clear();
            input["a"] = "5";
            input["b"] = "20";
            input["c"] = "A";
            result = _ENGINE.process("starting_multiple_endpoints", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsFalse(result.hasErrors());
            Assert.IsFalse(input.ContainsKey("r1"));
            Assert.AreEqual("A", input["result"]);
            Assert.AreEqual("2_LINE3", input["r2"]);
            Assert.AreEqual("3_LINE3", input["r3"]);

            // test 3 ERRORs
            input.Clear();
            input["a"] = "9";
            input["b"] = "99";
            result = _ENGINE.process("starting_multiple_endpoints", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(3, result.getErrors().Count);
            Assert.AreEqual("1_LINE4", result.getErrors()[0].getMessage());
            Assert.IsNull(result.getErrors()[0].getKey());
            Assert.AreEqual("2_LINE4", result.getErrors()[1].getMessage());
            Assert.IsNull(result.getErrors()[1].getKey());
            Assert.AreEqual("3_LINE4", result.getErrors()[2].getMessage());
            Assert.IsNull(result.getErrors()[2].getKey());
            Assert.IsFalse(input.ContainsKey("r1"));
            Assert.IsFalse(input.ContainsKey("r2"));
            Assert.IsFalse(input.ContainsKey("r3"));
        }

        [TestMethod]
        public void testRowNotFoundWithMultipleOutputs()
        {
            BasicDataProvider provider = new BasicDataProvider();

            // test a situation where a a row with mulitple inputs is not found
            BasicTable table = new BasicTable("table_input");
            table.addColumnDefinition("input1", ColumnType.INPUT);
            table.addColumnDefinition("output1", ColumnType.ENDPOINT);
            table.addColumnDefinition("output2", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "000", "VALUE:000", "VALUE:000" });
            table.addRawRow(new List<String>() { "001", "VALUE:001", "VALUE:001" });
            provider.addTable(table);

            BasicDefinition def = new BasicDefinition("sample_outputs");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);
            def.addInput(new BasicInput("input1", "table_input"));

            BasicMapping mapping = new BasicMapping("mapping1");
            BasicTablePath path = new BasicTablePath("table_input");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            DecisionEngineClass engine = new DecisionEngineClass(provider);

            // test match not found
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "4";
            input["b"] = "55";
            Result result = engine.process("sample_outputs", input);
            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(1, result.getErrors().Count);
            Assert.AreEqual("table_input", result.getErrors()[0].getTable());
            CollectionAssert.AreEqual(new List<String> { "output1", "output2" }, result.getErrors()[0].getColumns());
        }

        [TestMethod]
        public void testMultipleEndpointWithoutRecursion()
        {
            // test 2 JUMPs to same table and one value; this is not infinite recursion but since same table called twice it gets confused
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "4";
            input["b"] = "44";
            input["c"] = "A";
            Result result = _ENGINE.process("starting_multiple_endpoints", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsFalse(result.hasErrors());
            Assert.IsFalse(input.ContainsKey("r1"));
            Assert.IsFalse(input.ContainsKey("r3"));
            Assert.AreEqual("A", input["result"]);
            Assert.AreEqual("2_LINE5", input["r2"]);
        }

        [TestMethod]
        public void testInclusionsAndExclusions()
        {
            Dictionary<String, String> input = new Dictionary<String, String>();
            input["a"] = "1";
            input["b"] = "2";
            input["c"] = "3";
            Result result = _ENGINE.process("starting_inclusions", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual("2", input["result"]);
            Assert.IsFalse(input.ContainsKey("special"));

            input.Clear();
            input["a"] = "8";
            input["b"] = "2";
            input["c"] = "3";
            result = _ENGINE.process("starting_inclusions", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsFalse(result.hasErrors());
            Assert.AreEqual("3", input["result"]);
            Assert.AreEqual("SUCCESS", input["special"]);

            input.Clear();
            input["a"] = "9";
            input["b"] = "2";
            input["c"] = "3";
            result = _ENGINE.process("starting_inclusions", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsFalse(result.hasErrors());
            Assert.IsFalse(input.ContainsKey("result"));
            Assert.AreEqual("SUCCESS", input["special"]);
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void testDuplicateAlgorithms() 
        {
            BasicDataProvider provider = new BasicDataProvider();
            BasicDefinition def = new BasicDefinition();
            def.setId("TEST1");
            provider.addDefinition(def);
            provider.addDefinition(def);
        }

        [TestMethod]
        public void testAlgorithmInputs()
        {
            IDataProvider provider = _ENGINE.getProvider();

            HashSet<String> hash1 = new HashSet<String>() { };
            HashSet<String> hash2 = _ENGINE.getInputs(provider.getDefinition("starting_min"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "a", "b", "c", "e" };
            hash2 = _ENGINE.getInputs(provider.getDefinition("starting_sample"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "a", "b", "c" };
            hash2 = _ENGINE.getInputs(provider.getDefinition("starting_inclusions"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "a" };
            hash2 = _ENGINE.getInputs(provider.getDefinition("starting_recursion"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "a", "b", "c" };
            hash2 = _ENGINE.getInputs(provider.getDefinition("starting_multiple_endpoints"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "b", "not_in_input_list" };
            hash2 = _ENGINE.getInputs(provider.getDefinition("starting_inclusions_extra_inputs"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "main_input" };
            hash2 = _ENGINE.getInputs(provider.getDefinition("starting_intermediate_values"));
            Assert.IsTrue(hash1.SetEquals(hash2));
        }

        [TestMethod]
        public void testGetAlgorithmOutputs()
        {
            IDataProvider provider = _ENGINE.getProvider();

            HashSet<String> hash1 = new HashSet<String>() { };
            HashSet<String> hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_min"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "result", "shared_result" };
            hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_sample"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "result", "special" };
            hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_inclusions"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "result" };
            hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_recursion"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "result", "r1", "r2", "r3" };
            hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_multiple_endpoints"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "mapped_result" };
            hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_inclusions_extra_inputs"));
            Assert.IsTrue(hash1.SetEquals(hash2));

            hash1 = new HashSet<String>() { "intermediate_output", "final_output" };
            hash2 = _ENGINE.getOutputs(provider.getDefinition("starting_intermediate_values"));
            Assert.IsTrue(hash1.SetEquals(hash2));
        }

        [TestMethod]
        public void testGetTableInputsAsString()
        {
            BasicTable table = new BasicTable("table_inputs");
            table.addColumnDefinition("a", ColumnType.INPUT);
            table.addColumnDefinition("b", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addColumnDefinition("result", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "1,2,5-9", "00-12", "Line1", "VALUE:LINE1" });
            table.addRawRow(new List<String>() { "3,4", "17,19,22", "Line2", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() { "0", "23-30", "Line3", "VALUE:LINE3" });
            table.addRawRow(new List<String>() { "5", "20", "Line4", "JUMP:table_jump_sample" });
            table.addRawRow(new List<String>() { "*", "55", "Line5", "VALUE:LINE5" });
            table.addRawRow(new List<String>() { "9", "99", "Line6", "ERROR:999" });

            Dictionary<String, String> context = new Dictionary<String, String>();

            Assert.AreEqual(DecisionEngineFuncs._BLANK_OUTPUT + "," + DecisionEngineFuncs._BLANK_OUTPUT, DecisionEngineFuncs.getTableInputsAsString(table, context));

            context["b"] = "25";
            Assert.AreEqual(DecisionEngineFuncs._BLANK_OUTPUT + ",25", DecisionEngineFuncs.getTableInputsAsString(table, context));

            context["a"] = "7";
            Assert.AreEqual("7,25", DecisionEngineFuncs.getTableInputsAsString(table, context));
            context["a"] = "    7";
            Assert.AreEqual("7,25", DecisionEngineFuncs.getTableInputsAsString(table, context));
            context["a"] = "7    ";
            Assert.AreEqual("7,25", DecisionEngineFuncs.getTableInputsAsString(table, context));

            table = new BasicTable("table_empty");
            context = new Dictionary<String, String>();
            Assert.AreEqual("", DecisionEngineFuncs.getTableInputsAsString(table, context));
        }

        [TestMethod]
        public void testOutputsAndDefaults()
        {
            BasicDataProvider provider = new BasicDataProvider();

            BasicTable table = new BasicTable("table_input");
            table.addColumnDefinition("input1", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addRawRow(new List<String>() { "000", "Alpha" });
            table.addRawRow(new List<String>() { "001", "Beta" });
            table.addRawRow(new List<String>() { "002", "Gamma" });
            provider.addTable(table);

            table = new BasicTable("table_output");
            table.addColumnDefinition("output1", ColumnType.INPUT);
            table.addColumnDefinition("description", ColumnType.DESCRIPTION);
            table.addRawRow(new List<String>() { "A", "Alpha" });
            table.addRawRow(new List<String>() { "B", "Beta" });
            table.addRawRow(new List<String>() { "C", "Gamma" });
            provider.addTable(table);

            BasicDefinition def = new BasicDefinition("sample_outputs");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);
            def.addInput(new BasicInput("input1", "table_input"));

            BasicOutput output = new BasicOutput("output1", "table_output");
            output.setDefault("A");
            def.addOutput(output);

            output = new BasicOutput("output2");
            def.addOutput(output);

            BasicMapping mapping = new BasicMapping("mapping1");
            def.addMapping(mapping);
            provider.addDefinition(def);

            DecisionEngineClass engine = new DecisionEngineClass(provider);

            Dictionary<String, String> input = new Dictionary<String, String>();
            input["input1"] = "000";
            Result result = engine.process("sample_outputs", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());

            // default value should be set
            Assert.AreEqual("A", input["output1"]);
            // no default value so it should be blank
            Assert.AreEqual("", input["output2"]);

            Assert.IsFalse(result.hasErrors());

            HashSet<String> hash1 = new HashSet<String>() { "table_input", "table_output" };
            HashSet<String> hash2 = engine.getInvolvedTables(def);
            Assert.IsTrue(hash1.SetEquals(hash2));

            // modify the definition to create a bad default value for output1
            def.getOutputs()[0].setDefault("BAD");
            provider.initDefinition(def);
            engine = new DecisionEngineClass(provider);

            input = new Dictionary<String, String>();
            input["input1"] = "000";
            result = engine.process("sample_outputs", input);

            Assert.AreEqual(Result.Type.STAGED, result.getType());
            Assert.IsTrue(result.hasErrors());
            Assert.AreEqual(Error.Type.INVALID_OUTPUT, result.getErrors()[0].getType());
            Assert.AreEqual("table_output", result.getErrors()[0].getTable());
            Assert.AreEqual("output1", result.getErrors()[0].getKey());

            // default value should be set
            Assert.AreEqual("BAD", input["output1"]);
            // no default value so it should be blank
            Assert.AreEqual("", input["output2"]);
        }

        [TestMethod]
        public void testInitialContextReferences()
        {
            BasicDefinition def = new BasicDefinition("test_initial_context");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);

            def.addInitialContext("a", "foo1");
            def.addInitialContext("b", "{{foo1}}");
            def.addInitialContext("c", "foo2");
            def.addInitialContext("d", "{{foo2}}");
            def.addInitialContext("e", "{{bad_key}}");

            BasicDataProvider provider = new BasicDataProvider();
            provider.addDefinition(def);
            DecisionEngineClass engine = new DecisionEngineClass(provider);

            Dictionary<String, String> context = new Dictionary<String, String>();
            context["foo1"] = "FIRST";
            context["foo2"] = "SECOND";
            Result result = engine.process("test_initial_context", context);

            Assert.AreEqual(Result.Type.STAGED, result.getType());

            Assert.AreEqual(context["a"], "foo1");
            Assert.AreEqual(context["b"], "FIRST");
            Assert.AreEqual(context["c"], "foo2");
            Assert.AreEqual(context["d"], "SECOND");
            Assert.AreEqual(context["e"], "");
        }

        [TestMethod]
        public void testInputsOutputsDefaultContextReferences()
        {
            BasicDefinition def = new BasicDefinition("test_context");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);

            // add inputs
            BasicInput input = new BasicInput("input1");
            input.setDefault("foo1");
            def.addInput(input);

            input = new BasicInput("input2");
            input.setDefault("{{foo1}}");
            def.addInput(input);

            // add outputs
            BasicOutput output = new BasicOutput("output1");
            output.setDefault("foo2");
            def.addOutput(output);

            output = new BasicOutput("output2");
            output.setDefault("{{foo2}}");
            def.addOutput(output);

            output = new BasicOutput("output3");
            output.setDefault("{{bad_key}}");
            def.addOutput(output);

            output = new BasicOutput("output4");
            output.setDefault("{{input1}}");
            def.addOutput(output);

            output = new BasicOutput("output5");
            output.setDefault("{{input2}}");
            def.addOutput(output);

            BasicDataProvider provider = new BasicDataProvider();
            provider.addDefinition(def);
            DecisionEngineClass engine = new DecisionEngineClass(provider);

            Dictionary<String, String> context = new Dictionary<String, String>();
            context["foo1"] = "FIRST";
            context["foo2"] = "SECOND";
            Result result = engine.process("test_context", context);

            Assert.AreEqual(Result.Type.STAGED, result.getType());

            Assert.AreEqual(context["output1"], "foo2");
            Assert.AreEqual(context["output2"], "SECOND");
            Assert.AreEqual(context["output3"], "");
            Assert.AreEqual(context["output4"], "foo1");
            Assert.AreEqual(context["output5"], "FIRST");
        }

        [TestMethod]
        public void testMappingInputsWithReferenceInTable()
        {
            BasicDataProvider provider = new BasicDataProvider();

            // test a situation where an input is also used as a reference in an endpoint; when the definition remaps that input it should not show up
            // as both values
            BasicTable table = new BasicTable("table_input");
            table.addColumnDefinition("input1", ColumnType.INPUT);
            table.addColumnDefinition("output1", ColumnType.ENDPOINT);
            table.addRawRow(new List<String>() { "000", "VALUE:001" });
            table.addRawRow(new List<String>() { "001", "VALUE:000" });
            table.addRawRow(new List<String>() { "002", "VALUE:{{input1}}" });
            provider.addTable(table);

            BasicDefinition def = new BasicDefinition("sample_outputs");
            def.setOnInvalidInput(StagingInputErrorHandler.FAIL);
            def.addInput(new BasicInput("input1", "table_input"));

            BasicMapping mapping = new BasicMapping("mapping1");
            BasicTablePath path = new BasicTablePath("table_input");
            path.addInputMapping("remapped1", "input1");
            mapping.addTablePath(path);
            def.addMapping(mapping);
            provider.addDefinition(def);

            DecisionEngineClass engine = new DecisionEngineClass(provider);

            HashSet<String> hash1 = new HashSet<String>() { "remapped1" };
            HashSet<String> hash2 = engine.getInputs(def.getMappings()[0].getTablePaths()[0]);
            Assert.IsTrue(hash1.SetEquals(hash2));
        }


    }
}


