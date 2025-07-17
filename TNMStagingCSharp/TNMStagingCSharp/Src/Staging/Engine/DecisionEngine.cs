// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

using TNMStagingCSharp.Src.Staging.Entities;
using static TNMStagingCSharp.Src.Staging.Entities.Error;

namespace TNMStagingCSharp.Src.Staging.Engine
{
    public class DecisionEngineFuncs
    {
        private readonly static Regex _TEMPLATE_REFERENCE = new Regex("\\{\\{(.*?)\\}\\}", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        // string to use for blank or null in error strings
        public static readonly String _BLANK_OUTPUT = "<blank>";

        public static readonly String _CONTEXT_MISSING_MESSAGE = "Context must not be missing";

        //========================================================================================================================
        // Checked whether the value is a reference to another variable or context
        // @param value String value
        // @return true if the value is a reference to another variable or context
        //========================================================================================================================
        public static bool isReferenceVariable(String value)
        {
            return value != null && value.StartsWith("{{") && value.EndsWith("}}");
        }


        //========================================================================================================================
        // Takes a key reference, like {{key}} and returns just the key ("key" in this example)
        // @param value a key refrerence
        // @return the inner key
        //========================================================================================================================
        public static String trimBraces(String value)
        {
            if (value.Length > 3)
                return value.Substring(2, value.Length - 4);
            else
                return value;
        }


        //========================================================================================================================
        // Return the list of endpoints for the matching row in the table; returns null if there is no match
        // @param table a Table
        // @param context a Map containing the context
        // @return returns a List of Endpoint entities from the matching row or null if no match
        //========================================================================================================================
        public static List<IEndpoint> matchTable(ITable table, Dictionary<String, String> context)
        {
            return matchTable(table, context, null);
        }


        //========================================================================================================================
        // Return the list of endpoints for the matching row in the table; returns null if there is no match
        // @param table a Table
        // @param context a Map containing the context
        // @param keysToMatch if not null, only keys in this set will be matched against
        // @return returns a List of Endpoint entities from the matching row or null if no match
        //========================================================================================================================
        public static List<IEndpoint> matchTable(ITable table, Dictionary<String, String> context, HashSet<String> keysToMatch)
        {
            List<IEndpoint> endpoints = null;

            int index = findMatchingTableRow(table, context, keysToMatch);

            if (index >= 0)
                endpoints = table.getTableRows()[index].getEndpoints();

            return endpoints;
        }


        //========================================================================================================================
        // Return the matching table row index based on the passed context
        // @param table a Table
        // @param context a Map containing the context
        // @return the index of the matching table row or null if no match was found
        //========================================================================================================================
        public static int findMatchingTableRow(ITable table, Dictionary<String, String> context)
        {
            return findMatchingTableRow(table, context, null);
        }


        //========================================================================================================================
        // Return the matching table row index based on the passed context
        // @param table a Table
        // @param context a Map containing the context
        // @param keysToMatch if not null, only keys in this set will be matched against
        // @return the index of the matching table row or null if no match was found
        //========================================================================================================================
        public static int findMatchingTableRow(ITable table, Dictionary<String, String> context, HashSet<String> keysToMatch)
        {
            int rowIndex = -1;

            if (context == null)
                throw new System.InvalidOperationException(_CONTEXT_MISSING_MESSAGE);

            List<ITableRow> pRows = table.getTableRows();
            List<IColumnDefinition> pColDefs = table.getInputColumnDefinitions();
            int iRowCount = pRows.Count;
            int iColCount = pColDefs.Count;

            String sColKey = "";
            String sContextValue = "";
            IColumnDefinition col = null;
            bool matchAll = false;

            for (int i = 0; i < iRowCount; i++)
            {
                matchAll = true;
                for (int c = 0; c < iColCount; c++)
                {
                    col = pColDefs[c];
                    sColKey = col.getKey();
                    if ((keysToMatch == null || keysToMatch.Contains(sColKey)))
                    {
                        context.TryGetValue(sColKey, out sContextValue);
                        matchAll = testMatch(pRows[i].getColumnInput(sColKey), sContextValue, context);
                    }
                    if (!matchAll)
                        break;
                }

                // if all inputs match, we are done
                if (matchAll)
                {
                    rowIndex = i;
                    break;
                }
            }

            return rowIndex;
        }

        //========================================================================================================================
        // Tests that a value is contained in a list of ranges; if the list of ranges is missing or empty, then all values will match to it
        // @param values a List of Range objects
        // @param value a value to look for
        // @param context the context will be used to do key lookups when values are in the format of {{var}}
        // @return return true if the value is contained in the List of Range objects
        //========================================================================================================================
        public static bool testMatch(List<Range> values, String value, Dictionary<String, String> context)
        {
            if (values == null)
            {
                return true;
            }
            else
            {
                int iCount = values.Count;
                if (iCount == 0)
                {
                    return true;
                }
                else
                {
                    for (int i = 0; i < iCount; i++)
                    {
                        if (values[i].contains(value, context)) return true;
                    }
                    return false;
                }

            }

        }


        //========================================================================================================================
        // Translates a value.  If it is a reference to a context, like {{var}} it will return the context value; otherwise
        // if will return the value unchanged.  If the context key does not exist in the context, blank will be returned
        // @param value String value
        // @param context Context for handling variable references
        // @return the context value if a reference, otherwise the original value is returned
        //========================================================================================================================
        public static String translateValue(String value, Dictionary<String, String> context)
        {
            if (value != null && value.StartsWith("{{"))
            {
                Match match = _TEMPLATE_REFERENCE.Match(value);
                if (match.Success)
                {
                    String referencedKey = match.Groups[1].Value;
                    if (!context.TryGetValue(referencedKey, out value))
                        value = "";
                }
            }
            return value;
        }


        // Return a comma-separated list of input values the table needs taken from the passed context.  Used for error message.
        // @param table a Table
        // @param context a Map of context
        // @return a String representing the input for the table
        public static String getTableInputsAsString(ITable table, Dictionary<String, String> context)
        {
            int iNumCols = 0;
            if (table.getColumnDefinitions() != null) iNumCols = table.getColumnDefinitions().Count;

            List<String> inputs = new List<String>(iNumCols);

            if (table.getColumnDefinitions() != null)
                foreach (IColumnDefinition def in table.getColumnDefinitions())
                    if (ColumnType.INPUT == def.getType())
                    {
                        String value = "";
                        if (context.ContainsKey(def.getKey()))
                            value = context[def.getKey()];

                        inputs.Add(((value == null || value.Trim().Length == 0) ? _BLANK_OUTPUT : value.Trim()));
                    }

            StringBuilder MyStringBuilder = new StringBuilder("");
            for (int i = 0; i < inputs.Count; i++)
            {
                if (MyStringBuilder.Length > 0) MyStringBuilder.Append(",");
                MyStringBuilder.Append(inputs[i]);
            }

            inputs = null;
            return MyStringBuilder.ToString();
        }

    }


