// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.DecisionEngine;
using TNMStagingCSharp.Src.Tools;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingSchemaInput : IInput
    {
        [JsonProperty("key", Order = 1)]
        private String _key;
        [JsonProperty("name", Order = 2)]
        private String _name;
        [JsonProperty("description", Order = 3)]
        private String _description;
        [JsonProperty("naaccr_item", Order = 4)]
        private int _naaccrItem;
        [JsonProperty("default", Order = 6)]
        private String _default;
        [JsonProperty("table", Order = 7)]
        private String _table;
        [JsonProperty("used_for_staging", Order = 8)]
        private bool _usedForStaging;
        [JsonProperty("unit", Order = 10)]
        private String _unit;
        [JsonProperty("decimal_places", Order = 11)]
        private int _decimalPlaces;
        [JsonProperty("metadata", Order = 12)]
        [JsonConverter(typeof(CustomHashSetConverter<String>))]
        private HashSet<String> _metadata;


        int miHashCode;


        public StagingSchemaInput()
        {
        }

        public StagingSchemaInput(String key, String name)
        {
            setKey(key);
            setName(name);
            ComputeHashCode();
        }

        public StagingSchemaInput(String key, String name, String table)
        {
            setKey(key);
            setName(name);
            setTable(table);
            ComputeHashCode();
        }

        // Copy constructor
        // @param other other StagingSchemaInput
        public StagingSchemaInput(StagingSchemaInput other)
        {
            setKey(other.getKey());
            setName(other.getName());
            setDescription(other.getDescription());
            setNaaccrItem(other.getNaaccrItem());
            setDefault(other.getDefault());
            setTable(other.getTable());
            if (other.getMetadata() != null)
                setMetadata(new HashSet<String>(other.getMetadata()));
            setUsedForStaging(other.getUsedForStaging());
            setUnit(other.getUnit());
            setDecimalPlaces(other.getDecimalPlaces());
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

        public String getDefault()
        {
            return _default;
        }

        public void setDefault(String aDefault)
        {
            _default = aDefault;
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

        public bool getUsedForStaging()
        {
            return _usedForStaging;
        }

        public void setUsedForStaging(bool usedForStaging)
        {
            _usedForStaging = usedForStaging;
            ComputeHashCode();
        }

        public int getDecimalPlaces()
        {
            return _decimalPlaces;
        }

        public void setDecimalPlaces(int decimalPlaces)
        {
            _decimalPlaces = decimalPlaces;
            ComputeHashCode();
        }

        public String getUnit()
        {
            return _unit;
        }

        public void setUnit(String unit)
        {
            _unit = unit;
            ComputeHashCode();
        }

        public HashSet<String> getMetadata()
        {
            return _metadata;
        }

        public void setMetadata(HashSet<String> metadata)
        {
            _metadata = metadata;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingSchemaInput that = (StagingSchemaInput)o;

            bool bRetval = true;

            // do not include _parsedValues
            bRetval = (_key == that._key) &&
                      (_name == that._name) &&
                      (_description == that._description) &&
                      (_naaccrItem == that._naaccrItem) &&
                      (_default == that._default) &&
                      (_table == that._table) &&
                      (_usedForStaging == that._usedForStaging) &&
                      (_unit == that._unit) &&
                      (_decimalPlaces == that._decimalPlaces);

            if (bRetval)
            {
                if ((_metadata == null) && (that._metadata == null))
                {
                    bRetval = true;
                }
                else if (((_metadata != null) && (that._metadata == null)) ||
                         ((_metadata == null) && (that._metadata != null)))
                {
                    bRetval = false;
                }
                else if ((_metadata != null) && (that._metadata != null))
                {
                    bRetval = (_metadata.SetEquals(that._metadata));
                }
            }

            return bRetval;
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
            MyStringBuilder.Append(_default);
            MyStringBuilder.Append(_table);
            MyStringBuilder.Append(_usedForStaging);
            MyStringBuilder.Append(_unit);
            MyStringBuilder.Append(_decimalPlaces);
            if (_metadata != null)
            {
                foreach (String s in _metadata)
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


    }
}


