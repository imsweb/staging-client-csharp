// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;


namespace TNMStagingCSharp.Src.Staging
{
    public class ExternalStagingFileDataProvider: StagingDataProvider
    {
        private String _algorithm;
        private String _version;
        private readonly Dictionary<String, StagingTable> _tables = new Dictionary<String, StagingTable>();
        private readonly Dictionary<String, StagingSchema> _schemas = new Dictionary<String, StagingSchema>();
        private readonly HashSet<String> _TableKeys = new HashSet<String>();
        private readonly HashSet<String> _SchemaKeys = new HashSet<String>();
        private readonly Dictionary<String, GlossaryDefinition> _glossaryTerms = new Dictionary<String, GlossaryDefinition>();

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

            //TrieBuilder builder = Trie.builder().onlyWholeWords().ignoreCase();
            _trie = new HashSet<String>();

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
                    else if (entry.FullName.StartsWith("glossary"))
                    {
                        //GlossaryDefinition glossary = getMapper().reader().readValue(getMapper().getFactory().createParser(extractEntry(stream)), GlossaryDefinition.class);
                        String s = extractEntry(entry);
                        GlossaryDefinition glossary = new GlossaryDefinition();
                        glossary = Newtonsoft.Json.JsonConvert.DeserializeObject<GlossaryDefinition>(s);

                        _glossaryTerms[glossary.getName()] = glossary;
                        _trie.Add(glossary.getName());
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

        public override ITable getTable(String id)
        {
            ITable oRetval = null;
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

        public override Schema getSchema(String id)
        {
            Schema oRetval = null;
            if (_schemas.ContainsKey(id))
                oRetval = _schemas[id];

            return oRetval;
        }

        public override IEndpoint getEndpoint(EndpointType type, String value)
        {
            return new StagingEndpoint(type, value);
        }

        public override ITableRow getTableRow()
        {
            return new StagingTableRow();
        }

        public override Range getMatchAllRange()
        {
            return new StagingRange();
        }

        public override Range getRange(String low, String high)
        {
            return new StagingRange(low, high);
        }

        public override HashSet<String> getGlossaryTerms()
        {
            return new HashSet<String>(_glossaryTerms.Keys);
        }

        public override GlossaryDefinition getGlossaryDefinition(String term)
        {
            return _glossaryTerms[term];
        }
    }
}


