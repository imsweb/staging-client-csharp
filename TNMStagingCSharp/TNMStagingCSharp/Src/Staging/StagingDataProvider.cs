// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.Entities.Impl;
using TNMStagingCSharp.Src.Staging.Engine;


namespace TNMStagingCSharp.Src.Staging
{
    // An abstract implementation of DataProvider customized for handling staging schemas/tables
    public abstract class StagingDataProvider : IDataProvider
    {
        // tables that all algorithm versions must have
        public static readonly String PRIMARY_SITE_TABLE = "primary_site";
        public static readonly String HISTOLOGY_TABLE = "histology";

        //private readonly static Entities.StagingRange _MATCH_ALL_ENDPOINT = new Entities.StagingRange();
        private static Range _matchAllEndpoint = null;

        private ConcurrentDictionary<String, List<Schema>> mLookupMemoryDict;
        private int miLookupMemoryDictCount;

        private ConcurrentDictionary<String, HashSet<String>> mValuesMemoryDict;

        private const int NUM_ITEMS_IN_CACHE_CAUSES_TRIM = 10000;

        protected HashSet<String> _trie;

        // Constructor loads all schemas and sets up cache
        public StagingDataProvider()
        {
            _matchAllEndpoint = getMatchAllRange();

            // cache schema lookups
            CreateLookupMemoryCache();

            // cache the valid values for certain tables including site and histology
            CreateValuesMemoryCache();
        }

        // Clear the caches
        public void invalidateCache()
        {
            mLookupMemoryDict.Clear();
            miLookupMemoryDictCount = 0;

            mValuesMemoryDict.Clear();
        }

        private void CreateLookupMemoryCache()
        {
            int concurrencyLevel = Math.Min(9, Environment.ProcessorCount + 1);

            mLookupMemoryDict = new ConcurrentDictionary<String, List<Schema>> (concurrencyLevel, NUM_ITEMS_IN_CACHE_CAUSES_TRIM);
            miLookupMemoryDictCount = 0;

        }

        private void CreateValuesMemoryCache()
        {
            int concurrencyLevel = Math.Min(9, Environment.ProcessorCount + 1);

            mValuesMemoryDict = new ConcurrentDictionary<String, HashSet<String>>(concurrencyLevel, NUM_ITEMS_IN_CACHE_CAUSES_TRIM);
        }


        // Initialize a schema.
        // @param schema schema entity
        // @return initialized schema entity
        public Schema initSchema(Schema schema)
        {
            // parse the schema selection ranges
            if (schema.getSchemaSelectionTable() == null)
                throw new System.InvalidOperationException("Schemas must have a schema selection table.");

            // store the inputs in a Map that can searched more efficiently
            if (schema.getInputs() != null)
            {
                Dictionary<String, IInput> parsedInputMap = new Dictionary<String, IInput>();

                foreach (IInput input in schema.getInputs())
                {
                    // verify that all inputs contain a key
                    if (input.getKey() == null)
                        throw new System.InvalidOperationException("All input definitions must have a 'key' defined.");

                    parsedInputMap[input.getKey()] = input;
                }

                schema.setInputMap(parsedInputMap);
            }

            // store the outputs in a Map that can searched more efficiently
            if (schema.getOutputs() != null)
            {
                Dictionary<String, IOutput> parsedOutputMap = new Dictionary<String, IOutput>();

                foreach (IOutput output in schema.getOutputs())
                {
                    // verify that all inputs contain a key
                    if (output.getKey() == null)
                        throw new System.InvalidOperationException("All output definitions must have a 'key' defined.");

                    parsedOutputMap[output.getKey()] = output;
                }

                schema.setOutputMap(parsedOutputMap);
            }

            // make sure that the mapping initial context does not set a value for an input field
            if (schema.getMappings() != null)
                foreach (IMapping mapping in schema.getMappings())
                    if (mapping.getInitialContext() != null)
                        foreach (IKeyValue kv in mapping.getInitialContext())
                            if (schema.getInputMap().ContainsKey(kv.getKey()))
                                throw new System.InvalidOperationException("The key '" + kv.getKey() + "' is defined in an initial context, but that is not allowed since it is also defined as an input.");


            return schema;
        }

