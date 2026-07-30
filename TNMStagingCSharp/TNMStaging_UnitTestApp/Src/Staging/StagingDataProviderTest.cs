using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;


namespace TNMStaging_UnitTestApp.Src.Staging
{
    [TestClass]
    public class StagingDataProviderTest
    {

        [TestMethod]
        public void testExtraInput()
        {
            StagingTable table = new StagingTable();
            table.setId("test_table");
            StagingColumnDefinition def1 = new StagingColumnDefinition();
            def1.setKey("input1");
            def1.setName("Input 1");
            def1.setType(ColumnType.INPUT);
            StagingColumnDefinition def2 = new StagingColumnDefinition();
            def2.setKey("result1");
            def2.setName("Result1");
            def2.setType(ColumnType.ENDPOINT);
            table.setColumnDefinitions(new List<IColumnDefinition>() { def1, def2 });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "1", "MATCH" });
            table.getRawRows().Add(new List<String>() { "2", "VALUE:{{extra1}}" });
            table.getRawRows().Add(new List<String>() { "{{extra2}}", "MATCH" });
            table.getRawRows().Add(new List<String>() { "{{ctx_year_current}}", "MATCH" });
            table.getRawRows().Add(new List<String>() { "5", "VALUE:{{ctx_year_current}}" });
            table.getRawRows().Add(new List<String>() { "6", "MATCH:{{match_extra}}" });
            table.getRawRows().Add(new List<String>() { "7", "ERROR:{{error_extra}}" });

            StagingDataProvider provider = new InMemoryDataProvider("test", "1.0");
            provider.initTable(table);

            HashSet<String> hash1 = new HashSet<String>() { "extra1", "extra2" };
            HashSet<String> hash2 = table.getExtraInput();

