// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;


namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    public class StagingKeyValue : IKeyValue
    {
        [JsonProperty("key", Order = 1)]
        private String _key;
        [JsonProperty("value", Order = 2)]
        private String _value;

        int miHashCode;

        public StagingKeyValue()
        {
        }

        public StagingKeyValue(String key, String value)
        {
            _key = key;
            _value = value;
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

        public String getValue()
        {
            return _value;
        }

        public void setValue(String value)
        {
            _value = value;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingKeyValue that = (StagingKeyValue)o;

            return Equals(_key, that._key) && Equals(_value, that._value);

        }

        public override int GetHashCode()
        {
            return miHashCode;
        }

        private void ComputeHashCode()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_key);
            MyStringBuilder.Append(_value);
            miHashCode = MyStringBuilder.ToString().GetHashCode();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }

    }
}


