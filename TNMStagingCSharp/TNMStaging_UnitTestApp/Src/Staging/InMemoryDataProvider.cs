/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.Staging
{
    /**
     * In implementation of DataProvider which holds all data in memory
     */
    class InMemoryDataProvider : StagingDataProvider
    {
        public enum Algorithm
        {
            TEST1
        }

        private readonly String _algorithm;
        private readonly String _version;

        private readonly Dictionary<String, StagingTable> _tables = new Dictionary<String, StagingTable>();
        private readonly Dictionary<String, StagingSchema> _schemas = new Dictionary<String, StagingSchema>();
        private readonly HashSet<String> _TableKeys = new HashSet<String>();
        private readonly HashSet<String> _SchemaKeys = new HashSet<String>();


        /**
         * Constructor loads all schemas and sets up table cache
         */
        public InMemoryDataProvider(String algorithm, String version) : base()
        {
            _algorithm = algorithm;
            _version = version;

            _SchemaKeys.Clear();
            _TableKeys.Clear();
        }

        public override String getAlgorithm()
        {
            return _algorithm;
        }

        public override String getVersion()
        {
            return _version;
        }

        public override ITable getTable(String id)
        {
            StagingTable oRetval = null;
            _tables.TryGetValue(id, out oRetval);

            return oRetval;
        }

        /**
         * Add a table
         */
        public void addTable(StagingTable table)
        {
            initTable(table);
            _tables[table.getId()] = table;

            foreach (KeyValuePair<string, StagingTable> entry in _tables)
            {
                _TableKeys.Add(entry.Key);
            }
        }

        /**
         * Return a set of all the table names
         */
        public override HashSet<String> getTableIds()
        {
            return _TableKeys;
        }

        public override IDefinition getDefinition(String id)
        {
            StagingSchema oRetval = null;
            _schemas.TryGetValue(id, out oRetval);

            return oRetval;
        }

        public override HashSet<String> getSchemaIds()
        {
            return _SchemaKeys;
        }

        /**
         * Add a schema
         */
        public void addSchema(StagingSchema schema)
        {
            initSchema(schema);
            _schemas[schema.getId()] = schema;

            foreach (KeyValuePair<string, StagingSchema> entry in _schemas)
            {
                _SchemaKeys.Add(entry.Key);
            }
        }

        public override HashSet<String> getGlossaryTerms()
        {
            throw new Exception("Glossary not supported in this provider");
        }

        public override GlossaryDefinition getGlossaryDefinition(String term)
        {
            throw new Exception("Glossary not supported in this provider");
        }

        public override List<GlossaryHit> getGlossaryMatches(String text)
        {
            throw new Exception("Glossary not supported in this provider");
        }
    }
}