    // An engine for processing declarative algorithms.
    public class DecisionEngineClass
    {
        private IDataProvider _provider;

        //========================================================================================================================
        // Construct the decision engine with the passed data provider
        // @param provider a DataProvider
        //========================================================================================================================
        public DecisionEngineClass(IDataProvider provider)
        {
            setProvider(provider);
        }

        //========================================================================================================================
        // Returns the internal data provider
        // @return a DataProvider
        //========================================================================================================================
        public IDataProvider getProvider()
        {
            return _provider;
        }


        //========================================================================================================================
        // Sets the provider and initiaizes all definitions and tables
        // @param provider a DataProvider
        //========================================================================================================================
        public void setProvider(IDataProvider provider)
        {
            _provider = provider;
        }


        //========================================================================================================================
        // Given a mapping and a context, check the inclusion/exclusion tables to see if mapping should be processed
        // @param mapping a Mapping
        // @param context a Map containing the context
        // @return true if the mapping is involved
        //========================================================================================================================
        public bool isMappingInvolved(IMapping mapping, Dictionary<String, String> context)
        {
            if (context == null)
                throw new System.InvalidOperationException(DecisionEngineFuncs._CONTEXT_MISSING_MESSAGE);

            bool matches = true;

            List<ITablePath> pIncTables = mapping.getInclusionTables();
            ITablePath path = null;

            Dictionary<String, String> pathContext = null;

            // process inclusion table if it exists
            if (mapping.getInclusionTables() != null)
            {

                for (int i=0; i < pIncTables.Count; i++)
                {
                    path = pIncTables[i];

                    // make a copy of the context so mapping changes are only included for a single table path
                    pathContext = new Dictionary<String, String>(context, StringComparer.Ordinal);

                    ITable table = getProvider().getTable(path.getId());
                    if (table == null)
                        throw new System.InvalidOperationException("Inclusion table '" + path.getId() + "' does not exist");
                    else
                    {
                        // if there is input mapping defined, add the new mapping to the context
                        if (path.getInputMapping() != null)
                        {
                            String sPathFrom = "";
                            foreach (IKeyMapping key in path.getInputMapping())
                            {
                                if (pathContext.TryGetValue(key.getFrom(), out sPathFrom))
                                {
                                    pathContext[key.getTo()] = sPathFrom;
                                }
                            }
                        }

                        matches = (DecisionEngineFuncs.matchTable(table, pathContext) != null);
                    }

                    // stop processing if any inclusion not met
                    if (!matches)
                    {
                        break;
                    }
                }
            }

            // process exclusion table if it exists
            List<ITablePath> pExcTables = mapping.getExclusionTables();

            if (matches && pExcTables != null)
            {

                for (int i = 0; i < pExcTables.Count; i++)
                {
                    path = pExcTables[i];

                    // make a copy of the context so mapping changes are only included for a single table path
                    pathContext = new Dictionary<String, String>(context, StringComparer.Ordinal);

                    ITable table = getProvider().getTable(path.getId());
                    if (table == null)
                        throw new System.InvalidOperationException("Exclusion table '" + path.getId() + "' does not exist");
                    else
                    {
                        // if there is input mapping defined, add the new mapping to the context
                        if (path.getInputMapping() != null)
                        {

                            String sPathFrom = "";
                            foreach (IKeyMapping key in path.getInputMapping())
                            {
                                if (pathContext.TryGetValue(key.getFrom(), out sPathFrom))
                                {
                                    pathContext[key.getTo()] = sPathFrom;
                                }
                            }
                        }

                        matches = (DecisionEngineFuncs.matchTable(table, pathContext) == null);
                    }

                    // stop processing if any exclusion met
                    if (!matches)
                    {
                        break;
                    }
                }
            }

            pathContext = null;

            return matches;
        }


