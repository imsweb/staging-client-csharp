// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TNMStagingCSharp.Src.DecisionEngine;
using TNMStagingCSharp.Src.Staging.Entities;


namespace TNMStagingCSharp.Src.Staging
{
    public sealed class Staging
    {
        // context keys definitions
        public static readonly String CTX_ALGORITHM_VERSION = "ctx_alg_version";
        public static readonly String CTX_YEAR_CURRENT = "ctx_year_current";

        // list of all context keys
        public static readonly ReadOnlyCollection<String> CONTEXT_KEYS = new ReadOnlyCollection<String>(new List<String>
                { CTX_ALGORITHM_VERSION, CTX_YEAR_CURRENT });


        private DecisionEngineClass _engine = null;
        private StagingDataProvider _provider = null;

        // Private constructor
        // @param provider data provider
        private Staging(StagingDataProvider provider)
        {
            _provider = provider;
            _engine = new DecisionEngineClass(provider);
        }

        // Create an instance of the Staging object with a pre-constructed provider
        // @param provider StagingDataProvider
        // @return a Staging instance
        public static Staging getInstance(StagingDataProvider provider)
        {
            return new Staging(provider);
        }

        // Return the algorithm name
        // @return an Algorithm
        public String getAlgorithm()
        {
            return _provider.getAlgorithm();
        }

        // Return the data version
        // @return a version string
        public String getVersion()
        {
            return _provider.getVersion();
        }

        // Return the list a schema ids
        // @return a Set of Strings of schema ids
        public HashSet<String> getSchemaIds()
        {
            return _provider.getSchemaIds();
        }

        // Return a schema based on schema id
        // @param id Schema identifier
        // @return an Algorithm object
        public StagingSchema getSchema(String id)
        {
            //return (_provider.getDefinition(id) as StagingSchema);
            return (StagingSchema)(_provider.getDefinition(id));
        }

        // Return true if the site is valid
        // @param site primary site
        // @return true if the side is valid
        public bool isValidSite(String site)
        {
            return _provider.isValidSite(site);
        }

        // Return true if the histology is valid
        // @param histology histology
        // @return true if the histology is valid
        public bool isValidHistology(String histology)
        {
            return _provider.isValidHistology(histology);
        }

        // Look up a schema based on site, histology and an optional discriminator.
        // @param lookup schema lookup input
        // @return a list of StagingSchema objects
        public List<StagingSchema> lookupSchema(SchemaLookup lookup)
        {
            return _provider.lookupSchema(lookup);
        }

        // Return a complete list of mapping table names
        // @return Set of String objects containing table names
        public HashSet<String> getTableIds()
        {
            return _provider.getTableIds();
        }

        // Return a mapping table based on table name
        // @param id table identifier
        // @return Table object
        public StagingTable getTable(String id)
        {
            ITable thisTable = _provider.getTable(id);
            StagingTable oRetval = (StagingTable)thisTable;
            return oRetval;
        }

        // Check the code validity of a single field in a schema.  If the schema or field do no exist, false will be returned.
        // @param schemaId schema identifier
        // @param key input key
        // @param value value to check validity
        // @return a boolean indicating whether the code exists for the the passed schema field
        public bool isCodeValid(String schemaId, String key, String value)
        {
            Dictionary<String, String> context = new Dictionary<String, String>(20, StringComparer.Ordinal);

            // add the value to the context
            if ((key == null) || (value == null) || (schemaId == null))
            {
                return false;
            }

            context[key] = value;

            // add the context keys
            addContextKeys(context);

            return isContextValid(schemaId, key, context);
        }

        // Check the validity of a single field of a schema based on the supplied context.  The value of this key should be in the context as well
        // as any other properties needed to evaluation validity.  If the schema or field do no exist, false will be returned.
        // @param schemaId schema identifier
        // @param key input key
        // @param context Map of keys/values to validate against
        // @return a boolean indicating whether the code exists for the the passed schema field
        public bool isContextValid(String schemaId, String key, Dictionary<String, String> context)
        {
            // first get the algorithm
            StagingSchema schema = getSchema(schemaId);
            if (schema == null)
                return false;

            // get the table id from the schema
            IInput input = null;
            if (!schema.getInputMap().TryGetValue(key, out input)) input = null;

            if (input == null)
                return false;

            // missing context will always return false
            if (context == null || context.Count  == 0)
                return false;

            // all context input needs to be trimmed
            Dictionary<String, String> testContext = new Dictionary<String, String>(20, StringComparer.Ordinal);
            foreach (KeyValuePair<String, String> entry in context)
                testContext[entry.Key] = (entry.Value != null ? entry.Value.Trim() : "");

            // if the input specifies a table for validation, test against it
            if (input.getTable() != null)
            {
                ITable table = getTable(input.getTable());

                return table != null && (DecisionEngineFuncs.matchTable(table, testContext) != null);
            }

            return true;
        }