        // Initialize a table.
        // @param table table entity
        // @return initialized table entity
        public ITable initTable(ITable table)
        {
            HashSet<String> extraInputs = new HashSet<String>();

            List<List<String>> pTableRawRows = table.getRawRows();

            // empty out the parsed rows
            table.clearTableRows();

            if (pTableRawRows != null)
            {
                foreach (List<String> row in pTableRawRows)
                {
                    ITableRow tableRowEntity = getTableRow();

                    // make sure the number of cells in the row matches the number of columns defined
                    if (table.getColumnDefinitions().Count != row.Count)
                        throw new System.InvalidOperationException("Table '" + table.getId() + "' has a row with " + row.Count + " values but should have " + table.getColumnDefinitions().Count + ": " + row);

                    // loop over the column definitions in order since the data needs to be retrieved by array position
                    for (int i = 0; i < table.getColumnDefinitions().Count; i++)
                    {
                        IColumnDefinition col = table.getColumnDefinitions()[i];
                        String cellValue = row[i];

                        switch (col.getType())
                        {
                            case ColumnType.INPUT:
                                // if there are no ranges in the list, that means the cell was "blank" and is not needed in the table row
                                List<Range> ranges = splitValues(cellValue);
                                if (!(ranges.Count == 0))
                                {
                                    tableRowEntity.addInput(col.getKey(), ranges);

                                    // if there are key references used (values that reference other inputs) like {{key}}, then add them to the extra inputs list
                                    foreach (Range range in ranges)
                                    {
                                        if (DecisionEngineFuncs.isReferenceVariable(range.getLow()))
                                            extraInputs.Add(DecisionEngineFuncs.trimBraces(range.getLow()));
                                        if (DecisionEngineFuncs.isReferenceVariable(range.getHigh()))
                                            extraInputs.Add(DecisionEngineFuncs.trimBraces(range.getHigh()));
                                    }
                                }
                                break;
                            case ColumnType.ENDPOINT:
                                IEndpoint endpoint = parseEndpoint(cellValue);
                                endpoint.setResultKey(col.getKey());
                                tableRowEntity.addEndpoint(endpoint);

                                // if there are key references used (values that reference other inputs) like {{key}}, then add them to the extra inputs list
                                if (EndpointType.VALUE == endpoint.getType() && DecisionEngineFuncs.isReferenceVariable(endpoint.getValue()))
                                    extraInputs.Add(DecisionEngineFuncs.trimBraces(endpoint.getValue()));
                                break;
                            case ColumnType.DESCRIPTION:
                                // do nothing
                                break;
                            default:
                                throw new System.InvalidOperationException("Table '" + table.getId() + " has an unknown column type: '" + col.getType() + "'");
                        }
                    }

                    tableRowEntity.ConvertColumnInput();

                    table.addTableRow(tableRowEntity);
                }
            }

            // add extra inputs, if any; do not include context variables since they are not user input
            foreach (String s in Staging.CONTEXT_KEYS)
            {
                extraInputs.Remove(s);
            }
            table.setExtraInput(extraInputs.Count == 0 ? null : extraInputs);

            table.GenerateInputColumnDefinitions();

            return table;
        }


        // Return true if the site is valid
        // @param site primary site
        // @return true if the side is valid
        public bool isValidSite(String site)
        {
            bool valid = (site != null);
            if (valid)
            {
                ITable table = getTable(PRIMARY_SITE_TABLE);
                if (table == null)
                    throw new System.InvalidOperationException("Unable to locate " + PRIMARY_SITE_TABLE + " table");
                
                valid = getValidSites().Contains(site);
             }
             return valid;
        }

        // Return true if the histology is valid
        // @param histology histology
        // @return true if the histology is valid
        public bool isValidHistology(String histology)
        {
            bool valid = (histology != null);
            if (valid)
            {
                ITable table = getTable(HISTOLOGY_TABLE);
                if (table == null)
                    throw new System.InvalidOperationException("Unable to locate " + HISTOLOGY_TABLE + " table");

                valid = getValidHistologies().Contains(histology);
            }
            return valid;
        }