        //========================================================================================================================
        // Given a schema and context, return a list of mappings that match inclusion and exclusion criteria
        // Given a schema and context, return a list of mappings that match inclusion and exclusion criteria
        // @param schema a Schema
        // @param context a Map containing the context
        // @return a List of involved Mapping entities
        //========================================================================================================================
        public List<IMapping> getInvolvedMappings(Schema schema, Dictionary<String, String> context)
        {
            List<IMapping> mappings = new List<IMapping>();

            if (context == null)
                throw new System.InvalidOperationException(DecisionEngineFuncs._CONTEXT_MISSING_MESSAGE);

            if (schema.getMappings() != null)
            {
                foreach (IMapping mapping in schema.getMappings())
                    if (isMappingInvolved(mapping, context))
                        mappings.Add(mapping);
            }

            return mappings;
        }


        //========================================================================================================================
        // Return a list of tables involved in a schema
        // @param schemaId an schema identifier
        // @return a set of table identifiers
        //========================================================================================================================
        public HashSet<String> getInvolvedTables(String schemaId)
        {
            Schema schema = getProvider().getSchema(schemaId);

            if (schema == null)
                throw new System.InvalidOperationException("Unknown starting table: '" + schemaId + "'");

            return getInvolvedTables(schema);
        }


        //========================================================================================================================
        // Return a list of tables involved in an schema.This includes not only the tables paths, but also tables references in the input section.
        // @param schema a schema
        // @return a set of table identifiers
        //========================================================================================================================
        public HashSet<String> getInvolvedTables(Schema schema)
        {
            HashSet<String> tables = new HashSet<String>();

            // first, evaluate inputs and outputs
            foreach (String key in schema.getInputMap().Keys)
            {
                IInput input = schema.getInputMap()[key];
                if (input.getTable() != null)
                    getInvolvedTables(getProvider().getTable(input.getTable()), tables);
                if (input.getDefaultTable() != null)
                    getInvolvedTables(getProvider().getTable(input.getDefaultTable()), tables);
            }
            foreach (String key in schema.getOutputMap().Keys)
            {
                IOutput output = schema.getOutputMap()[key];
                if (output.getTable() != null)
                    getInvolvedTables(getProvider().getTable(output.getTable()), tables);
            }

            // next loop over mappings and paths
            if (schema.getMappings() != null)
            {
                foreach (IMapping mapping in schema.getMappings())
                {
                    // handle inclusion tables
                    if (mapping.getInclusionTables() != null)
                        foreach (ITablePath path in mapping.getInclusionTables())
                            getInvolvedTables(getProvider().getTable(path.getId()), tables);

                    // handle exclusion tables
                    if (mapping.getExclusionTables() != null)
                        foreach (ITablePath path in mapping.getExclusionTables())
                            getInvolvedTables(getProvider().getTable(path.getId()), tables);

                    // handle table paths
                    if (mapping.getTablePaths() != null)
                        foreach (ITablePath path in mapping.getTablePaths())
                            getInvolvedTables(getProvider().getTable(path.getId()), tables);
                }
            }

            return tables;
        }