        // For a given table, return the index of the row that matches the key/value combination.  If you need to match on more than
        // one input, see the other findMatchingTableRow.
        // @param tableId table identifier
        // @param key input key
        // @param value value
        // @return the index of the matching table row or null if no match was found
        public int findMatchingTableRow(String tableId, String key, String value)
        {
            Dictionary<String, String> context = new Dictionary<String, String>(1, StringComparer.Ordinal);

            context[key] = value;

            return findMatchingTableRow(tableId, context);
        }

        // For a given table, return the index of the row that matches supplied context.
        // @param tableId table identifier
        // @param context context of input key/values to use to match
        // @return the index of the matching table row or null if no match was found
        public int findMatchingTableRow(String tableId, Dictionary<String, String> context)
        {
            int rowIndex = -1;

            // add the context keys
            addContextKeys(context);

            ITable table = getTable(tableId);
            if (table != null)
                rowIndex = DecisionEngineFuncs.findMatchingTableRow(table, context);

            return rowIndex;
        }

        // Return a list of tables identifiers involved in the specified schema
        // @param schemaId schema identifier
        // @return a Set of table identifiers; if the schema is not found the set will be empty
        public HashSet<String> getInvolvedTables(String schemaId)
        {
            HashSet<String> tables = new HashSet<String>();

            StagingSchema schema = getSchema(schemaId);
            if (schema != null && schema.getInvolvedTables() != null)
                tables = schema.getInvolvedTables();

            return tables;
        }

        // Return a list of schema identifiers which the specified table is used in
        // @param tableId table identifier
        // @return a Set of schema identifiers
        public HashSet<String> getInvolvedSchemas(String tableId)
        {
            HashSet<String> schemas = new HashSet<String>();

            // loop over all schemas and find the ones that have the passed table identifier in the involved table set
            foreach (String schemaId in getSchemaIds())
            {
                if (getInvolvedTables(schemaId).Contains(tableId))
                {
                    schemas.Add(schemaId);
                }
            }

            return schemas;
        }

        // Looks at all tables involved in the table path and returns a list of input keys that could be used.  This also includes the input keys
        // used in jumps tables.
        // @param path a StagingTablePath
        // @return a Set of unique input keys
        public HashSet<String> getInputs(StagingTablePath path)
        {
            return getInputs(path, new HashSet<String>());
        }

        // Looks at all tables involved in the table path and returns a list of input keys that could be used.  This also includes the input keys
        // used in jumps tables.
        // @param path a StagingTablePath
        // @param excludedInputs a list of input keys to not consider as inputs
        // @return a Set of unique input keys
        public HashSet<String> getInputs(StagingTablePath path, HashSet<String> excludedInputs)
        {
            HashSet<String> inputs = new HashSet<String>();

            if (path != null)
            {
                inputs.UnionWith(_engine.getInputs(path, excludedInputs));
            }

            // always remove all context variables since they are never needed to be supplied
            foreach (String s in CONTEXT_KEYS)
            {
                inputs.Remove(s);
            }

            return inputs;
        }

        // Looks at all tables involved in the mapping and returns a list of input keys that could be used.  This also includes the input keys
        // used in the inclusion and exclusion tables if any.
        // @param mapping a StagingMapping
        // @return a Set of unique input keys
        public HashSet<String> getInputs(StagingMapping mapping)
        {
            return getInputs(mapping, null, new HashSet<String>());
        }

        // Looks at all tables involved in the mapping and returns a list of input keys that could be used.  This always includes the input keys
        // used in the inclusion and exclusion tables if any.  The inputs from each table path will only be included if it passes the inclusion/exclusion
        // criteria based on the context.
        // @param mapping a StagingMapping
        // @param context a context of values used to to check mapping inclusion/exclusion
        // @param excludedInputs a list of input keys to not consider as inputs
        // @return a Set of unique input keys
        public HashSet<String> getInputs(StagingMapping mapping, Dictionary<String, String> context, HashSet<String> excludedInputs)
        {
            HashSet<String> inputs = new HashSet<String>();

            // the inclusion tables are always evaluated so any inputs there should be added always
            if (mapping.getInclusionTables() != null)
                foreach (StagingTablePath path in mapping.getInclusionTables())
                    inputs.UnionWith(getInputs(path, excludedInputs));

            // the exclusion tables are always evaluated so any inputs there should be added always
            if (mapping.getExclusionTables() != null)
                foreach (StagingTablePath path in mapping.getExclusionTables())
                    inputs.UnionWith(getInputs(path, excludedInputs));

            // if there are tables paths and if the mapping is involved, add the inputs
            if (mapping.getTablePaths() != null)
                if (context == null || _engine.isMappingInvolved(mapping, context))
                    inputs.UnionWith(_engine.getInputs(mapping, excludedInputs));

            // always remove all context variables since they are never needed to be supplied
            inputs.ExceptWith(CONTEXT_KEYS);

            return inputs;
        }

