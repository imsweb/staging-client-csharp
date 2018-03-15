/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.Staging
{
    [TestClass]
    public class BasicStagingTest
    {

        [TestMethod]
        public void testBlankInputs()
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("test", "1.0");

            StagingTable table = new StagingTable();
            table.setId("table_input1");
            StagingColumnDefinition def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            StagingColumnDefinition def2 = new StagingColumnDefinition();
            def2.setKey("result1");
            def2.setName("Result1");
            def2.setType(ColumnType.DESCRIPTION);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1, def2 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "1", "ONE" });
            table.getRawRows().Add(new List<String>() { "2", "TWO" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_input2");
            def1 = new StagingColumnDefinition();
            def1.setKey("input2");
            def1.setName("Input 2");
            def1.setType(ColumnType.INPUT);
            def2 = new StagingColumnDefinition();
            def2.setKey("result2");
            def2.setName("Result2");
            def2.setType(ColumnType.DESCRIPTION);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1, def2 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "", "Blank" });
            table.getRawRows().Add(new List<String>() { "A", "Letter A" });
            table.getRawRows().Add(new List<String>() { "B", "Letter B" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_selection");
            def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "*" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("primary_site");
            def1 = new StagingColumnDefinition();
            def1.setKey("site");
            def1.setName("Site");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "C509" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("histology");
            def1 = new StagingColumnDefinition();
            def1.setKey("hist");
            def1.setName("Histology");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "8000" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_year_dx");
            def1 = new StagingColumnDefinition();
            def1.setKey("year_dx");
            def1.setName("Year DX");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "1900-2100" });
            provider.addTable(table);

            StagingSchema schema = new StagingSchema();
            schema.setId("schema_test");
            schema.setSchemaSelectionTable("table_selection");
            List<StagingSchemaInput> inputs = new List<StagingSchemaInput>();
            inputs.Add(new StagingSchemaInput("site", "Primary Site", "primary_site"));
            inputs.Add(new StagingSchemaInput("hist", "Hist", "histology"));
            inputs.Add(new StagingSchemaInput("year_dx", "Year DX", "table_year_dx"));
            inputs.Add(new StagingSchemaInput("input1", "Input 1", "table_input1"));
            inputs.Add(new StagingSchemaInput("input2", "Input 2", "table_input2"));
            schema.setInputs(inputs);

            provider.addSchema(schema);

            TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);


            Assert.AreEqual("schema_test", staging.getSchema("schema_test").getId());

            // check case where required input field not supplied (i.e. no default); since there are is no workflow defined, this should
            // not cause an error

            StagingData data = new StagingData("C509", "8000");
            data.setInput("year_dx", "2018");
            data.setInput("input1", "1");

            staging.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());

            // pass in blank for "input2"
            data = new StagingData("C509", "8000");
            data.setInput("year_dx", "2018");
            data.setInput("input1", "1");
            data.setInput("input2", "");

            staging.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());

            // pass in null for "input2"

            data = new StagingData("C509", "8000");
            data.setInput("year_dx", "2018");
            data.setInput("input1", "1");
            data.setInput("input2", null);

            staging.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
        }

        [TestMethod]
        public void testNumericRangeTableMatch()
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("test", "1.0");

            StagingTable table = new StagingTable();
            table.setId("psa");
            StagingColumnDefinition def1 = new StagingColumnDefinition();
            def1.setKey("psa");
            def1.setName("PSA Value");
            def1.setType(ColumnType.INPUT);
            StagingColumnDefinition def2 = new StagingColumnDefinition();
            def2.setKey("description");
            def2.setName("PSA Description");
            def2.setType(ColumnType.DESCRIPTION);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1, def2 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "0.1", "0.1 or less nanograms/milliliter (ng/ml)" });
            table.getRawRows().Add(new List<String>() { "0.2-999.9", "0.2 – 999.9 ng/ml" });
            provider.addTable(table);

            TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);

            Assert.AreEqual(0, staging.findMatchingTableRow("psa", "psa", "0.1"));
            Assert.AreEqual(1, staging.findMatchingTableRow("psa", "psa", "0.2"));
            Assert.AreEqual(1, staging.findMatchingTableRow("psa", "psa", "500"));
            Assert.AreEqual(1, staging.findMatchingTableRow("psa", "psa", "500.99"));
            Assert.AreEqual(1, staging.findMatchingTableRow("psa", "psa", "500.0001"));
            Assert.AreEqual(1, staging.findMatchingTableRow("psa", "psa", "999.9"));
            Assert.AreEqual(-1, staging.findMatchingTableRow("psa", "psa", "1000"));
            Assert.AreEqual(-1, staging.findMatchingTableRow("psa", "psa", "-1"));
            Assert.AreEqual(-1, staging.findMatchingTableRow("psa", "psa", "0.01"));
        }

        [TestMethod]
        public void testGetInputsWithContext()
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("test", "1.0");

            StagingTable table = new StagingTable();
            table.setId("table_input1");
            StagingColumnDefinition def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "1" });
            table.getRawRows().Add(new List<String>() { "2" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_input2");
            def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 2");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "" });
            table.getRawRows().Add(new List<String>() { "A" });
            table.getRawRows().Add(new List<String>() { "B" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_selection");
            def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "*" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_mapping");

            def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            StagingColumnDefinition def2 = new StagingColumnDefinition();
            def2.setKey("input2");
            def2.setName("Input 2");
            def2.setType(ColumnType.INPUT);
            StagingColumnDefinition def3 = new StagingColumnDefinition();
            def3.setKey("mapped_field");
            def3.setName("Temp value");
            def3.setType(ColumnType.INPUT);
            StagingColumnDefinition def4 = new StagingColumnDefinition();
            def4.setKey("final_output");
            def4.setName("Output");
            def4.setType(ColumnType.ENDPOINT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1, def2, def3, def4 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "*", "*", "*", "VALUE:ABC" });
            provider.addTable(table);

            StagingSchema schema = new StagingSchema();
            schema.setId("schema_test");
            schema.setSchemaSelectionTable("table_selection");
            List<StagingSchemaInput> inputs = new List<StagingSchemaInput>();
            inputs.Add(new StagingSchemaInput("input1", "Input 1", "table_input1"));
            inputs.Add(new StagingSchemaInput("input2", "Input 2", "table_input2"));
            schema.setInputs(inputs);
            List<StagingSchemaOutput> outputs = new List<StagingSchemaOutput>();
            outputs.Add(new StagingSchemaOutput("final_output", "Final Output"));
            schema.setOutputs(outputs);

            StagingMapping mapping = new StagingMapping();
            mapping.setId("m1");
            List<IKeyValue> mapInitialContext = new List<IKeyValue>();
            mapInitialContext.Add(new StagingKeyValue("tmp_field", null));
            mapping.setInitialContext(mapInitialContext);
            StagingTablePath path = new StagingTablePath();
            path.setId("table_mapping");
            HashSet<IKeyMapping> pathInputMap = new HashSet<IKeyMapping>();
            pathInputMap.Add(new StagingKeyMapping("tmp_field", "mapped_field"));
            path.setInputMapping(pathInputMap);
            HashSet<String> pathInputs = new HashSet<String>();
            pathInputs.Add("input1");
            pathInputs.Add("input2");
            pathInputs.Add("tmp_field");
            path.setInputs(pathInputs);
            HashSet<String> pathOutputs = new HashSet<String>();
            pathOutputs.Add("final_output");
            path.setOutputs(pathOutputs);
            List<ITablePath> mapTablePaths = new List<ITablePath>();
            mapTablePaths.Add(path);
            mapping.setTablePaths(mapTablePaths);
            List<IMapping> schemaMappings = new List<IMapping>();
            schemaMappings.Add(mapping);
            schema.setMappings(schemaMappings);

            provider.addSchema(schema);

            TNMStagingCSharp.Src.Staging.Staging staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);

            HashSet<String> testSet1 = staging.getInputs(staging.getSchema("schema_test"));

            HashSet<String> testSet2 = new HashSet<String>();
            testSet2.Add("input1");
            testSet2.Add("input2");

            // should only return the "real" inputs and not the temp field set in initial context
            Assert.IsTrue(testSet1.SetEquals(testSet2));

        }

        [TestMethod]
        public void testInvalidContext()
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("test", "1.0");

            StagingTable table = new StagingTable();
            table.setId("table_input1");
            StagingColumnDefinition def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "1" });
            table.getRawRows().Add(new List<String>() { "2" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_selection");
            def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "*" });
            provider.addTable(table);

            table = new StagingTable();
            table.setId("table_mapping");
            def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            StagingColumnDefinition def2 = new StagingColumnDefinition();
            def2.setKey("final_output");
            def2.setName("Output");
            def2.setType(ColumnType.ENDPOINT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1, def2 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "*", "VALUE:ABC" });
            provider.addTable(table);

            StagingSchema schema = new StagingSchema();
            schema.setId("schema_test");
            schema.setSchemaSelectionTable("table_selection");
            List<StagingSchemaInput> inputs = new List<StagingSchemaInput>();
            inputs.Add(new StagingSchemaInput("input1", "Input 1", "table_input1"));
            schema.setInputs(inputs);
            List<StagingSchemaOutput> outputs = new List<StagingSchemaOutput>();
            outputs.Add(new StagingSchemaOutput("final_output", "Final Output"));
            schema.setOutputs(outputs);

            StagingMapping mapping = new StagingMapping();
            mapping.setId("m1");
            List<IKeyValue> mapInitialContext = new List<IKeyValue>();
            mapInitialContext.Add(new StagingKeyValue("input1", "XXX"));
            mapping.setInitialContext(mapInitialContext);
            StagingTablePath path = new StagingTablePath();
            path.setId("table_mapping");
            HashSet<String> pathInputs = new HashSet<String>();
            pathInputs.Add("input1");
            path.setInputs(pathInputs);
            HashSet<String> pathOutputs = new HashSet<String>();
            pathOutputs.Add("final_output");
            path.setOutputs(pathOutputs);

            List<ITablePath> mapTablePaths = new List<ITablePath>();
            mapTablePaths.Add(path);
            mapping.setTablePaths(mapTablePaths);
            List<IMapping> schemaMappings = new List<IMapping>();
            schemaMappings.Add(mapping);
            schema.setMappings(schemaMappings);

            try
            {
                provider.addSchema(schema);
                Assert.Fail("Add Schema should have thrown an exception.");
            } catch (System.InvalidOperationException e)
            {
                Assert.IsTrue(e.Message.Contains("not allowed since it is also defined as an input"));
            }

    }



}
}



