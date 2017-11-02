// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingSchema : IDefinition
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
        [JsonProperty("last_modified", Order = 18)]
        private DateTime _lastModified;
        [JsonProperty("schema_num", Order = 9)]
        private int _schemaNum;
        [JsonProperty("schema_selection_table", Order = 10)]
        private String _schemaSelectionTable;
        [JsonProperty("schema_discriminators", Order = 11)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private HashSet<String> _schemaDiscriminators;
        [JsonProperty("inputs", Order = 13)]
        [JsonConverter(typeof(CustomListConverter<StagingSchemaInput>))]
        private List<StagingSchemaInput> _inputs;
        [JsonProperty("outputs", Order = 14)]
        [JsonConverter(typeof(CustomListConverter<StagingSchemaOutput>))]
        private List<StagingSchemaOutput> _outputs;
        [JsonProperty("initial_context", Order = 12)]
        [JsonConverter(typeof(CustomHashSetConverter_StagingKeyValue_IKeyValue))]
        private HashSet<IKeyValue> _initialContext;
        [JsonProperty("mappings", Order = 15)]
        [JsonConverter(typeof(CustomListConverter_StagingMapping_IMapping))]
        private List<IMapping> _mappings;
        [JsonProperty("involved_tables", Order = 16)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private HashSet<String> _involvedTables;
        [JsonProperty("on_invalid_input", Order = 17)]
        [JsonConverter(typeof(StringEnumConverter))]
        private StagingInputErrorHandler _onInvalidInput;

        int miHashCode;

        // parsed fields
        private Dictionary<String, IInput> _parsedInputMap = new Dictionary<String, IInput>();
        private Dictionary<String, IOutput> _parsedOutputMap = new Dictionary<String, IOutput>();

        public StagingSchema()
        {
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

        public DateTime getLastModified()
        {
            return _lastModified;
        }

        public void setLastModified(DateTime lastModified)
        {
            _lastModified = lastModified;
            ComputeHashCode();
        }

        public int getSchemaNum()
        {
            return _schemaNum;
        }

        public void setSchemaNum(int schemaNum)
        {
            _schemaNum = schemaNum;
            ComputeHashCode();
        }

        public String getSchemaSelectionTable()
        {
            return _schemaSelectionTable;
        }

        public void setSchemaSelectionTable(String schemaSelectionTable)
        {
            _schemaSelectionTable = schemaSelectionTable;
            ComputeHashCode();
        }

        public HashSet<String> getSchemaDiscriminators()
        {
            return _schemaDiscriminators;
        }

        public void setSchemaDiscriminators(HashSet<String> schemaDiscriminators)
        {
            _schemaDiscriminators = schemaDiscriminators;
            ComputeHashCode();
        }

        public List<StagingSchemaInput> getInputs()
        {
            return _inputs;
        }

        public void setInputs(List<StagingSchemaInput> inputs)
        {
            _inputs = inputs;
            ComputeHashCode();
        }

        public List<StagingSchemaOutput> getOutputs()
        {
            return _outputs;
        }

        public void setOutputs(List<StagingSchemaOutput> outputs)
        {
            _outputs = outputs;
            ComputeHashCode();
        }

        public HashSet<IKeyValue> getInitialContext()
        {
            return _initialContext;
        }

        public void setInitialContext(HashSet<IKeyValue> initialContext)
        {
            _initialContext = initialContext;
            ComputeHashCode();
        }

        public List<IMapping> getMappings()
        {
            return _mappings;
        }

        public void setMappings(List<IMapping> mapping)
        {
            _mappings = mapping;
            ComputeHashCode();
        }

        public Dictionary<String, IInput> getInputMap()
        {
            return _parsedInputMap;
        }

        public void setInputMap(Dictionary<String, IInput> parsedInputMap)
        {
            _parsedInputMap = parsedInputMap;
            ComputeHashCode();
        }

        public Dictionary<String, IOutput> getOutputMap()
        {
            return _parsedOutputMap;
        }

        public void setOutputMap(Dictionary<String, IOutput> parsedOutputMap)
        {
            _parsedOutputMap = parsedOutputMap;
            ComputeHashCode();
        }

        public HashSet<String> getInvolvedTables()
        {
            return _involvedTables;
        }

        public void setInvolvedTables(HashSet<String> involvedTables)
        {
            _involvedTables = involvedTables;
            ComputeHashCode();
        }

        public StagingInputErrorHandler getOnInvalidInput()
        {
            return _onInvalidInput;
        }

        public void setOnInvalidInput(StagingInputErrorHandler onInvalidInput)
        {
            _onInvalidInput = onInvalidInput;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingSchema schema = (StagingSchema)o;

            // do not include _id, _lastModified and _parsedInputMap
            return Equals(_displayId, schema._displayId) &&
                    Equals(_algorithm, schema._algorithm) &&
                    Equals(_version, schema._version) &&
                    Equals(_name, schema._name) &&
                    Equals(_title, schema._title) &&
                    Equals(_description, schema._description) &&
                    Equals(_subtitle, schema._subtitle) &&
                    Equals(_notes, schema._notes) &&
                    Equals(_schemaNum, schema._schemaNum) &&
                    Equals(_schemaSelectionTable, schema._schemaSelectionTable) &&
                    Equals(_schemaDiscriminators, schema._schemaDiscriminators) &&
                    Equals(_inputs, schema._inputs) &&
                    Equals(_outputs, schema._outputs) &&
                    Equals(_initialContext, schema._initialContext) &&
                    Equals(_mappings, schema._mappings) &&
                    Equals(_onInvalidInput, schema._onInvalidInput) &&
                    Equals(_involvedTables, schema._involvedTables);
        }

        public override int GetHashCode()
        {
            // do not include _id, _lastModified and _parsedInputMap
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
            MyStringBuilder.Append(_schemaNum);
            MyStringBuilder.Append(_schemaSelectionTable);
            if (_schemaDiscriminators != null)
            {
                foreach (String s in _schemaDiscriminators)
                {
                    MyStringBuilder.Append(s);
                }
            }
            if (_inputs != null)
            {
                for (int i = 0; i < _inputs.Count; i++)
                {
                    MyStringBuilder.Append(_inputs[i].GetHashString());
                }
            }
            if (_outputs != null)
            {
                for (int i = 0; i < _outputs.Count; i++)
                {
                    MyStringBuilder.Append(_outputs[i].GetHashString());
                }
            }
            if (_initialContext != null)
            {
                foreach (IKeyValue ik in _initialContext)
                {
                    MyStringBuilder.Append(ik.getKey());
                    MyStringBuilder.Append(ik.getValue());
                }
            }
            if (_mappings != null)
            {
                for (int i = 0; i < _mappings.Count; i++)
                {
                    MyStringBuilder.Append(_mappings[i].GetHashString());
                }
            }
            if (_involvedTables != null)
            {
                foreach (String s in _involvedTables)
                {
                    MyStringBuilder.Append(s);
                }
            }

            miHashCode = MyStringBuilder.ToString().GetHashCode();
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
            sRetval += indent + "Notes:                 " + _notes + Strings.EOL;
            sRetval += indent + "LastModified:          " + _lastModified + Strings.EOL;
            sRetval += indent + "SchemaNum:             " + _schemaNum + Strings.EOL;
            sRetval += indent + "SchemaSelectionTable:  " + _schemaSelectionTable + Strings.EOL;
            sRetval += indent + "SchemaDiscriminators:  " + ((_schemaDiscriminators == null) ? "NULL" : _schemaDiscriminators.Count.ToString()) + Strings.EOL;
            sRetval += indent + "Inputs:                " + ((_inputs == null) ? "NULL" : _inputs.Count.ToString()) + Strings.EOL;
            sRetval += indent + "Outputs:               " + ((_outputs == null) ? "NULL" : _outputs.Count.ToString()) + Strings.EOL;
            sRetval += indent + "InitialContext:        " + ((_initialContext == null) ? "NULL" : _initialContext.Count.ToString()) + Strings.EOL;
            sRetval += indent + "Mappings:              " + ((_mappings == null) ? "NULL" : _mappings.Count.ToString()) + Strings.EOL;
            sRetval += indent + "InvolvedTables:        " + ((_involvedTables == null) ? "NULL" : _involvedTables.Count.ToString()) + Strings.EOL;
            sRetval += indent + "OnInvalidInput:        " + _onInvalidInput + Strings.EOL;
            sRetval += indent + "ParsedInputMap:        " + ((_parsedInputMap == null) ? "NULL" : _parsedInputMap.Count.ToString()) + Strings.EOL;
            sRetval += indent + "ParsedOutputMap:       " + ((_parsedOutputMap == null) ? "NULL" : _parsedOutputMap.Count.ToString()) + Strings.EOL;

            return sRetval;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }


    }
}