        // Parse the string representation of an endpoint into a Endpoint object
        // @param endpoint endpoint String
        // @return an Endpoint object
        public IEndpoint parseEndpoint(String endpoint)
        {
            String[] parts = endpoint.Split(":".ToCharArray(), 2);
            bool bTypeEmpty = true;
            EndpointType type = EndpointType.ERROR;

            try
            {
                type = (EndpointType)Enum.Parse(typeof(EndpointType), parts[0].Trim());
                bTypeEmpty = false;
            }
            catch 
            {
                // catch exception; it will be re-thrown below
                bTypeEmpty = true;
            }

            // make sure type was valid
            if (bTypeEmpty)
                throw new System.InvalidOperationException("Invalid endpoint type: '" + endpoint + "'.  Must be either JUMP, VALUE, MATCH, STOP, or ERROR");

            String value = parts.Length == 2 ? parts[1].Trim() : null;

            // some endpoint types do not require a value but these do
            if ((value == null || value.Length == 0) && EndpointType.JUMP == type)
                throw new System.InvalidOperationException("JUMP endpoint types must have a value: '" + endpoint + "'");

            return getEndpoint(type, value);
        }

        // Parses a string in having lists of ranges into a List of Range objects
        // @param values String representing sets value ranges
        // @return a parsed list of string Range objects
        public List<Range> splitValues(String values)
        {
            List<Range> convertedRanges = new List<Range>();

            if (values != null)
            {
                // if the value of the string is "*", then consider it as matching anything
                if (values == "*")
                    convertedRanges.Add(_matchAllEndpoint);
                else
                {
                    // split the string; the "9999" makes sure to not discard empty strings
                    String[] ranges = values.Split(",".ToCharArray(), 9999);

                    foreach (String range in ranges)
                    {
                        // Not sure if this is the best way to handle this, but I ran into a problem when I converted the CS data.  One of the tables had
                        // a value of "N0(mol-)".  This fails since it considers it a range and we require our ranges to have the same size on both sides.
                        // The only thing I can think of is for cases like this, assume it is not a range and use the whole string as a non-range value.
                        // The problem is that if something is entered incorrectly and was supposed to be a range, we will not process it correctly.  We
                        // may need to revisit this issue later.
                        String[] parts = range.Split("-".ToCharArray());
                        if (parts.Length == 2)
                        {
                            String low = parts[0].Trim();
                            String high = parts[1].Trim();

                            // check if both sides of the range are numeric values; if so the length does not have to match
                            bool isNumericRange = isNumeric(low) && isNumeric(high);

                            // if same length, a numeric range, or one of the parts is a context variable, use the low and high as range.  Otherwise consier
                            // a single value (i.e. low = high)
                            if (low.Length == high.Length || isNumericRange || DecisionEngineFuncs.isReferenceVariable(low) || DecisionEngineFuncs.isReferenceVariable(high))
                                convertedRanges.Add(getRange(low, high));
                            else
                                convertedRanges.Add(getRange(range.Trim(), range.Trim()));


                        }
                        else
                            convertedRanges.Add(getRange(range.Trim(), range.Trim()));
                    }
                }
            }

            return convertedRanges;
        }

        public static bool isNumeric(String value)
        {
            int resultInt = 0;
            bool retval = int.TryParse(value, out resultInt);
            if (!retval)
            {
                float resultFloat = 0;
                retval = float.TryParse(value, out resultFloat);
            }
            return retval;
        }

        // Return the algorithm associated with the provider
        // @return algorithm id
        public abstract String getAlgorithm();

        // Return the version associated with the provider
        // @return version number
        public abstract String getVersion();

        // Return a new table
        // @param id the table id
        // @return Table entity
        public abstract ITable getTable(String id);

        // Return a new schema
        // @param id the schema id
        // @return Schema entity
        public abstract Schema getSchema(String id);

        // Return a new endpoint
        // @param type type of endpoint
        // @param value value of endpoint
        // @return Endpoint entity
        public abstract IEndpoint getEndpoint(EndpointType type, String value);

        // Return a new table row
        // @return TableRow entity
        public abstract ITableRow getTableRow();

        // Return a range representing "match all"
        // @return a Range entity
        public abstract Range getMatchAllRange();