        // Looks at all tables involved in all the mappings in the definition and returns a list of input keys that could be used.  It will also deal with mapped
        // inputs.  Note that if an input to a table was not a supplied input (i.e. it was created as an output of a previous table) it will not be included
        // in the list of inputs.  The inputs will also include any used in schema selection.  All inputs returned from this method should be in the schema input
        // list otherwise there is a problem with the schema.
        // @param schema a StagingSchema
        // @return a Set of unique input keys
        public HashSet<String> getInputs(StagingSchema schema)
        {
            return getInputs(schema, null);
        }

        // Looks at all tables involved in all the mappings in the definition and returns a list of input keys that could be used.  It will also deal with mapped
        // inputs.  The inputs from each mapping will only be included if it passes the inclusion/exclusion criteria based on the context.  Note that if an input
        // to a table was not a supplied input (i.e. it was created as an output of a previous table) it will not be included in the list of inputs.  The inputs will
        // also include any used in schema selection.  All inputs returned from this method should be in the schema input list otherwise there is a problem with the
        // schema.
        // @param schema a StagingSchema
        // @param context a context of values used to to check mapping inclusion/exclusion
        // @return a Set of unique input keys
        public HashSet<String> getInputs(StagingSchema schema, Dictionary<String, String> context)
        {
            HashSet<String> inputs = new HashSet<String>();

            // add schema selection fields
            if (schema.getSchemaSelectionTable() != null)
            {
                StagingTable table = getTable(schema.getSchemaSelectionTable());
                if (table != null)
                {
                    foreach (StagingColumnDefinition def in table.getColumnDefinitions())
                        if (ColumnType.INPUT == def.getType())
                            inputs.Add(def.getKey());
                }
            }

            // process all mappings
            if (schema.getMappings() != null)
            {
                HashSet<String> excludedInputs = new HashSet<String>();
                HashSet<String> thisInput = null;
                foreach (StagingMapping mapping in schema.getMappings())
                {
                    thisInput = getInputs(mapping, context, excludedInputs);
                    inputs.UnionWith(thisInput);
                }
            }

            // always remove all context variables since they are never needed to be supplied
            inputs.ExceptWith(CONTEXT_KEYS);

            return inputs;
        }

        // Looks at a table path (and all jump tables within in) and returns a list of output keys that could be created.
        // @param path a StagingTablePath
        // @return a Set of unique output keys
        public HashSet<String> getOutputs(StagingTablePath path)
        {
            return _engine.getOutputs(path);
        }

        // Looks at all tables involved in the mapping and returns a list of output keys that could be created.  This also includes the output keys
        // used in the inclusion and exclusion tables if any.
        // @param mapping a StagingMapping
        // @return a Set of unique output keys
        public HashSet<String> getOutputs(StagingMapping mapping)
        {
            return getOutputs(mapping, null);
        }

        // Looks at all tables involved in the mapping and returns a list of output keys that could be created.  This also includes the output keys
        // used in the inclusion and exclusion tables if any.  The outputs from each mapping will only be included if it passes the inclusion/exclusion
        // criteria based on the context.
        // @param mapping a StagingMapping
        // @param context a context of values used to to check mapping inclusion/exclusion
        // @return a Set of unique output keys
        public HashSet<String> getOutputs(StagingMapping mapping, Dictionary<String, String> context)
        {
            HashSet<String> outputs = new HashSet<String>();

            if (mapping.getTablePaths() != null)
            {
                if (context == null || _engine.isMappingInvolved(mapping, context))
                    outputs.UnionWith(_engine.getOutputs(mapping));
            }

            return outputs;
        }

        // Looks at all tables involved in all the mappings in the definition and returns a list of output keys that could be created.  It will also deal with mapped
        // outputs.
        // @param schema a StagingSchema
        // @return a Set of unique output keys
        public HashSet<String> getOutputs(StagingSchema schema)
        {
            return getOutputs(schema, null);
        }