        //========================================================================================================================
        // Internal recursive helper function to find the tables that could be called from within a table, stepping through all JUMPs
        // @param table a Table
        // @param tables a Set of Strings representing the involved table identifiers
        // @return the same Set that was passed in, with possibly extra table identifiers added
        //========================================================================================================================
        private HashSet<String> getInvolvedTables(ITable table, HashSet<String> tables)
        {
            if (table == null)
                return tables;

            tables.Add(table.getId());

            if (table.getTableRows() != null)
                foreach (ITableRow tableRow in table.getTableRows())
                {
                    foreach (IEndpoint endpoint in tableRow.getEndpoints())
                    {
                        if (endpoint != null && EndpointType.JUMP == endpoint.getType())
                        {
                            // if table has already been visited, don't call getInvolvedTables again; otherwise we could have infinite recursion
                            if (!tables.Contains(endpoint.getValue()))
                                getInvolvedTables(getProvider().getTable(endpoint.getValue()), tables);
                        }
                    }
                }

            return tables;
        }


        //========================================================================================================================
        // Returns a list of inputs that are required for the specified TablePath.  This method will deal with mapped inputs.
        // Note that if an output key is added during the mapping and used as an input in one of the later tables, we do not want
        // to include it in the final list of inputs.  Order matters here since if the key was already used as an input before being
        // re-mapped, then it is still considered an input, otherwise if should be excluded.
        // @param path a TablePath
        // @return a Set of unique inputs
        //========================================================================================================================
        public HashSet<String> getInputs(ITablePath path)
        {
            return getInputs(path, new HashSet<String>());
        }


        //========================================================================================================================
        // Returns a list of inputs that are required for the specified TablePath.  This method will deal with mapped inputs.
        // Note that if an output key is added during the mapping and used as an input in one of the later tables, we do not want
        // to include it in the final list of inputs.  Order matters here since if the key was already used as an input before being
        // re-mapped, then it is still considered an input, otherwise if should be excluded.
        // @param path a TablePath
        // @param excludedInputs a list of keys that should not be included in the inputs
        // @return a Set of unique inputs
        //========================================================================================================================
        public HashSet<String> getInputs(ITablePath path, HashSet<String> excludedInputs)
        {
            HashSet<String> inputs = new HashSet<String>();

            if (path != null)
            {
                Dictionary<String, String> inputMappings = new Dictionary<String, String>(100, StringComparer.Ordinal);

                if (path.getInputMapping() != null)
                    foreach (IKeyMapping keymapping in path.getInputMapping())
                        inputMappings[keymapping.getTo()] = keymapping.getFrom();
                Dictionary<String, String> outputMappings = new Dictionary<String, String>(100, StringComparer.Ordinal);
                if (path.getOutputMapping() != null)
                    foreach (IKeyMapping keymapping in path.getOutputMapping())
                        outputMappings[keymapping.getFrom()] = keymapping.getTo();

                // process the table (and any "JUMP" tables) for the mapping
                foreach (String tableId in getInvolvedTables(getProvider().getTable(path.getId()), new HashSet<String>()))
                {
                    ITable table = getProvider().getTable(tableId);
                    if (table != null)
                    {
                        // first process the inputs from the column definitions
                        if (table.getColumnDefinitions() != null)
                        {
                            foreach (IColumnDefinition def in table.getColumnDefinitions())
                            {
                                if (ColumnType.INPUT == def.getType())
                                {
                                    String inputKey = inputMappings.ContainsKey(def.getKey()) ? inputMappings[def.getKey()] : def.getKey();
                                    if (!excludedInputs.Contains(inputKey))
                                        inputs.Add(inputKey);
                                }
                                else if (ColumnType.ENDPOINT == def.getType())
                                {
                                    String outputKey = outputMappings.ContainsKey(def.getKey()) ? outputMappings[def.getKey()] : def.getKey();
                                    if (!inputs.Contains(outputKey))
                                        excludedInputs.Add(outputKey);
                                }
                            }
                        }

                        // next add any inputs that are referenced in the table rows, i.e. format of {{key}}
                        if (table.getExtraInput() != null)
                        {
                            String sNewInputKey = "";
                            foreach (String inputKey in table.getExtraInput())
                            {
                                sNewInputKey = inputKey;
                                // variable references need to use input mappings as well

                                if (inputMappings.ContainsKey(inputKey))
                                    sNewInputKey = inputMappings[inputKey];

                                if (!excludedInputs.Contains(sNewInputKey))
                                    inputs.Add(sNewInputKey);
                            }
                        }
                    }
                }
            }

            return inputs;
        }


