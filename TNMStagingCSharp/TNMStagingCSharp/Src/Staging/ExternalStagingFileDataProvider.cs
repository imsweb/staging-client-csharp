// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.Staging.Entities;


namespace TNMStagingCSharp.Src.Staging
{
    public class ExternalStagingFileDataProvider: StagingDataProvider
    {
        private String _algorithm;
        private String _version;
        private Dictionary<String, StagingTable> _tables = new Dictionary<String, StagingTable>();
        private Dictionary<String, StagingSchema> _schemas = new Dictionary<String, StagingSchema>();
        private HashSet<String> _TableKeys = new HashSet<String>();
        private HashSet<String> _SchemaKeys = new HashSet<String>();

        // Constructor loads all schemas and sets up table cache
        // @param is InputStream pointing the the zip file
        public ExternalStagingFileDataProvider(Stream inStream) : base()
        {
            init(inStream);
        }

        // Read a zip entry from an inputstream and return as a byte array
        private static String extractEntry(ZipArchiveEntry entry)
        {
            String s = "";
            using (Stream stream = entry.Open())
                using (StreamReader reader = new StreamReader(stream))
                {
                    s = reader.ReadToEnd();
                }
            return s;
        }

        // Initialize data provider
        private void init(Stream inStream)
        {
            HashSet<String> algorithms = new HashSet<String>();
            HashSet<String> versions = new HashSet<String>();


            using (ZipArchive archive = new ZipArchive(inStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if ((entry.Name.Length == 0) || (!entry.Name.EndsWith(".json")))
                        continue;

                    if (entry.FullName.StartsWith("tables"))
                    {
                        String s = extractEntry(entry);
                        StagingTable table = new StagingTable();
                        table = Newtonsoft.Json.JsonConvert.DeserializeObject<StagingTable>(s);

                        if (DebugSettings.DEBUG_LOADED_TABLES)
                        {
                            Debug.WriteLine("Table: ");
                            Debug.WriteLine(table.GetDebugString("  "));
                        }

                        initTable(table);

                        algorithms.Add(table.getAlgorithm());
                        versions.Add(table.getVersion());

                        _tables[table.getId()] = table;
                    }
                    else if (entry.FullName.StartsWith("schemas")) 
                    {
                        String s = extractEntry(entry);
                        StagingSchema schema = new StagingSchema();
                        schema = Newtonsoft.Json.JsonConvert.DeserializeObject<StagingSchema>(s);

                        if (DebugSettings.DEBUG_LOADED_SCHEMAS)
                        {
                            Debug.WriteLine("Schema: ");
                            Debug.WriteLine(schema.GetDebugString("  "));
                        }

                        initSchema(schema);

                        algorithms.Add(schema.getAlgorithm());
                        versions.Add(schema.getVersion());

                        _schemas[schema.getId()] = schema;
                    }

                }
            }

            // verify that all the algorithm names and versions are consistent
            if (algorithms.Count != 1)
                throw new System.InvalidOperationException("Error initializing provider; only a single algorithm should be included in file");
            if (versions.Count != 1)
                throw new System.InvalidOperationException("Error initializing provider; only a single version should be included in file");

            HashSet<String>.Enumerator enumAlg = algorithms.GetEnumerator();
            HashSet<String>.Enumerator enumVer = versions.GetEnumerator();
            enumAlg.MoveNext();
            enumVer.MoveNext();
            _algorithm = enumAlg.Current;
            _version = enumVer.Current;

            GenerateSchemaIds();
            GenerateTableIds();

            // finally, initialize any caches now that everything else has been set up
            invalidateCache();
        }

        private void GenerateSchemaIds()
        {
            _SchemaKeys.Clear();
            foreach (KeyValuePair<string, StagingSchema> entry in _schemas)
            {
                _SchemaKeys.Add(entry.Key);
            }
        }

        private void GenerateTableIds()
        {
            _TableKeys.Clear();
            foreach (KeyValuePair<string, StagingTable> entry in _tables)
            {
                _TableKeys.Add(entry.Key);
            }
        }

        public override String getAlgorithm()
        {
            return _algorithm.ToLower();
        }

        public override String getVersion()
        {
            return _version;
        }

        public override DecisionEngine.ITable getTable(String id)
        {
            DecisionEngine.ITable oRetval = null;
            if (id != null)
                if (_tables.ContainsKey(id))
                    oRetval = _tables[id];

            return oRetval;

        }

        public override HashSet<String> getSchemaIds()
        {
            return _SchemaKeys;
        }

        public override HashSet<String> getTableIds()
        {
            return _TableKeys;
        }

        public override DecisionEngine.IDefinition getDefinition(String id)
        {
            DecisionEngine.IDefinition oRetval = null;
            if (_schemas.ContainsKey(id))
                oRetval = _schemas[id];

            return oRetval;
        }
    }
}


