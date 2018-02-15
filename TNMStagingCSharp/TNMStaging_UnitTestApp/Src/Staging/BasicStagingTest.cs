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
        protected static TNMStagingCSharp.Src.Staging.Staging _STAGING;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
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

            _STAGING = TNMStagingCSharp.Src.Staging.Staging.getInstance(provider);
        }


        [TestMethod]
        public void testBlankInputs()
        {
            Assert.AreEqual("schema_test", _STAGING.getSchema("schema_test").getId());

            // check case where required input field not supplied (i.e. no default); since there are is no workflow defined, this should
            // not cause an error

            StagingData data = new StagingData("C509", "8000");
            data.setInput("year_dx", "2018");
            data.setInput("input1", "1");

            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());

            // pass in blank for "input2"
            data = new StagingData("C509", "8000");
            data.setInput("year_dx", "2018");
            data.setInput("input1", "1");
            data.setInput("input2", "");

            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());

            // pass in null for "input2"

            data = new StagingData("C509", "8000");
            data.setInput("year_dx", "2018");
            data.setInput("input1", "1");
            data.setInput("input2", null);

            _STAGING.stage(data);
            Assert.AreEqual(StagingData.Result.STAGED, data.getResult());
        }

    }
}