        //========================================================================================================================
        // Looks at all tables involved in the mapping and returns a list of inputs that are used.  This also includes the inputs
        // used in the inclusion and exclusion tables if any.
        // @param mapping a Mapping
        // @param excludedInputs a list of keys that should not be included in the inputs
        // @return a Set of unique inputs
        //========================================================================================================================
        public HashSet<String> getInputs(IMapping mapping, HashSet<String> excludedInputs)
        {
            HashSet<String> inputs = new HashSet<String>();

            // if any fields are added in the initial context, they should not be considered inputs since their value is set
            if (mapping.getInitialContext() != null)
                foreach (IKeyValue kv in mapping.getInitialContext())
                    excludedInputs.Add(kv.getKey());

            // handle inclusion tables if any
            if (mapping.getInclusionTables() != null)
                foreach (ITablePath path in mapping.getInclusionTables())
                    inputs.UnionWith(getInputs(path, excludedInputs));

            // handle exclusion tables if any
            if (mapping.getExclusionTables() != null)
                foreach (ITablePath path in mapping.getExclusionTables())
                    inputs.UnionWith(getInputs(path, excludedInputs));

            // handle table paths if any
            if (mapping.getTablePaths() != null)
                foreach (ITablePath path in mapping.getTablePaths())
                    inputs.UnionWith(getInputs(path, excludedInputs));

            return inputs;
        }


        //========================================================================================================================
        // Looks at all tables involved in all the mappings in the schema and returns a list of inputs that are used.It will 
        // also deal with mapped inputs.
        // @param schema a schema
        // @return a Set of Strings containing the unique schema input keys
        //========================================================================================================================
        public HashSet<String> getInputs(Schema schema)
        {
            HashSet<String> inputs = new HashSet<String>();
            HashSet<String> excludedInputs = new HashSet<String>();

            if (schema.getMappings() != null)
                foreach (IMapping mapping in schema.getMappings())
                    inputs.UnionWith(getInputs(mapping, excludedInputs));

            return inputs;
        }


        // Return a list of outputs that are produced form the specified TablePath.  It will also handle mapped outputs.
        // @param path a TablePath
        // @return a Set of Strings containing the unique Mapping output keys
        public HashSet<String> getOutputs(ITablePath path)
        {
            HashSet<String> outputs = new HashSet<String>();

            if (path != null)
            {
                // build map of from key -> to key
                Dictionary<String, String> mappings = new Dictionary<String, String>(100, StringComparer.Ordinal);
                if (path.getOutputMapping() != null)
                    foreach (IKeyMapping keymapping in path.getOutputMapping())
                        mappings.Add(keymapping.getFrom(), keymapping.getTo());

                foreach (String tableId in getInvolvedTables(getProvider().getTable(path.getId()), new HashSet<String>()))
                {
                    ITable table = getProvider().getTable(tableId);
                    if (table != null && table.getColumnDefinitions() != null)
                    {
                        foreach (IColumnDefinition def in table.getColumnDefinitions())
                        {
                            if (ColumnType.ENDPOINT == def.getType() && def.getKey() != null)
                                outputs.Add(mappings.ContainsKey(def.getKey()) ? mappings[def.getKey()] : def.getKey());
                        }
                    }
                }
            }

            return outputs;
        }


        //========================================================================================================================
        // Looks at all tables involved in the mapping and returns a list of outputs that are produced.  It will also handle mapped outputs.  Since
        // inclusion/exclusion tables should not map any new values, they are not included in the calculation.
        // @param mapping a Mapping
        // @return a Set of Strings containing the unique Mapping output keys
        //========================================================================================================================
        public HashSet<String> getOutputs(IMapping mapping)
        {
            HashSet<String> outputs = new HashSet<String>();

            if (mapping.getTablePaths() != null)
                foreach (ITablePath path in mapping.getTablePaths())
                    outputs.UnionWith(getOutputs(path));

            return outputs;
        }


        //========================================================================================================================
        // Looks at all tables involved in all the mappings in the schema and returns a list of outputs produced.It will 
        // also handle mapped outputs.
        // @param schema a schema
        // @return a Set of Strings containing the unique Mapping output keys
        //========================================================================================================================
        public HashSet<String> getOutputs(Schema schema)
        {
            HashSet<String> outputs = new HashSet<String>();

            if (schema.getMappings() != null)
                foreach (IMapping mapping in schema.getMappings())
                    outputs.UnionWith(getOutputs(mapping));

            return outputs;
        }

