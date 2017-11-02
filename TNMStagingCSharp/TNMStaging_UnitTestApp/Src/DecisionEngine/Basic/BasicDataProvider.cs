using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    /**
     * In implementation of DataProvider which holds all data in memory
     */
    public class BasicDataProvider : IDataProvider
    {

        private static String _MATCH_ALL_STRING = "*";
        private static BasicStringRange _MATCH_ALL_ENDPOINT = new BasicStringRange();
        private Dictionary<String, BasicTable> _tables = new Dictionary<String, BasicTable>();
        private Dictionary<String, BasicDefinition> _definitions = new Dictionary<String, BasicDefinition>();

        /**
         * Default constructor
         */
        public BasicDataProvider()
        {
        }

        /**
         * Initialize a definition.
         * @param definition a BasicDefinition
         */
        public void initDefinition(BasicDefinition definition)
        {
            // parse the values into something that can searched more efficiently
            if (definition.getInputs() != null)
            {
                Dictionary<String, IInput> parsedInputMap = new Dictionary<String, IInput>();
                foreach (BasicInput input in definition.getInputs())
                {
                    // verify that all inputs contain a key
                    if (input.getKey() == null)
                        throw new InvalidOperationException("All input defintions must have a 'key' value defined.");

                    parsedInputMap[input.getKey()] = input;

                    // the parsed input map provides an easy way to look up by key
                    definition.setInputMap(parsedInputMap);
                }
            }

            // store the outputs in a Map that can searched more efficiently
            if (definition.getOutputs() != null)
            {
                Dictionary<String, IOutput> parsedOutputMap = new Dictionary<String, IOutput>();

                foreach (BasicOutput output in definition.getOutputs())
                {
                    // verify that all inputs contain a key
                    if (output.getKey() == null)
                        throw new InvalidOperationException("All output definitions must have a 'key' defined.");

                    parsedOutputMap[output.getKey()] = output;
                }

                definition.setOutputMap(parsedOutputMap);
            }
        }

        /**
         * Initialize a table.
         * @param table a BasicTable
         */
        public void initTable(BasicTable table)
        {
            HashSet<String> extraInputs = new HashSet<String>();

            if (table.getRawRows() != null)
            {
                foreach (List<String> row in table.getRawRows())
                {
                    BasicTableRow tableRowEntity = new BasicTableRow();

                    // make sure the number of cells in the row matches the number of columns defined
                    if (table.getColumnDefinitions().Count != row.Count)
                        throw new InvalidOperationException("Table '" + table.getId() + "' has a row with " + row.Count + " values but should have " + table.getColumnDefinitions().Count + ": " + row);

                    // loop over the column definitions in order since the data needs to retrieved by array position
                    for (int i = 0; i < table.getColumnDefinitions().Count; i++)
                    {
                        IColumnDefinition col = table.getColumnDefinitions()[i];
                        String cellValue = row[i];

                        switch (col.getType())
                        {
                            case ColumnType.INPUT:
                                // if there are no ranges in the list, that means the cell was "blank" and is not needed in the table row
                                List<BasicStringRange> ranges = splitValues(cellValue);
                                if (!(ranges.Count == 0))
                                {
                                    tableRowEntity.addInput(col.getKey(), ranges);

                                    // if there are key references used (values that reference other inputs) like {{key}}, then add them to the extra inputs list
                                    foreach (BasicStringRange range in ranges)
                                    {
                                        if (DecisionEngineFuncs.isReferenceVariable(range.getLow()))
                                            extraInputs.Add(DecisionEngineFuncs.trimBraces(range.getLow()));
                                        if (DecisionEngineFuncs.isReferenceVariable(range.getHigh()))
                                            extraInputs.Add(DecisionEngineFuncs.trimBraces(range.getHigh()));
                                    }
                                }
                                break;
                            case ColumnType.ENDPOINT:
                                BasicEndpoint endpoint = parseEndpoint(cellValue);
                                if (EndpointType.VALUE == endpoint.getType())
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
                                throw new InvalidOperationException("Table '" + table.getId() + " has an unknown column type: '" + col.getType() + "'");
                        }
                    }

                    table.getTableRows().Add(tableRowEntity);
                }
            }

            // add extra inputs, if any; do not include context variables since they are not user input
            foreach (String s in TNMStagingCSharp.Src.Staging.Staging.CONTEXT_KEYS)
            {
                extraInputs.Remove(s);
            }

            table.GenerateInputColumnDefinitions();

            table.setExtraInput(extraInputs.Count == 0 ? null : extraInputs);
        }

        /**
         * Parse the string representation of an endpoint into a Endpoint object.  There are two supported formats:
         * <p>
         * ENDPOINT_TYPE
         * ENDPOINT_TYPE:PARAMETER
         * </p>
         * @param endpoint an endpoint
         * @return an BasicEndpoint object
         */
        public BasicEndpoint parseEndpoint(String endpoint)
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

            return new BasicEndpoint(type, value);
        }

        /**
         * Parses a string of values into a List of StringRange entities.  The values can contain ranges and multiple values.  Some examples might be:
         * <p>
         * 10
         * 10-14
         * 10,15,20
         * 11,13-15,25-29,35
         * </p>
         * Note that all values (both low and high) must be the same length since they are evaluated using String comparison.
         * @param values a string of values
         * @return a List of BasicStringRange objects
         */
        protected List<BasicStringRange> splitValues(String values)
        {
            List<BasicStringRange> convertedRanges = new List<BasicStringRange>();

            if (values != null)
            {
                // if the value of the string is "*", then consider it as matching anything
                if (values == _MATCH_ALL_STRING)
                    convertedRanges.Add(_MATCH_ALL_ENDPOINT);
                else
                {
                    String[] ranges = values.Split(",".ToCharArray());

                    foreach (String range in ranges)
                    {
                        // Not sure if this is the best way to handle this, but I ran into a problem when I converted the CS data.  One of the tables had
                        // a value of "N0(mol-)".  This fails since it considers it a range and we require our ranges to have the same size on both sides.
                        // The only thing I can think of is for cases like this, assume it is not a range and use the whole string as a non-range value.
                        // The problem is that if something is entered incorrectly and was supposed to be a range, we will not process it correctly.  We
                        // may need to revisit this issue later.
                        String[] parts = range.Split("-".ToCharArray());
                        if (parts.Length == 1)
                            convertedRanges.Add(new BasicStringRange(parts[0].Trim(), parts[0].Trim()));
                        else if (parts.Length == 2)
                        {
                            if (parts[0].Trim().Length != parts[1].Trim().Length)
                                convertedRanges.Add(new BasicStringRange(range.Trim(), range.Trim()));
                            else
                                convertedRanges.Add(new BasicStringRange(parts[0].Trim(), parts[1].Trim()));
                        }
                        else
                            convertedRanges.Add(new BasicStringRange(range.Trim(), range.Trim()));
                    }
                }
            }

            return convertedRanges;
        }

        public ITable getTable(String id)
        {
            ITable oRetval = null;
            if (_tables.ContainsKey(id))
                oRetval = _tables[id];

            return oRetval;
        }

        public IDefinition getDefinition(String id)
        {
            IDefinition oRetval = null;
            if (_definitions.ContainsKey(id))
                oRetval = _definitions[id];

            return oRetval;
        }

        /**
         * Add a table to the list
         * @param table a BasicTable
         */
        public void addTable(BasicTable table)
        {
            if (_tables.ContainsKey(table.getId()))
                throw new InvalidOperationException("ERROR: A table with identifier '" + table.getId() + "' already exists");

            initTable(table);

            _tables[table.getId()] = table;
        }

        /**
         * Add a starting point to the list
         * @param definition a BasicDefinition
         */
        public void addDefinition(BasicDefinition definition)
        {
            if (_definitions.ContainsKey(definition.getId()))
                throw new InvalidOperationException("ERROR: An definition with identifier '" + definition.getId() + "' already exists");

            initDefinition(definition);

            _definitions[definition.getId()] = definition;
        }

    }
}
