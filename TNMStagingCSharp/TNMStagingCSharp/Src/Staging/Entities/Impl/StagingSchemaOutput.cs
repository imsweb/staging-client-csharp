// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.Tools;

namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    public class StagingSchemaOutput : IOutput
    {
        [JsonProperty("key", Order = 1)]
        private String _key;
        [JsonProperty("name", Order = 2)]
        private String _name;
        [JsonProperty("description", Order = 3)]
        private String _description;
        [JsonProperty("naaccr_item", Order = 4)]
        private int _naaccrItem;
        [JsonProperty("naaccr_xml_id", Order = 5)]
        private String _naaccrXmlId;
        [JsonProperty("table", Order = 6)]
        private String _table;
        [JsonProperty("default", Order = 7)]
        private String _default;
        [JsonProperty("metadata", Order = 8)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private List<StagingMetadata> _metadata;

        int miHashCode;

        public StagingSchemaOutput()
        {
        }

        public StagingSchemaOutput(String key)
        {
            setKey(key);
        }

        public StagingSchemaOutput(String key, String name)
        {
            setKey(key);
            setName(name);
            ComputeHashCode();
        }

        public StagingSchemaOutput(String key, String name, String table)
        {
            setKey(key);
            setName(name);
            setTable(table);
            ComputeHashCode();
        }

        // Copy constructor
        // @param other other StagingSchemaInput
        public StagingSchemaOutput(StagingSchemaOutput other)
        {
            setKey(other.getKey());
            setName(other.getName());
            setDescription(other.getDescription());
            setNaaccrItem(other.getNaaccrItem());
            setNaaccrXmlId(other.getNaaccrXmlId());
            setTable(other.getTable());
            setDefault(other.getDefault());
            if (other.getMetadata() != null)
                setMetadata(new List<StagingMetadata>(other._metadata));
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

        public String getDescription()
        {
            return _description;
        }

        public void setDescription(String description)
        {
            _description = description;
            ComputeHashCode();
        }

        public int getNaaccrItem()
        {
            return _naaccrItem;
        }

        public void setNaaccrItem(int naaccrItem)
        {
            _naaccrItem = naaccrItem;
            ComputeHashCode();
        }

        public String getNaaccrXmlId()
        {
            return _naaccrXmlId;
        }

        public void setNaaccrXmlId(String naaccrXmlId)
        {
            _naaccrXmlId = naaccrXmlId;
            ComputeHashCode();
        }

        public String getTable()
        {
            return _table;
        }

        public void setTable(String table)
        {
            _table = table;
            ComputeHashCode();
        }

        public String getDefault()
        {
            return _default;
        }

        public void setDefault(String aDefault)
        {
            _default = aDefault;
            ComputeHashCode();
        }

        public List<Metadata> getMetadata()
        {
            return new List<Metadata>(_metadata);
        }

        public void setMetadata(List<StagingMetadata> metadata)
        {
            _metadata = metadata;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingSchemaOutput that = (StagingSchemaOutput)o;

            // do not include _parsedValues
            return Equals(_key, that._key) &&
                   Equals(_name, that._name) &&
                   Equals(_description, that._description) &&
                   Equals(_naaccrItem, that._naaccrItem) &&
                   Equals(_naaccrXmlId, that._naaccrXmlId) &&
                   Equals(_table, that._table) &&
                   Equals(_default, that._default) &&
                   _metadata.SequenceEqual(that._metadata);
        }

        public override int GetHashCode()
        {
            // do not include _parsedValues
            return miHashCode;
        }

        private void ComputeHashCode()
        {
            miHashCode = GetHashString().GetHashCode();
        }

        public String GetHashString()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_key);
            MyStringBuilder.Append(_name);
            MyStringBuilder.Append(_description);
            MyStringBuilder.Append(_naaccrItem);
            MyStringBuilder.Append(_naaccrXmlId);
            MyStringBuilder.Append(_table);
            MyStringBuilder.Append(_default);
            foreach (StagingMetadata m in _metadata)
            {
                MyStringBuilder.Append(m.GetHashString());
            }
            return MyStringBuilder.ToString();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }


    }
}

