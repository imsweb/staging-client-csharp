using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicTable : ITable
    {

        private String _id;
        private List<IColumnDefinition> _definitions;
        private HashSet<String> _extraInput;
        private List<List<String>> _rows = new List<List<String>>();

        // parsed fields
        private List<ITableRow> _parsedTableRows = new List<ITableRow>();

        private List<IColumnDefinition> _inputdefinitions;

        /**
         * Default constructor
         */
        public BasicTable()
        {
        }

        /**
         * Construct with a table name
         * @param name a table name
         */
        public BasicTable(String name)
        {
            setId(name);
        }

        public String getId()
        {
            return _id;
        }

        public void setId(String id)
        {
            _id = id;
        }

        public List<IColumnDefinition> getColumnDefinitions()
        {
            return _definitions;
        }

        public HashSet<String> getExtraInput()
        {
            return _extraInput;
        }

        public void setExtraInput(HashSet<String> extraInput)
        {
            _extraInput = extraInput;
        }

        public void setColumnDefinitions(List<IColumnDefinition> definitions)
        {
            _definitions = definitions;
        }

        public void addColumnDefinition(String key, ColumnType type)
        {
            if (_definitions == null)
                _definitions = new List<IColumnDefinition>();

            _definitions.Add(new BasicColumnDefinition(key, type));
        }


        public List<IColumnDefinition> getInputColumnDefinitions()
        {
            return _inputdefinitions;
        }

        public void GenerateInputColumnDefinitions()
        {
            _inputdefinitions = new List<IColumnDefinition>();
            IColumnDefinition col = null;
            for (int i = 0; i < _definitions.Count; i++)
            {
                col = _definitions[i];
                if (col != null)
                    if (ColumnType.INPUT == col.getType())
                    {
                        _inputdefinitions.Add(col);
                    }
            }
        }



        public List<List<String>> getRawRows()
        {
            return _rows;
        }

        public void setRawRows(List<List<String>> rows)
        {
            _rows = rows;
        }

        public void addRawRow(List<String> row)
        {
            if (_rows == null)
                _rows = new List<List<String>>();

            _rows.Add(row);
        }

        public List<ITableRow> getTableRows()
        {
            return _parsedTableRows;
        }

        public void setTableRows(List<ITableRow> parsedTableRows)
        {
            _parsedTableRows = parsedTableRows;
        }

    }
}