        // Return a newly created range
        // @param low low value
        // @param high high value
        // @return Range entity
        public abstract Range getRange(String low, String high);

        // Return a set of all schema identifiers as 
        // @return a Set of schema identifiers
        public abstract HashSet<String> getSchemaIds();

        // Return a set of all the table names
        // @return a List of table identifier
        public abstract HashSet<String> getTableIds();

        // Return a set of supported glossary terms
        // @return a Set of terms
        public abstract HashSet<String> getGlossaryTerms();

        // Return a defitition of a glossary term
        // @param term glossary term
        // @return a glossary definiiion
        public abstract GlossaryDefinition getGlossaryDefinition(String term);

        // Return a list of all glossary matches in the supplied text
        // @param text text to match against
        // @return a List of glossary hits
        public virtual List<GlossaryHit> getGlossaryMatches(String text)
        {
            //return _trie.parseText(text).stream().map(hit-> new GlossaryHit(hit.getKeyword(), hit.getStart(), hit.getEnd())).collect(Collectors.toSet());
            List<GlossaryHit> retval = new List<GlossaryHit>();

            //onlyWholeWords().ignoreCase();
            int index = -1;
            foreach (String term in _trie)
            {
                index = 0;
                while (index >= 0)
                {
                    index = text.IndexOf(term, index, StringComparison.InvariantCultureIgnoreCase);
                    if (index >= 0)
                    {
                        char before = index - 1 >= 0 ? text[index - 1] : ' ';
                        char after = index + term.Length < text.Length ? text[index + term.Length] : ' ';
                        if (!char.IsLetter(before) && !char.IsLetter(after))
                        {
                            GlossaryHit newHit = new GlossaryHit(term.ToLower(), index, index + term.Length - 1);
                            retval.Add(newHit);
                        }
                        index += term.Length;
                    }
                }
            }
            return retval;
        }

        // Return all the legal site values
        // @return a set of valid sites
        public HashSet<String> getValidSites()
        {
            HashSet<String> lstRetval = null;

            // Dictionary for cache.
            if (!mValuesMemoryDict.TryGetValue(PRIMARY_SITE_TABLE, out lstRetval))
            {
                if (mValuesMemoryDict.Count > NUM_ITEMS_IN_CACHE_CAUSES_TRIM)
                {
                    mValuesMemoryDict.Clear();
                }
                lstRetval = getAllInputValues(PRIMARY_SITE_TABLE);
                mValuesMemoryDict.TryAdd(PRIMARY_SITE_TABLE, lstRetval);
            }

            return lstRetval;
        }

        // Return all the legal histology values
        // @return a set of valid histologies
        public HashSet<String> getValidHistologies()
        {
            HashSet<String> lstRetval = null;

            // Dictionary for cache.
            if (!mValuesMemoryDict.TryGetValue(HISTOLOGY_TABLE, out lstRetval))
            {
                if (mValuesMemoryDict.Count > NUM_ITEMS_IN_CACHE_CAUSES_TRIM)
                {
                    mValuesMemoryDict.Clear();
                }
                lstRetval = getAllInputValues(HISTOLOGY_TABLE);
                mValuesMemoryDict.TryAdd(HISTOLOGY_TABLE, lstRetval);
            }


            return lstRetval;
        }


        // Look up a schema based on site, histology and an optional discriminator.
        // @param lookup schema lookup input
        // @return a list of StagingSchemaInfo objects
        public List<Schema> lookupSchema(SchemaLookup lookup)
        {
            List<Schema> lstRetval = null;

            // If doing a more broad lookup without giving both site and histology, do not use the cache.  I don't want to cache
            // since the results could include all the data
            if (lookup.getSite() == null || lookup.getHistology() == null)
                return getSchemas(lookup);


            // Dictionary for cache.
            String sIDString = lookup.GetHashString();


            if (!mLookupMemoryDict.TryGetValue(sIDString, out lstRetval))
            {
                if (miLookupMemoryDictCount > NUM_ITEMS_IN_CACHE_CAUSES_TRIM)
                {
                    mLookupMemoryDict.Clear();
                    Interlocked.Exchange(ref miLookupMemoryDictCount, 0);
                }

                lstRetval = getSchemas(lookup);

                if (mLookupMemoryDict.TryAdd(sIDString, lstRetval))
                {
                    Interlocked.Increment(ref miLookupMemoryDictCount);
                }

            }

            return lstRetval;
        }


