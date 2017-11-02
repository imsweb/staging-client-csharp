// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingColumnDefinition : IColumnDefinition
    {

        [JsonProperty("key", Order = 1)]
        private String _key;
        [JsonProperty("name", Order = 2)]
        private String _name;
        [JsonProperty("type", Order = 3)]
        [JsonConverter(typeof(StringEnumConverter))]
        private ColumnType _type;
        [JsonProperty("source", Order = 4)]
        private String _source;

        int miHashCode;

        public StagingColumnDefinition()
        {
        }

        // Constructor
        // @param key input key
        // @param name column name
        // @param type column type
        public StagingColumnDefinition(String key, String name, ColumnType type)
        {
            setKey(key);
            setName(name);
            setType(type);
            ComputeHashCode();
        }

        public String getKey()
        {
            return _key;
        }

        public void setKey(String key)
        {
            _key = key;
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

        public ColumnType getType()
        {
            return _type;
        }

        public void setType(ColumnType type)
        {
            _type = type;
            ComputeHashCode();
        }

        public String getSource()
        {
            return _source;
        }

        public void setSource(String source)
        {
            _source = source;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingColumnDefinition that = (StagingColumnDefinition)o;

            return (_key == that._key) && (_name == that._name) && (_type == that._type) && (_source == that._source);
        }

        public override int GetHashCode()
        {
            //return Objects.hash(_key, _name, _type, _source);
            return miHashCode;
        }
        private void ComputeHashCode()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_key);
            MyStringBuilder.Append(_name);
            MyStringBuilder.Append(_type);
            MyStringBuilder.Append(_source);
            miHashCode = MyStringBuilder.ToString().GetHashCode();
        }

        public String GetDebugString(String indent)
        {
            String sRetval = "(Key:" + _key + ", Name:" + _name + ", Type:" + _type + ", Source: " + _source + ")";
            return sRetval;
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }

    }
}