        //========================================================================================================================
        // Calculates the default value for an Input using supplied context
        // @param input Input definition
        // @param context a Map containing the context
        // @param result a Result object to store errors
        // @return the default value for the input or blank if there is none
        //========================================================================================================================
        public String getDefault(IInput input, Dictionary<String, String> context, Result result)
        {
            String value = "";

            if (input.getDefault() != null)
            {
                value = DecisionEngineFuncs.translateValue(input.getDefault(), context);
            }
            else if (input.getDefaultTable() != null)
            {
                ITable defaultTable = getProvider().getTable(input.getDefaultTable());
                if (defaultTable == null)
                {
                    result.addError(new ErrorBuilder(Error.Type.UNKNOWN_TABLE).message("Default table does not exist: " + input.getDefaultTable()).key(input.getKey()).build());
                    return value;
                }

                // look up default value from table
                IEnumerable<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(defaultTable, context);
                if (endpoints != null)
                {
                    value = null;
                    foreach (IEndpoint end in endpoints)
                    {
                        if (value == null)
                        {
                            if (end.getType().Equals(EndpointType.VALUE) &&
                                end.getResultKey().Equals(input.getKey()))
                            {
                                value = DecisionEngineFuncs.translateValue(end.getValue(), context);
                            }
                        }
                    }
                    /*
                    value = endpoints.stream()
                    .filter(endpoint->EndpointType.VALUE.equals(endpoint.getType()))
                    .filter(endpoint->endpoint.getResultKey().equals(input.getKey()))
                    .map(endpoint->translateValue(endpoint.getValue(), context))
                            .findFirst()
                            .orElse(null);
                    */
                }

                // if no match found, report the error
                if (endpoints == null || value == null)
                {
                    result.addError(new ErrorBuilder(Error.Type.MATCH_NOT_FOUND)
                            .message("Default table " + input.getDefaultTable() + " did not find a match")
                            .key(input.getKey())
                            .build());
                    return "";
                }
            }

            return value;
        }


        //========================================================================================================================
        // Using the supplied context, process an schema.The results will be added to the context.
        // @param schemaId an schema identifier
        // @param context a Map containing the context
        // @return a Result
        //========================================================================================================================
        public Result process(String schemaId, Dictionary<String, String> context)
        {
            Schema start = getProvider().getSchema(schemaId);

            if (start == null)
                throw new System.InvalidOperationException("Unknown schema: '" + schemaId + "'");

            return process(start, context);
        }