            // since context variables are not user-supplied, they should not be included in the extra input
            Assert.IsTrue(hash1.SetEquals(hash2));

            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { "{{ctx_year_current}}", "MATCH" });

            provider.initTable(table);

            Assert.IsNull(table.getExtraInput());
        }

        [TestMethod]
        public void testSplitValues()
        {
            StagingDataProvider provider = new InMemoryDataProvider("algorithm", "verison");

            Assert.AreEqual(0, provider.splitValues(null).Count);

            Assert.AreEqual(1, provider.splitValues("").Count);
            Assert.AreEqual(1, provider.splitValues("*").Count);
            Assert.AreEqual(1, provider.splitValues("1 2 3").Count);
            Assert.AreEqual(1, provider.splitValues("23589258625086").Count);

            Assert.AreEqual(2, provider.splitValues("A,B").Count);
            Assert.AreEqual(2, provider.splitValues(" A , B ").Count);
            Assert.AreEqual(2, provider.splitValues("A , B").Count);

            Assert.AreEqual(10, provider.splitValues("A,B,C,D,E,F,G,H,I,J").Count);

            List<Range> ranges = provider.splitValues(",1,2,3,4");
            Assert.AreEqual(5, ranges.Count);
            Assert.AreEqual("", ranges.First().getLow());
            Assert.AreEqual("", ranges.First().getHigh());

            ranges = provider.splitValues("     ,1,2,3,4");
            Assert.AreEqual(5, ranges.Count);
            Assert.AreEqual("", ranges.First().getLow());
            Assert.AreEqual("", ranges.First().getHigh());

            ranges = provider.splitValues("1,2,3,4,");
            Assert.AreEqual(5, ranges.Count);
            Assert.AreEqual("", ranges[4].getLow());
            Assert.AreEqual("", ranges[4].getHigh());

            ranges = provider.splitValues("1,2,3,4,    ");
            Assert.AreEqual(5, ranges.Count);
            Assert.AreEqual("", ranges[4].getLow());
            Assert.AreEqual("", ranges[4].getHigh());

            ranges = provider.splitValues("1,11,111-222");
            Assert.AreEqual(3, ranges.Count);
            Assert.AreEqual("1", ranges.First().getLow());
            Assert.AreEqual("1", ranges.First().getHigh());
            Assert.AreEqual("11", ranges[1].getLow());
            Assert.AreEqual("11", ranges[1].getHigh());
            Assert.AreEqual("111", ranges[2].getLow());
            Assert.AreEqual("222", ranges[2].getHigh());

            ranges = provider.splitValues("88,90-95,99");
            Assert.AreEqual(3, ranges.Count);
            Assert.AreEqual("88", ranges.First().getLow());
            Assert.AreEqual("88", ranges.First().getHigh());
            Assert.AreEqual("90", ranges[1].getLow());
            Assert.AreEqual("95", ranges[1].getHigh());
            Assert.AreEqual("99", ranges[2].getLow());
            Assert.AreEqual("99", ranges[2].getHigh());

            ranges = provider.splitValues("p0I-");
            Assert.AreEqual(1, ranges.Count);
            Assert.AreEqual("p0I-", ranges.First().getLow());
            Assert.AreEqual("p0I-", ranges.First().getHigh());

            ranges = provider.splitValues("N0(mol-)");
            Assert.AreEqual(1, ranges.Count);
            Assert.AreEqual("N0(mol-)", ranges.First().getLow());
            Assert.AreEqual("N0(mol-)", ranges.First().getHigh());

            // test numeric ranges
            ranges = provider.splitValues("1-21");
            Assert.AreEqual(1, ranges.Count);
            Assert.AreEqual("1", ranges.First().getLow());
            Assert.AreEqual("21", ranges.First().getHigh());

            ranges = provider.splitValues("21-111");
            Assert.AreEqual(1, ranges.Count);
            Assert.AreEqual("21", ranges.First().getLow());
            Assert.AreEqual("111", ranges.First().getHigh());
        }

        [TestMethod]
        public void testTableRowParsing()
        {
            StagingTable table = new StagingTable();
            table.setId("test_table");
            table.setColumnDefinitions(new List<IColumnDefinition>() { new StagingColumnDefinition("key1", "Input 1", ColumnType.INPUT) });
            table.setRawRows(new List<List<String>>());
            table.getRawRows().Add(new List<String>() { ",1,2,3" });
            table.getRawRows().Add(new List<String>() { "1,2,3," });

            StagingDataProvider provider = new InMemoryDataProvider("test", "1.0");
            provider.initTable(table);

            Assert.AreEqual(2, table.getTableRows().Count);


            //StagingTableRow
            List<ITableRow> tablerows = table.getTableRows();

            StagingTableRow row = (tablerows[0] as StagingTableRow);
            Assert.AreEqual(4, row.getInputs()["key1"].Count);

            row = (tablerows[1] as StagingTableRow);
            Assert.AreEqual(4, row.getInputs()["key1"].Count);

        }

        [TestMethod]
        void testInMemoryProviderContract() 
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("Test Algorithm", "1.2.3");

            Assert.AreEqual("Test Algorithm", provider.getAlgorithm());
            Assert.AreEqual("1.2.3", provider.getVersion());
            Assert.IsTrue(provider.getTableIds().Count == 0);
            Assert.IsTrue(provider.getSchemaIds().Count == 0);
            Assert.IsNull(provider.getTable("missing"));
            Assert.IsNull(provider.getSchema("missing"));

            StagingSchema schema = new StagingSchema("present");
            schema.setSchemaSelectionTable("selection");
            provider.addSchema(schema);
            Assert.IsTrue(provider.getSchemaIds().SetEquals(new HashSet<string>() { "present" }));
            Assert.AreEqual(schema, provider.getSchema("present"));
        }

        [TestMethod]
        void testInMemoryGlossaryIsUnsupported() 
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("test", "1.0");

            //assertGlossaryUnsupported(provider::getGlossaryTerms);
            bool exceptionThrown = false;
            try
            {
                provider.getGlossaryTerms();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Equals("Glossary not supported in this provider"))
                {
                    exceptionThrown = true;
                }
            }
            Assert.IsTrue(exceptionThrown);

            //assertGlossaryUnsupported(() -> provider.getGlossaryDefinition("term"));
            exceptionThrown = false;
            try
            {
                provider.getGlossaryDefinition("term");
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Equals("Glossary not supported in this provider"))
                {
                    exceptionThrown = true;
                }
            }
            Assert.IsTrue(exceptionThrown);

            //assertGlossaryUnsupported(() -> provider.getGlossaryMatches("text"));
            exceptionThrown = false;
            try
            {
                provider.getGlossaryMatches("text");
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Equals("Glossary not supported in this provider"))
                {
                    exceptionThrown = true;
                }
            }
            Assert.IsTrue(exceptionThrown);
        }

        //private static void assertGlossaryUnsupported(Runnable operation) 
        //{
        //    IllegalStateException exception = assertThrows(IllegalStateException.class, operation::run);
        //    Assert.AreEqual("Glossary not supported in this provider", exception.getMessage());
        //}

        //private static void assertInvalidInputDefinition(RuntimeException exception)
        //{
        //    Assert.IsTrue(exception.getCause() instanceof IllegalStateException);
        //    Assert.AreEqual(
        //        "Table 'primary_site' must have one and only one INPUT column.",
        //        exception.getCause().getMessage()
        //    );
        //}

        [TestMethod]
        void testValidValuesForMissingTable() 
        {
            StagingDataProvider provider = new InMemoryDataProvider("test", "1.0");

            Assert.IsTrue(provider.getValidSites().Count == 0);
        }

        private static StagingTable CreateTable(String id, params StagingColumnDefinition[] definitions)
        {
            StagingTable table = new StagingTable(id);
            table.setColumnDefinitions(definitions.ToList<IColumnDefinition>());
            table.setRawRows(new List<List<string>>());
            return table;
        }


        [TestMethod]
        void testValidValuesRequireExactlyOneInputDefinition() 
        {
            InMemoryDataProvider noInputs = new InMemoryDataProvider("test", "1.0");
            noInputs.addTable(
                CreateTable(
                    StagingDataProvider.PRIMARY_SITE_TABLE,
                    new StagingColumnDefinition("result", "Result", ColumnType.ENDPOINT)
                )
            );

            //RuntimeException noInputException = assertThrows(RuntimeException.class, noInputs::getValidSites);
            //assertInvalidInputDefinition(noInputException);
            bool exceptionThrown = false;
            try
            {
                noInputs.getValidSites();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Equals("Table 'primary_site' must have one and only one INPUT column."))
                {
                    exceptionThrown = true;
                }
            }
            Assert.IsTrue(exceptionThrown);

            InMemoryDataProvider multipleInputs = new InMemoryDataProvider("test", "1.0");
            multipleInputs.addTable(
                CreateTable(
                    StagingDataProvider.PRIMARY_SITE_TABLE,
                    new StagingColumnDefinition("site", "Site", ColumnType.INPUT),
                    new StagingColumnDefinition("other", "Other", ColumnType.INPUT)
                )
            );

            //RuntimeException multipleInputException = assertThrows(RuntimeException.class, multipleInputs::getValidSites);
            //assertInvalidInputDefinition(multipleInputException);
            exceptionThrown = false;
            try
            {
                multipleInputs.getValidSites();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Equals("Table 'primary_site' must have one and only one INPUT column."))
                {
                    exceptionThrown = true;
                }
            }
            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        void testValidValuesExpandRangesWithZeroPadding() 
        {
            InMemoryDataProvider provider = new InMemoryDataProvider("test", "1.0");
            StagingTable table = CreateTable(
                StagingDataProvider.PRIMARY_SITE_TABLE,
                new StagingColumnDefinition("site", "Site", ColumnType.INPUT)
            );
            table.setRawRows(
                new List<List<string>>()
                {
                    new List<string>() { "001" },
                    new List<string>() { "003-005" },
                    new List<string>() { "010" },
                }
            );
            provider.addTable(table);

            Assert.IsTrue(provider.getValidSites().SetEquals(new HashSet<string>() { "001", "003", "004", "005", "010" }));
            Assert.IsTrue(provider.isValidSite("004"));
            Assert.IsFalse(provider.isValidSite("4"));
        }


        [TestMethod]
        public void testPadStart()
        {
            Assert.IsNull(StagingDataProvider.padStart(null, 1, '0'));

            Assert.AreEqual(StagingDataProvider.padStart("123", 1, '0'), "123");
            Assert.AreEqual(StagingDataProvider.padStart("123", 3, '0'), "123");
            Assert.AreEqual(StagingDataProvider.padStart("123", 4, '0'), "0123");
            Assert.AreEqual(StagingDataProvider.padStart("1", 5, '0'), "00001");
        }

        [TestMethod]
        public void testIsNumeric()
        {
            Assert.IsTrue(StagingDataProvider.isNumeric("0"));
            Assert.IsTrue(StagingDataProvider.isNumeric("1"));
            Assert.IsTrue(StagingDataProvider.isNumeric("-1"));
            Assert.IsTrue(StagingDataProvider.isNumeric("1.1"));

            Assert.IsFalse(StagingDataProvider.isNumeric(null));
            Assert.IsFalse(StagingDataProvider.isNumeric(""));
            Assert.IsFalse(StagingDataProvider.isNumeric("1.1.1"));
            Assert.IsFalse(StagingDataProvider.isNumeric("NAN"));
        }
    }
}