        // Looks at all tables involved in all the mappings in the definition and returns a list of output keys that will be created.  It will also deal with mapped
        // outputs.  The outputs from each mapping will only be included if it passes the inclusion/exclusion criteria based on the context.  If the schema has StagingOutputs
        // defined, then the calulated output list is exactly the same as the schema output list.
        // @param schema a StagingSchema
        // @param context a context of values used to to check mapping inclusion/exclusion
        // @return a Set of unique output keys
        public HashSet<String> getOutputs(StagingSchema schema, Dictionary<String, String> context)
        {
            HashSet<String> outputs = new HashSet<String>();

            // if outputs are defined in the schema, then there is no reason to look any further into the mappings; the output defines exactly what keys will
            // be returned and it doesn't matter what context is passed in that case
            if (schema.getOutputMap() != null)
            {
                foreach (KeyValuePair<String, IOutput> entry in schema.getOutputMap())
                    outputs.Add(entry.Key);

                return outputs;
            }

            // if outputs were not defined, then the tables involved in the mappings will be used to determine the possible outputs

            if (schema.getMappings() != null)
                foreach (StagingMapping mapping in schema.getMappings())
                    outputs.UnionWith(getOutputs(mapping, context));

            // if valid outputs are defined on the schema level, only return outputs that defined; this removed "temporary" outputs that may be defined during the
            // staging process
            List<String> lstKeysToRemove = new List<String>();
            foreach (String entry in outputs)
            {
                if (!schema.getOutputMap().ContainsKey(entry))
                {
                    lstKeysToRemove.Add(entry);
                }
            }

            foreach (String sKeyName in lstKeysToRemove)
            {
                outputs.Remove(sKeyName);
            }

            return outputs;
        }

        // Stage the passed case.
        // @param data all input values are passed through this database
        // @return the same StagingData with output values filled in
        public StagingData stage(StagingData data)
        {
            // first clear out schema/output/errors/path
 
            data.setSchemaId(null);
            data.setOutput(new Dictionary<String, String>(100, StringComparer.Ordinal));
            data.setErrors(new List<Error>(100));
            data.setPath(new List<String>(100));

            // make sure site and histology are supplied
            if (data.getInput(StagingData.PRIMARY_SITE_KEY) == null || data.getInput(StagingData.HISTOLOGY_KEY) == null)
            {
                data.setResult(StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY);
                return data;
            }
             
            // get the schema; if a single schema is not found, return right away with an error
            List<StagingSchema> schemas = lookupSchema(new SchemaLookup(data.getInput()));
            if (schemas.Count != 1)
            {
                if (schemas.Count == 0)
                    data.setResult(StagingData.Result.FAILED_NO_MATCHING_SCHEMA);
                else
                    data.setResult(StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS);
                return data;
            }

            StagingSchema schema = null;
            if (schemas.Count > 0) schema = schemas[0];

            // add schema id to result
            data.setSchemaId(schema.getId());

            // copy the input into a new context
            Dictionary<String, String> context = new Dictionary<String, String>(data.getInput(), StringComparer.Ordinal);

            // make sure all supplied inputs are defined in the definition
            foreach (KeyValuePair<String, String> entry in context)
                if (!schema.getInputMap().ContainsKey(entry.Key))
                    data.addError(new Error.ErrorBuilder(Error.Type.UNKNOWN_INPUT).message("Unknown input key supplied: " + entry.Key).key(entry.Key).build());

            if (data.getErrors().Count > 0)
            {
                data.setResult(StagingData.Result.FAILED_INVALID_INPUT);
                return data;
            }

            // add context variables
            addContextKeys(context);

            // check that year of DX is valid
            if (!isContextValid(schema.getId(), StagingData.YEAR_DX_KEY, context))
            {
                data.setResult(StagingData.Result.FAILED_INVALID_YEAR_DX);
                return data;
            }

            // perform the staging
            Result result = _engine.process(schemas[0].getId(), context);

            // remove the context variables
            removeContextKeys(context);

            // set the staging data result based on the Result returned from the DecisionEngine
            if (Result.Type.FAILED_INPUT == result.getType())
                data.setResult(StagingData.Result.FAILED_INVALID_INPUT);
            else
                data.setResult(StagingData.Result.STAGED);

            // remove the original input keys from the resulting context;  in addition, we want to remove any input keys
            // from the resulting context that were set with a default value; to accomplish this remove all keys that are
            // defined as input in the selected schema
            foreach (KeyValuePair<String, String> entry in data.getInput())
                context.Remove(entry.Key);
            foreach (StagingSchemaInput input in schemas[0].getInputs())
                context.Remove(input.getKey());

            // add the results to the data card
            data.setOutput(result.getContext());
            data.setErrors(result.getErrors());
            data.setPath(result.getPath());

            return data;
        }

        // Add the context keys which are used in staging and other calls
        // @param context Map of context
        // @return updated Map of context
        private Dictionary<String, String> addContextKeys(Dictionary<String, String> context)
        {
            // make the algorithm version available in the context
            context[CTX_ALGORITHM_VERSION] = getVersion();

            // put the current year in the context
            context[CTX_YEAR_CURRENT] = DateTime.Now.Year.ToString();

            return context;
        }

        // Remove all context keys
        // @param context Map of context
        // @return updated Map of context
        private Dictionary<String, String> removeContextKeys(Dictionary<String, String> context)
        {
            context.Remove(CTX_YEAR_CURRENT);

            return context;
        }
    }
}