        // Look up a schema based on site, histology and an optional discriminator.
        // @param lookup schema lookup input
        // @return a list of StagingSchemaInfo objects
        private List<Schema> getSchemas(SchemaLookup lookup)
        {
            List<Schema> matchedSchemas = new List<Schema>(5);

            String site = lookup.getInput(StagingData.PRIMARY_SITE_KEY);
            String histology = lookup.getInput(StagingData.HISTOLOGY_KEY);
            bool hasDiscriminator = lookup.hasDiscriminator();

            // site or histology must be supplied and they must be valid; I am assuming that all algorithms must have tables that validate
            // both site and histology
            if ((site != null && !isValidSite(site)) || (histology != null && !isValidHistology(histology)))
                return matchedSchemas;

            // searching on a discriminator is only supported if also searching on site and histology; if ssf25 supplied without either
            // of those fields, return no results
            if (hasDiscriminator && (site == null || (site.Length == 0) || histology == null || (histology.Length == 0)))
                return matchedSchemas;

            // site or histology must be supplied
            if (site != null || histology != null)
            {
                HashSet<String> lstSchemaIds = getSchemaIds();

                // loop over selection table and match using only the supplied keys
                foreach (String schemaId in lstSchemaIds)
                {
                    StagingSchema schema = (StagingSchema)(getSchema(schemaId));

                    if (schema.getSchemaSelectionTable() != null)
                    {
                        ITable table = getTable(schema.getSchemaSelectionTable());
                        if (table != null && DecisionEngineFuncs.matchTable(table, lookup.getInputs(), lookup.getKeys()) != null)
                            matchedSchemas.Add(schema);
                    }
                }
            }

            return matchedSchemas;
        }

        // Given a table, return a Set of all the distinct input values.  This is for tables that have a single INPUT column.
        // @param tableId table identifier
        // @return A set of unique inputs
        private HashSet<String> getAllInputValues(String tableId)
        {
            HashSet<String> values = new HashSet<String>();

            // if the table is not found, return right away with an empty list
            //StagingTable table = (getTable(tableId) as StagingTable);
            ITable table = getTable(tableId);
            if (table == null)
                return values;

            // find the input key
            HashSet<String> inputKeys = new HashSet<String>();
            foreach (IColumnDefinition col in table.getColumnDefinitions())
            {
                if (col.getType() == ColumnType.INPUT)
                {
                    inputKeys.Add(col.getKey());
                }
            }

            if (inputKeys.Count != 1)
                throw new System.InvalidOperationException("Table '" + table.getId() + "' must have one and only one INPUT column.");

            HashSet<String>.Enumerator enumInput = inputKeys.GetEnumerator();
            enumInput.MoveNext();
            String inputKey = enumInput.Current;

            foreach (ITableRow row in table.getTableRows())
            {
                foreach (Range range in row.getColumnInput(inputKey))
                {
                    if (range.getLow() != null)
                    {
                        if (range.getLow() == range.getHigh())
                            values.Add(range.getLow());
                        else
                        {
                            int low = Convert.ToInt32(range.getLow());
                            int high = Convert.ToInt32(range.getHigh());

                            // add all values in range
                            for (int i = low; i <= high; i++)
                                values.Add(padStart(i.ToString(), range.getLow().Length, '0'));
                        }
                    }
                }

            }

            return values;
        }

        // Returns a string, of length at least {@code minLength}, consisting of {@code string} prepended
        // with as many copies of {@code padChar} as are necessary to reach that length.
        public static String padStart(String sValue, int minLength, char padChar)
        {
            if (sValue == null || sValue.Length >= minLength)
                return sValue;

            StringBuilder sb = new StringBuilder(minLength);
            for (int i = sValue.Length; i < minLength; i++)
                sb.Append(padChar);
            sb.Append(sValue);

            return sb.ToString();
        }
    }
}