        //========================================================================================================================
        // Using the supplied context, process a schema.The results will be added to the context.
        // @param schema a schema
        // @param context a Map containing the context
        // @return a Result
        //========================================================================================================================
        public Result process(Schema schema, Dictionary<String, String> context)
        {
            Result result = new Result(context);

            // trim all context Strings; " " will match ""
            List<String> lstKeys = new List<String>(context.Count);
            List<String> lstValues = new List<String>(context.Count);
            foreach (KeyValuePair<String, String> entry in context)
                if (entry.Value != null)
                {
                    lstKeys.Add(entry.Key);
                    lstValues.Add(entry.Value.Trim());
                }
            for (int i=0; i < lstKeys.Count; i++)
            {
                context[lstKeys[i]] = lstValues[i];
            }


            // validate inputs
            bool stopForBadInput = false;
            foreach (String key in schema.getInputMap().Keys)
            {
                IInput input = schema.getInputMap()[key];

                String value = null;
                if (context.ContainsKey(input.getKey()))
                    value = context[input.getKey()];


                // if value not supplied, use the default and set it back into the context; if not supplied and no default, set the input the blank
                /*
                if (value == null)
                {
                    value = (input.getDefault() != null ? DecisionEngineFuncs.translateValue(input.getDefault(), context) : "");
                    context[input.getKey()] = value;
                }
                */
                // if value not supplied, use the default or defaultTable and set it back into the context; if not supplied and no default, set the input the blank
                if (value == null)
                {
                    context[input.getKey()] = getDefault(input, context, result);
                }

                // validate value against associated table if supplied; if a value is not supplied, or blank, there is no need to validate it against the table
                if (value != null && value.Length > 0 && input.getTable() != null)
                {
                    ITable lookup = getProvider().getTable(input.getTable());
                    if (lookup == null)
                    {
                        result.addError(new Error.ErrorBuilder(Error.Type.UNKNOWN_TABLE).message("Input table does not exist: " + input.getTable()).key(input.getKey()).build());
                        continue;
                    }

                    IEnumerable<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(lookup, context);
                    if (endpoints == null)
                    {
                        result.addError(new Error.ErrorBuilder(input.getUsedForStaging() ? Error.Type.INVALID_REQUIRED_INPUT : Error.Type.INVALID_NON_REQUIRED_INPUT).message(
                                "Invalid '" + input.getKey() + "' value (" + (value.Length == 0 ? DecisionEngineFuncs._BLANK_OUTPUT : value) + ")").key(input.getKey()).table(input.getTable()).build());

                        // if the schema error handling is set to FAIL or if the input is required for staging and the error handling is set to FAIL_WHEN_REQUIRED_FOR_STAGING,
                        // then stop processing and return a failure result
                        if ((StagingInputErrorHandler.FAIL == schema.getOnInvalidInput()) || (input.getUsedForStaging()
                                && (StagingInputErrorHandler.FAIL_WHEN_USED_FOR_STAGING == schema.getOnInvalidInput())))
                            stopForBadInput = true;
                    }
                }

            }

            // if an invalid input was flagged to stop processing, set result and exit
            if (stopForBadInput)
            {
                result.setType(Result.Type.FAILED_INPUT);
                return result;
            }

            // add all output keys to the context; if no default is supplied, use an empty string
            foreach (KeyValuePair<String, IOutput> entry in schema.getOutputMap())
                context[entry.Value.getKey()] = (entry.Value.getDefault() != null ? DecisionEngineFuncs.translateValue(entry.Value.getDefault(), context) : "");

            // add the initial context
            if (schema.getInitialContext() != null)
                foreach (IKeyValue keyValue in schema.getInitialContext())
                    context[keyValue.getKey()] = DecisionEngineFuncs.translateValue(keyValue.getValue(), context);

            // process each mapping if it is "involved", which is checked using the current context against inclusion/exclusion criteria
            if (schema.getMappings() != null)
            {
                foreach (IMapping mapping in schema.getMappings())
                {

                    // make sure mapping passes inclusion/exclusion tables if present
                    if (isMappingInvolved(mapping, context))
                    {

                        // if there are any inclusion/exclusion tables, add them to path
                        if (mapping.getInclusionTables() != null)
                            foreach (ITablePath path in mapping.getInclusionTables())
                                result.addPath(mapping.getId(), path.getId());
                        if (mapping.getExclusionTables() != null)
                            foreach (ITablePath path in mapping.getExclusionTables())
                                result.addPath(mapping.getId(), path.getId());

                        // set the mapping-specific initial context if any
                        if (mapping.getInitialContext() != null)
                            foreach (IKeyValue keyValue in mapping.getInitialContext())
                                context[keyValue.getKey()] = keyValue.getValue();

                        // loop over all table paths in the mapping
                        if (mapping.getTablePaths() != null)
                        {
                            foreach (ITablePath path in mapping.getTablePaths())
                            {
                                String tableId = path.getId();

                                // if there is input mapping defined, add the new mapping to the context
                                if (path.getInputMapping() != null)
                                {
                                    foreach (IKeyMapping key in path.getInputMapping())
                                    {
                                        String mapFromKey = key.getFrom();

                                        if (!context.ContainsKey(mapFromKey))
                                        {
                                            result.addError(new Error.ErrorBuilder(Error.Type.UNKNOWN_INPUT_MAPPING).message("Input mapping '" + mapFromKey + "' does not exist for table '" + tableId + "'").key(
                                                    mapFromKey).table(tableId).build());
                                            continue;
                                        }

                                        String sContextValue = "";
                                        if (context.ContainsKey(mapFromKey))
                                            sContextValue = context[mapFromKey];

                                        // DEBUG
                                        //Debug.WriteLine("Table " + tableId + ":  Change " + key.getTo() + " to " + sContextValue);


                                        context[key.getTo()] = sContextValue;


                                    }
                                }

                                // create a stack to keep track of table calls and ensure there is no infinite recursion
                                Stack<String> stack = new Stack<String>(30);

                                // recursively process the mapping; if false is returned, stop all processing
                                bool continueProcessing = process(mapping.getId(), tableId, path, result, stack);

                                // remove the temporary input mappings
                                if (path.getInputMapping() != null)
                                {
                                    foreach (IKeyMapping key in path.getInputMapping())
                                        context.Remove(key.getTo());
                                }

                                if (!continueProcessing)
                                    break;
                            }

                        }
                    }

                }
            }

            // if outputs were specified, remove any extra keys and validate the others if a table was specified
            if (schema.getOutputMap() != null && schema.getOutputMap().Count > 0)
            {

                // Remove all of the undefined keys.
                List<String> lstKeysToRemove = new List<String>();
                foreach (KeyValuePair<String, String> entry in context)
                {
                    if (!schema.getOutputMap().ContainsKey(entry.Key))
                    {
                        lstKeysToRemove.Add(entry.Key);
                    }
                }

                foreach(String sKeyName in lstKeysToRemove)
                {
                    context.Remove(sKeyName);
                }


                Dictionary<String, String>.Enumerator iter = context.GetEnumerator();
                while (iter.MoveNext())
                {
                    KeyValuePair<String, String> entry = iter.Current;
                    IOutput output = null;
                    if (schema.getOutputMap().ContainsKey(entry.Key))
                    {
                        output = schema.getOutputMap()[entry.Key];
                    }

                    // if the key is not defined in the output, remove it
                    if (output == null)
                    {
                        // Do nothing.
                    }
                    else if (output.getTable() != null)
                    {
                        ITable lookup = getProvider().getTable(output.getTable());

                        if (lookup == null)
                        {
                            result.addError(new Error.ErrorBuilder(Error.Type.UNKNOWN_TABLE).message("Output table does not exist: " + output.getTable()).key(output.getKey()).build());
                            continue;
                        }

                        // verify the value of the output key is contained in the associated table
                        IEnumerable<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(lookup, context);
                        if (endpoints == null)
                        {
                            String value = "";
                            if (context.ContainsKey(output.getKey()))
                                value = context[output.getKey()];

                            result.addError(new Error.ErrorBuilder(Error.Type.INVALID_OUTPUT).message("Invalid '" + output.getKey() + "' value (" + (value.Length == 0 ? DecisionEngineFuncs._BLANK_OUTPUT : value) + ")").key(
                                    output.getKey()).table(output.getTable()).build());
                        }
                    }
                }
            }

            return result;
        }


