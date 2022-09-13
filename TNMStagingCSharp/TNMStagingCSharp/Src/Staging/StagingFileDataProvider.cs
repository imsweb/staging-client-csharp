// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;


namespace TNMStagingCSharp.Src.Staging
{
    // In implementation of DataProvider which holds all data in memory
    public class StagingFileDataProvider : StagingDataProvider
    {
        private readonly String _algorithm;
        private readonly String _version;
        private readonly Dictionary<String, StagingTable> _tables = new Dictionary<String, StagingTable>();
        private readonly Dictionary<String, StagingSchema> _schemas = new Dictionary<String, StagingSchema>();
        private readonly HashSet<String> _TableKeys = new HashSet<String>();
        private readonly HashSet<String> _SchemaKeys = new HashSet<String>();
        private readonly Dictionary<String, String> _glossaryTerms = new Dictionary<String, String>();
        private readonly String _basedir;

        private const String _ALGORITHM_BASE_DIR = "algorithms\\";
        private const String _JSON_EXT = ".json";

        // Constructor loads all schemas and sets up table cache
        // @param algorithm algorithm
        // @param version version
        public StagingFileDataProvider(String algorithm, String version): base ()
        {
            _algorithm = algorithm;
            _version = version;

            _basedir = System.IO.Directory.GetCurrentDirectory() + "\\";
            if (!Directory.Exists(_basedir + _ALGORITHM_BASE_DIR))
            {
                _basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
                if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0)
                {
                    _basedir += "\\..\\";
                }
                _basedir += "Resources\\";
            }


            String directory = "";

            // loop over all tables and load them into Map
            try
            {
                directory = _basedir + _ALGORITHM_BASE_DIR + algorithm.ToLower() + "\\" + version + "\\tables";
                foreach (String file in readLines(directory + "\\ids.txt"))
                {
                    if (file.Length != 0)
                    {
                        TextReader reader = getStagingInputStream(directory + "\\" + file + _JSON_EXT);
                        StagingTable table = new StagingTable();

                        using (reader)
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            table = (StagingTable)serializer.Deserialize(reader, typeof(StagingTable));
                        }


                        initTable(table);
                        _tables[table.getId()] = table;
                    }
                }
            }
            catch (IOException e) 
            {
                throw new System.InvalidOperationException("IOException reading tables: " + e.Message);
            }


            // loop over all schemas and load them into Map
            try
            {
                directory = _basedir + _ALGORITHM_BASE_DIR + algorithm.ToLower() + "\\" + version + "\\schemas";

                foreach (String file in readLines(directory + "\\ids.txt"))
                {
                    if (file.Length != 0)
                    {
                        TextReader reader = getStagingInputStream(directory + "\\" + file + _JSON_EXT);
                        StagingSchema schema = new StagingSchema();

                        using (reader)
                        {
                            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                            schema = (StagingSchema)serializer.Deserialize(reader, typeof(StagingSchema));
                        }

                        initSchema(schema);
                        _schemas[schema.getId()] = schema;
                    }
                }
            }
            catch (IOException e) 
            {
                throw new System.InvalidOperationException("IOException reading schemas: " + e.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
                Debug.WriteLine("           " + ex.StackTrace);
                throw new System.InvalidOperationException("Exception reading schemas: " + ex.Message);
            }

            // load the glossary terms
            try
            {
                _trie = new HashSet<String>();
                directory = _basedir + _ALGORITHM_BASE_DIR + algorithm.ToLower() + "\\" + version + "\\";
                String termsFilename = directory + "Glossary\\terms.txt";

                // if the file is not found, that just means that there are no glossary terms
                if (File.Exists(termsFilename))
                {
                    foreach (String line in readLines(termsFilename))
                    {
                        if (line.Length != 0)
                        {
                            String[] parts = line.Split('~');
                            if (parts.Length != 2)
                            {
                                throw new System.InvalidOperationException("Error parsing glossary terms.  Should only be two parts of each line in terms.txt");
                            }

                            _glossaryTerms[parts[0]] = parts[1];
                            _trie.Add(parts[0]);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                throw new System.InvalidOperationException("IOException reading glossary terms: " + e.Message);
            }

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


        // @param location relative file location within the classpath
        // @return a {@link String} {@link java.util.List} of all lines in the file
        private static List<String> readLines(String location)
        {
            List<String> lstRetval = new List<String>();

            TextReader input = getStagingInputStream(location);
            string s = "";
            while (s != null)
            {
                s = input.ReadLine();
                if (s != null) lstRetval.Add(s);
            }

            return lstRetval;
        }


        // @param location relative file location within the classpath
        // @return The TextReader resource
        private static TextReader getStagingInputStream(String location)
        {
            TextReader input = null;
            if (File.Exists(location))
            {
                FileStream fstream = File.Open(location, FileMode.Open, FileAccess.Read, FileShare.Read);
                input = new StreamReader(fstream);
            }

            if (input == null)
                throw new System.InvalidOperationException("Internal error reading file; File could not be found: " + location);

            return input;
        }


        // Return the algorithm
        // @return the algorithm
        public override String getAlgorithm()
        {
            return _algorithm.ToLower();
        }

        // Return the data version
        // @return a String representing the version
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
            StagingSchema oRetval = null;
            _schemas.TryGetValue(id, out oRetval);
            return oRetval;
        }

        public override HashSet<String> getGlossaryTerms()
        {
            return new HashSet<String>(_glossaryTerms.Keys);
        }

        public override GlossaryDefinition getGlossaryDefinition(String term)
        {
            String id = _glossaryTerms[term];
            if (id == null)
            {
                return null;
            }

            string directory = _basedir + _ALGORITHM_BASE_DIR + _algorithm.ToLower() + "\\" + _version + "\\";
            string filename = directory + "Glossary\\" + id + _JSON_EXT;

            try
            {
                TextReader reader = getStagingInputStream(filename);
                GlossaryDefinition glossaryDef = new GlossaryDefinition();
                using (reader)
                {
                    Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                    glossaryDef = (GlossaryDefinition)serializer.Deserialize(reader, typeof(GlossaryDefinition));
                }
                return glossaryDef;
            }
            catch (IOException e) 
            {
                throw new System.InvalidOperationException("Error reading glossary term: " + e.Message);
            }
        }
    }
}

