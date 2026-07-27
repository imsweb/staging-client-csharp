using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.CS;
using TNMStagingCSharp.Src.Staging.Engine;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;
using TNMStagingCSharp.Src.Staging.EOD;
using TNMStagingCSharp.Src.Staging.TNM;
using static TNMStagingCSharp.Src.Staging.CS.CsStagingData;
using static TNMStagingCSharp.Src.Staging.EOD.EodStagingData;
using static TNMStagingCSharp.Src.Staging.TNM.TnmStagingData;

namespace TNMStaging_UnitTestApp.Src.Staging
{
    internal class StagingInputsOutputsTest
    {
        private InMemoryDataProvider _provider;
        private TNMStagingCSharp.Src.Staging.Staging _staging;
        private StagingTablePath _mainPath;
        private StagingMapping _mapping;

        [ClassInitialize()]
        public void ClassInit(TestContext context)
        {
            _provider = new InMemoryDataProvider("test", "1.0");
            _provider.addTable(
                table(
                    "selection",
                    new List<StagingColumnDefinition>()
                    {
                        new StagingColumnDefinition("selector", "Selector", ColumnType.INPUT),
                        new StagingColumnDefinition(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT, "Current year", ColumnType.INPUT)
                    },
                    new List<string>() {"*", "*"}
                )
            );
            _provider.addTable(
                table(
                    "inclusion",
                    new List<StagingColumnDefinition>() { new StagingColumnDefinition("include_flag", "Include", ColumnType.INPUT) },
                    new List<string>() { "Y" }
                )
            );
            _provider.addTable(
                table(
                    "exclusion",
                    new List<StagingColumnDefinition>() { new StagingColumnDefinition("exclude_flag", "Exclude", ColumnType.INPUT) },
                    new List<string>() { "Y" }
                )
            );
            _provider.addTable(
                table(
                    "main",
                    new List<StagingColumnDefinition>()
                    {
                        new StagingColumnDefinition("raw_input", "Raw input", ColumnType.INPUT),
                        new StagingColumnDefinition(TNMStagingCSharp.Src.Staging.Staging.CTX_ALGORITHM_VERSION, "Algorithm version", ColumnType.INPUT),
                        new StagingColumnDefinition(TNMStagingCSharp.Src.Staging.Staging.CTX_YEAR_CURRENT, "Current year", ColumnType.INPUT),
                        new StagingColumnDefinition("raw_output", "Raw output", ColumnType.ENDPOINT)
                    },
                    new List<string>() { "*", "*", "*", "VALUE:result" }
                )
            );

            _mainPath = new StagingTablePath("main");
            _mainPath.addInputMapping("case_input", "raw_input");
            _mainPath.addOutputMapping("raw_output", "mapped_output");

            _mapping = new StagingMapping("conditional", new List<ITablePath>() { _mainPath });
            _mapping.setInclusionTables(new List<ITablePath>() { new StagingTablePath("inclusion") });
            _mapping.setExclusionTables(new List<ITablePath>() { new StagingTablePath("exclusion") });

            _staging = TNMStagingCSharp.Src.Staging.Staging.getInstance(_provider);
        }

        [TestMethod]
        void getsMappedTablePathInputsAndOutputs()
        {
            assertThat(_staging.getInputs(_mainPath)).containsExactly("case_input");
            assertThat(_staging.getInputs(_mainPath, new HashSet<>(Set.of("case_input")))).isEmpty();
            assertThat(_staging.getInputs((StagingTablePath)null)).isEmpty();

            assertThat(_staging.getOutputs(_mainPath)).containsExactly("mapped_output");
        }

        [TestMethod]
        void getsMappingInputsAndOutputsWithExclusionsAndContext()
        {
            Dictionary<String, String> included = Map.of("include_flag", "Y", "exclude_flag", "N");
            Dictionary<String, String> notIncluded = Map.of("include_flag", "N", "exclude_flag", "N");
            Dictionary<String, String> excluded = Map.of("include_flag", "Y", "exclude_flag", "Y");

            assertThat(_staging.getInputs(_mapping)).containsExactlyInAnyOrder(
                "include_flag",
                "exclude_flag",
                "case_input"
            );
            assertThat(_staging.getInputs(_mapping, included, new HashSet<>())).containsExactlyInAnyOrder(
                "include_flag",
                "exclude_flag",
                "case_input"
            );
            assertThat(_staging.getInputs(_mapping, notIncluded, new HashSet<>())).containsExactlyInAnyOrder(
                "include_flag",
                "exclude_flag"
            );
            assertThat(_staging.getInputs(_mapping, excluded, new HashSet<>(Set.of("include_flag")))).containsExactly(
                "exclude_flag"
            );

            assertThat(_staging.getOutputs(_mapping)).containsExactly("mapped_output");
            assertThat(_staging.getOutputs(_mapping, included)).containsExactly("mapped_output");
            assertThat(_staging.getOutputs(_mapping, notIncluded)).isEmpty();
            assertThat(_staging.getOutputs(_mapping, excluded)).isEmpty();
        }

        [TestMethod]
        void getsSchemaInputsAndExplicitOrInferredOutputs()
        {
            Dictionary<String, String> excluded = Map.of("include_flag", "Y", "exclude_flag", "Y");

            StagingSchema inferred = schema("inferred", _mapping);
            _provider.addSchema(inferred);
            inferred.setOutputMap(null);

            assertThat(_staging.getInputs(inferred)).containsExactlyInAnyOrder(
                "selector",
                "include_flag",
                "exclude_flag",
                "case_input"
            );
            assertThat(_staging.getInputs(inferred, excluded)).containsExactlyInAnyOrder(
                "selector",
                "include_flag",
                "exclude_flag"
            );
            assertThat(_staging.getOutputs(inferred)).containsExactly("mapped_output");
            assertThat(_staging.getOutputs(inferred, excluded)).isEmpty();

            StagingSchema explicit = schema("explicit", _mapping);
            explicit.setOutputs(
            Arrays.asList(
                    new StagingSchemaOutput("declared_one", "Declared one"),
                    new StagingSchemaOutput("declared_two", "Declared two")
                )
            );
            _provider.addSchema(explicit);

            assertThat(_staging.getOutputs(explicit)).containsExactlyInAnyOrder("declared_one", "declared_two");
            assertThat(_staging.getOutputs(explicit, excluded)).containsExactlyInAnyOrder("declared_one", "declared_two");
        }

        private StagingSchema schema(String id, StagingMapping mapping)
        {
            StagingSchema schema = new StagingSchema(id);
            schema.setSchemaSelectionTable("selection");
            schema.setMappings(new List<IMapping>() { mapping });
            return schema;
        }

        private StagingTable table(String id, List<StagingColumnDefinition> definitions, List<String> row)
        {
            StagingTable table = new StagingTable(id);
            table.setColumnDefinitions((List<IColumnDefinition>)definitions);
            table.setRawRows(Collections.singletonList(row));
            return table;
        }


    }
}

/*
// Copyright (C) 2026 Information Management Services, Inc.
package com.imsweb.staging;

import static org.assertj.core.api.Assertions.assertThat;

import com.imsweb.staging.entities.ColumnDefinition.ColumnType;
import com.imsweb.staging.entities.impl.StagingColumnDefinition;
import com.imsweb.staging.entities.impl.StagingMapping;
import com.imsweb.staging.entities.impl.StagingSchema;
import com.imsweb.staging.entities.impl.StagingSchemaOutput;
import com.imsweb.staging.entities.impl.StagingTable;
import com.imsweb.staging.entities.impl.StagingTablePath;
import java.util.Arrays;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

class StagingInputsOutputsTest
{


*/



