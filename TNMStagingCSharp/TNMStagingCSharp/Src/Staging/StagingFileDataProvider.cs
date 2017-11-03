// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.IO;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging
{
    // In implementation of DataProvider which holds all data in memory
    public class StagingFileDataProvider: StagingDataProvider
    {
        private String _algorithm;
        private String _version;
        private Dictionary<String, StagingTable> _tables = new Dictionary<String, StagingTable>();
        private Dictionary<String, StagingSchema> _schemas = new Dictionary<String, StagingSchema>();
        private HashSet<String> _TableKeys = new HashSet<String>();
        private HashSet<String> _SchemaKeys = new HashSet<String>();


        // Constructor loads all schemas and sets up table cache
        // @param algorithm algorithm
        // @param version version
        protected StagingFileDataProvider(String algorithm, String version): base ()
        {
            _algorithm = algorithm;
            _version = version;

            String basedir = System.IO.Directory.GetCurrentDirectory() + "\\";
            if (!Directory.Exists(basedir + "Algorithms\\"))
            {
                basedir = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\";
                if (System.IO.Directory.GetCurrentDirectory().IndexOf("x64") >= 0) basedir += "\\..\\";
                basedir += "Resources\\";
            }


            String directory = "";

            // loop over all tables and load them into Map
            try
            {
                directory = basedir + "Algorithms\\" + algorithm.ToLower() + "\\" + version + "\\tables";
                foreach (String file in readLines(directory + "\\ids.txt"))
                {
                    if (file.Length != 0)
                    {
                        TextReader reader = getStagingInputStream(directory + "\\" + file + ".json");
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
                directory = basedir + "Algorithms\\" + algorithm.ToLower() + "\\" + version + "\\schemas";

                foreach (String file in readLines(directory + "\\ids.txt"))
                {
                    if (file.Length != 0)
                    {
                        TextReader reader = getStagingInputStream(directory + "\\" + file + ".json");
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

        public override HashSet<String> getSchemaIds()
        {
            return _SchemaKeys;
        }

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

    }
}

