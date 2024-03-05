// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.Tools;


namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    public class StagingTable : ITable
    {
        [JsonProperty("id", Order = 1)]
        private String _displayId;
        [JsonProperty("algorithm", Order = 2)]
        private String _algorithm;
        [JsonProperty("version", Order = 3)]
        private String _version;
        [JsonProperty("name", Order = 4)]
        private String _name;
        [JsonProperty("title", Order = 5)]
        private String _title;
        [JsonProperty("description", Order = 7)]
        private String _description;
        [JsonProperty("subtitle", Order = 6)]
        private String _subtitle;
        [JsonProperty("notes", Order = 8)]
        private String _notes;
        [JsonProperty("rational", Order = 9)]
        private String _rationale;
        [JsonProperty("additional_info", Order = 10)]
        private String _additionalInfo;
        [JsonProperty("coding_guildlines", Order = 11)]
        private String _codingGuidelines;
        [JsonProperty("footnotes", Order = 12)]
        private String _footnotes;
        [JsonProperty("last_modified", Order = 13)]
        private DateTime _lastModified;
        [JsonProperty("definition", Order = 14)]
        [JsonConverter(typeof(CustomListConverter_StagingColumnDefinition_IColumnDefinition))]
        private List<IColumnDefinition> _definition;
        [JsonProperty("extra_input", Order = 15)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private HashSet<String> _extraInput;
        [JsonProperty("rows", Order = 16)]
        [JsonConverter(typeof(CustomListConverter<List<String>>))]
        private List<List<String>> _rows = new List<List<String>>();


        int miHashCode;

        // parsed fields
        private List<ITableRow> _parsedTableRows = new List<ITableRow>();

        private List<IColumnDefinition> _inputdefinitions;


        public StagingTable()
        {
        }

        public StagingTable(String id)
        {
            setId(id);
        }

        public String getId()
        {
            return _displayId;
        }

        public void setId(String id)
        {
            _displayId = id;
            ComputeHashCode();
        }

        public String getAlgorithm()
        {
            return _algorithm;
        }

        public void setAlgorithm(String algorithm)
        {
            _algorithm = algorithm;
            ComputeHashCode();
        }

        public String getVersion()
        {
            return _version;
        }

        public void setVersion(String version)
        {
            _version = version;
            ComputeHashCode();
        }

        public String getName()
        {
            return _name;
        }

        public void setName(String name)
        {
            _name = name;
            ComputeHashCode();
        }

        public String getTitle()
        {
            return _title;
        }

        public void setTitle(String title)
        {
            _title = title;
            ComputeHashCode();
        }

        public String getDescription()
        {
            return _description;
        }

        public void setDescription(String description)
        {
            _description = description;
            ComputeHashCode();
        }

        public String getSubtitle()
        {
            return _subtitle;
        }

        public void setSubtitle(String subtitle)
        {
            _subtitle = subtitle;
            ComputeHashCode();
        }

        public String getNotes()
        {
            return _notes;
        }

        public void setNotes(String notes)
        {
            _notes = notes;
            ComputeHashCode();
        }

        public String getRationale()
        {
            return _rationale;
        }

        public void setRationale(String rationale)
        {
            _rationale = rationale;
            ComputeHashCode();
        }

        public String getAdditionalInfo()
        {
            return _additionalInfo;
        }

        public void setAdditionalInfo(String additionalInformation)
        {
            _additionalInfo = additionalInformation;
            ComputeHashCode();
        }

        public String getCodingGuidelines()
        {
            return _codingGuidelines;
        }

        public void setCodingGuidelines(String codingGuidelines)
        {
            _codingGuidelines = codingGuidelines;
            ComputeHashCode();
        }

        public String getFootnotes()
        {
            return _footnotes;
        }

        public void setFootnotes(String footnotes)
        {
            _footnotes = footnotes;
            ComputeHashCode();
        }

        public DateTime getLastModified()
        {
            return _lastModified;
        }

        public void setLastModified(DateTime lastModified)
        {
            _lastModified = lastModified;
            ComputeHashCode();
        }

        public List<IColumnDefinition> getColumnDefinitions()
        {
            return _definition;
        }

        public List<IColumnDefinition> getInputColumnDefinitions()
        {
            return _inputdefinitions;
        }

        public void GenerateInputColumnDefinitions()
        {
            _inputdefinitions = new List<IColumnDefinition>();
            IColumnDefinition col = null;
            for (int i = 0; i < _definition.Count; i++)
            {
                col = _definition[i];
                if (col != null)
                    if (ColumnType.INPUT == col.getType())
                    {
                        _inputdefinitions.Add(col);
                    }
            }
        }



        public void setColumnDefinitions(List<IColumnDefinition> definition)
        {
            _definition = definition;
            ComputeHashCode();
        }

        public void addColumnDefinition(String key, ColumnType type)
        {
            if (_definition == null)
                _definition = new List<IColumnDefinition>();

            StagingColumnDefinition def = new StagingColumnDefinition();
            def.setKey(key);
            def.setType(type);

            _definition.Add(def);
        }


        public HashSet<String> getExtraInput()
        {
            return _extraInput;
        }

        public void setExtraInput(HashSet<String> extraInput)
        {
            _extraInput = extraInput;
            ComputeHashCode();
        }

        public List<List<String>> getRawRows()
        {
            return _rows;
        }

        public void setRawRows(List<List<String>> rows)
        {
            _rows = rows;
            ComputeHashCode();
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
            ComputeHashCode();
        }

        public void addTableRow(ITableRow row)
        {
            getTableRows().Add((StagingTableRow)row);
        }

        public void clearTableRows()
        {
            _parsedTableRows.Clear();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingTable table = (StagingTable)o;

            // intentionally does not include _id, _lastModified, _parsedTableRows
            return Equals(_displayId, table._displayId) &&
                   Equals(_algorithm, table._algorithm) &&
                   Equals(_version, table._version) &&
                   Equals(_name, table._name) &&
                   Equals(_title, table._title) &&
                   Equals(_description, table._description) &&
                   Equals(_subtitle, table._subtitle) &&
                   Equals(_notes, table._notes) &&
                   Equals(_rationale, table._rationale) &&
                   Equals(_additionalInfo, table._additionalInfo) &&
                   Equals(_codingGuidelines, table._codingGuidelines) &&
                   Equals(_footnotes, table._footnotes) &&
                   Equals(_definition, table._definition) &&
                   Equals(_extraInput, table._extraInput) &&
                   Equals(_rows, table._rows);
        }

        public override int GetHashCode()
        {
            // intentionally does not include _id, _lastModified, _parsedTableRows
            return miHashCode;
        }

        public void ComputeHashCode()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_displayId);
            MyStringBuilder.Append(_algorithm);
            MyStringBuilder.Append(_version);
            MyStringBuilder.Append(_name);
            MyStringBuilder.Append(_title);
            MyStringBuilder.Append(_description);
            MyStringBuilder.Append(_subtitle);
            MyStringBuilder.Append(_notes);
            MyStringBuilder.Append(_rationale);
            MyStringBuilder.Append(_additionalInfo);
            MyStringBuilder.Append(_codingGuidelines);
            MyStringBuilder.Append(_footnotes);
            MyStringBuilder.Append(_lastModified.ToString());
            if (_definition != null)
            {
                for (int i = 0; i < _definition.Count; i++)
                {
                    MyStringBuilder.Append(_definition[i].getKey());
                    MyStringBuilder.Append(_definition[i].getType());
                }
            }
            if (_extraInput != null)
            {
                foreach (String s in _extraInput)
                {
                    MyStringBuilder.Append(s);
                }
            }
            if (_rows != null)
            {
                for (int i = 0; i < _rows.Count; i++)
                {
                    for (int j = 0; j < _rows[i].Count; j++)
                    {
                        MyStringBuilder.Append(_rows[i][j]);
                    }
                }
            }
            miHashCode = MyStringBuilder.ToString().GetHashCode();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }



        public String GetDebugString(String indent)
        {
            String sRetval = "";

            sRetval += indent + "Display ID:            " + _displayId + Strings.EOL;
            sRetval += indent + "Algorithm:             " + _algorithm + Strings.EOL;
            sRetval += indent + "Version:               " + _version + Strings.EOL;
            sRetval += indent + "Name:                  " + _name + Strings.EOL;
            sRetval += indent + "Title:                 " + _title + Strings.EOL;
            sRetval += indent + "Description:           " + _description + Strings.EOL;
            sRetval += indent + "Subtitle:              " + _subtitle + Strings.EOL;
            //sRetval += indent + "Notes:                 " + _notes + Strings.EOL;
            sRetval += indent + "Rationale:             " + _rationale + Strings.EOL;
            sRetval += indent + "AdditionalInfo:        " + _additionalInfo + Strings.EOL;
            sRetval += indent + "CodingGuidelines:      " + _codingGuidelines + Strings.EOL;
            sRetval += indent + "Footnotes:             " + _footnotes + Strings.EOL;
            sRetval += indent + "LastModified:          " + _lastModified + Strings.EOL;
            sRetval += indent + "Definition:            " + Strings.EOL;
            String sDefStr = "NULL";
            if (_definition != null)
            {
                sDefStr = "";
                foreach (StagingColumnDefinition id in _definition)
                {
                    sDefStr += id.GetDebugString("  ") + Strings.EOL;
                }
            }
            sRetval += indent + sDefStr + Strings.EOL;

            sRetval += indent + "ExtraInput:            " + Strings.EOL;
            String sExtraStr = "NULL";
            if (_extraInput != null)
            {
                sExtraStr = "";
                foreach (String s in _extraInput)
                {
                    sExtraStr += s + ", ";
                }
            }
            sRetval += indent + sExtraStr + Strings.EOL;
            sRetval += indent + "Rows:                  " + Strings.EOL;
            String sRowsStr = "NULL";
            if (_rows != null)
            {
                sRowsStr = "";
                foreach (List<String> ls in _rows)
                {
                    foreach (String s in ls)
                    {
                        sRowsStr += s + ", ";
                    }
                    sRowsStr += Strings.EOL;
                }
            }
            sRetval += indent + sRowsStr + Strings.EOL;

            return sRetval;
        }

    }
}


