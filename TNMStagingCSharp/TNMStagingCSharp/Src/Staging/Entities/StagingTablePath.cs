// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingTablePath : ITablePath
    {
        [JsonProperty("id", Order = 1)]
        private String _id;
        [JsonProperty("input_mapping", Order = 2)]
        [JsonConverter(typeof(CustomHashSetConverter_StagingKeyMapping_IKeyMapping))]
        private HashSet<IKeyMapping> _inputMapping;
        [JsonProperty("output_mapping", Order = 3)]
        [JsonConverter(typeof(CustomHashSetConverter_StagingKeyMapping_IKeyMapping))]
        private HashSet<IKeyMapping> _outputMapping;
        [JsonProperty("inputs", Order = 4)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private HashSet<String> _inputs;
        [JsonProperty("outputs", Order = 5)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private HashSet<String> _outputs;

        int miHashCode;

        public StagingTablePath()
        {
        }

        public StagingTablePath(String id)
        {
            setId(id);
            ComputeHashCode();
        }

        public String getId()
        {
            return _id;
        }

        public void setId(String id)
        {
            _id = id;
            ComputeHashCode();
        }

        public HashSet<IKeyMapping> getInputMapping()
        {
            return _inputMapping;
        }


        public void setInputMapping(HashSet<IKeyMapping> input)
        {
            _inputMapping = input;
            ComputeHashCode();
        }

        public HashSet<IKeyMapping> getOutputMapping()
        {
            return _outputMapping;
        }

        public void setOutputMapping(HashSet<IKeyMapping> output)
        {
            _outputMapping = output;
            ComputeHashCode();
        }

        public HashSet<String> getInputs()
        {
            return _inputs;
        }

        public void setInputs(HashSet<String> inputs)
        {
            _inputs = inputs;
            ComputeHashCode();
        }

        public HashSet<String> getOutputs()
        {
            return _outputs;
        }

        public void setOutputs(HashSet<String> outputs)
        {
            _outputs = outputs;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingTablePath that = (StagingTablePath)o;

            return Equals(_id, that._id) &&
                   Equals(_inputMapping, that._inputMapping) &&
                   Equals(_inputs, that._inputs) &&
                   Equals(_outputMapping, that._outputMapping) &&
                   Equals(_outputs, that._outputs);
        }

        public override int GetHashCode()
        {
            return miHashCode;
        }

        public void ComputeHashCode()
        {
            //return Objects.hash(_id, _inputMapping, _inputs, _outputMapping, _outputs);
            miHashCode = GetHashString().GetHashCode();
        }

        public String GetHashString()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_id);
            if (_inputMapping != null)
            {
                foreach (IKeyMapping ik in _inputMapping)
                {
                    MyStringBuilder.Append(ik.getFrom());
                    MyStringBuilder.Append(ik.getTo());
                }
            }
            if (_outputMapping != null)
            {
                foreach (IKeyMapping ik in _outputMapping)
                {
                    MyStringBuilder.Append(ik.getFrom());
                    MyStringBuilder.Append(ik.getTo());
                }
            }
            if (_inputs != null)
            {
                foreach (String s in _inputs)
                {
                    MyStringBuilder.Append(s);
                }
            }
            if (_outputs != null)
            {
                foreach (String s in _outputs)
                {
                    MyStringBuilder.Append(s);
                }
            }

            return MyStringBuilder.ToString();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }


        public String GetDebugString()
        {
            String sRetval = "";
            sRetval += "  ID:        " + _id + Strings.EOL;
            String input_map_str = "NULL";
            String output_map_str = "NULL";
            if (_inputMapping != null)
            {
                input_map_str = "";
                foreach (IKeyMapping ik in _inputMapping)
                {
                    input_map_str += "<" + ik.getFrom() + ", " + ik.getTo() + ">, ";
                }
            }
            if (_outputMapping != null)
            {
                output_map_str = "";
                foreach (IKeyMapping ik in _outputMapping)
                {
                    output_map_str += "<" + ik.getFrom() + ", " + ik.getTo() + ">, ";
                }
            }

            sRetval += "    Input Mapping:   " + input_map_str + Strings.EOL;
            sRetval += "    Output Mapping:  " + output_map_str + Strings.EOL;
            sRetval += "    Inputs:          " + DebugStringUtils.GetStringHashSet(_inputs) + Strings.EOL;
            sRetval += "    Outputs:         " + DebugStringUtils.GetStringHashSet(_outputs) + Strings.EOL;
            return sRetval;
        }
    }
}