        //========================================================================================================================
        // Internal method to recursively process a table
        // @param mappingId a Mapping identifier
        // @param tableId a Table identifier
        // @param path a TablePath
        // @param result a Result
        // @param stack a stack which tracks the path and makes sure the path doesn't enter an infinite recusive state
        // @return a boolean indicating whether processing should continue
        //========================================================================================================================
        protected bool process(String mappingId, String tableId, ITablePath path, Result result, Stack<String> stack)
        {
            bool continueProcessing = true;

            ITable table = getProvider().getTable(tableId);
            if (table == null)
            {
                result.addError(new Error.ErrorBuilder(Error.Type.UNKNOWN_TABLE).message("The processing of '" + path.getId() + "' contains a reference to an unknown table: '" + tableId + "'").table(tableId)
                        .build());
                return true;
            }

            // track the path history to make sure no table is reached twice
            if (stack.Contains(tableId))
            {
                result.addError(new Error.ErrorBuilder(Error.Type.INFINITE_LOOP).message(
                        "The processing of '" + path.getId() + "' has entered an infinite recursive state.  Table '" + tableId + "' was accessed multiple times.").table(tableId).build());
                return true;
            }

            // keep track of every table that was visited for the entire process
            result.addPath(mappingId, tableId);

            // add the table to the recursion stack
            stack.Push(tableId);

            // look for the match in the mapping table; if no match is found, used the table-specific no_match value
            IEnumerable<IEndpoint> endpoints = DecisionEngineFuncs.matchTable(table, result.getContext());

            if (endpoints == null)
            {
                List<String> colList = new List<String>();
                foreach (IColumnDefinition c in table.getColumnDefinitions())
                {
                    if (c.getType() == ColumnType.ENDPOINT)
                        colList.Add(c.getKey());
                }

                // if a match is not found, include all the endpoints as columns in the error
                result.addError(new Error.ErrorBuilder(Error.Type.MATCH_NOT_FOUND)
                        .message("Match not found in table '" + tableId + "' (" + DecisionEngineFuncs.getTableInputsAsString(table, result.getContext()) + ")")
                        .table(tableId)
                        .columns(colList)
                        .build());
            }
            else
            {
                EndpointType endpType = 0;
                String endpValue = "";
                String endpResultKey = "";
                String sNewValue = "";
                Dictionary<String, String> ResDict = result.getContext();

                foreach (IEndpoint endpoint in endpoints)
                {
                    endpType = endpoint.getType();
                    endpValue = endpoint.getValue();
                    endpResultKey = endpoint.getResultKey();


                    if (EndpointType.STOP == endpType)
                        continueProcessing = false;
                    else if (EndpointType.JUMP == endpType)
                        continueProcessing = process(mappingId, endpValue, path, result, stack);
                    else if (EndpointType.ERROR == endpType)
                    {
                        String message = endpValue;
                        if (message == null || message.Length == 0)
                            message = "Matching resulted in an error in table '" + tableId + "' for column '" + endpoint.getResultKey() + "' (" + DecisionEngineFuncs.getTableInputsAsString(table, result.getContext()) + ")";

                        result.addError(new Error.ErrorBuilder(Error.Type.STAGING_ERROR).message(message).table(tableId).columns(new List<String>{endpoint.getResultKey()}).build());
                    }
                    else if (EndpointType.VALUE == endpType)
                    {
                        // if output mapping(s) were provided, check whether the key was mapped
                        List<String> mappedKeys = new List<String>();
                        if (path.getOutputMapping() != null)
                        {
                            foreach (IKeyMapping key in path.getOutputMapping())
                            {
                                if (key.getFrom() == endpResultKey)
                                    mappedKeys.Add(key.getTo());
                            }
                        }

                        // if the value if null, that is indicating that the key should be removed from the context; otherwise set the value into the context
                        if (mappedKeys.Count == 0)
                        {
                            mappedKeys.Add(endpResultKey);
                        }

                        // iterate over all the mappings for this endpoint key

                        foreach (String key in mappedKeys)
                        {
                            if (endpValue == null)
                                ResDict.Remove(key);
                            else
                            {
                                sNewValue = DecisionEngineFuncs.translateValue(endpValue, ResDict);
                                ResDict[key] = sNewValue;
                            }
                        }
                    }
                }
            }

            // processing of this table is complete and it can be removed from the recursion stack
            stack.Pop();

            return continueProcessing;
        }

    }
}


