// Copyright (C) 2017 Information Management Services, Inc.

using System;
using Newtonsoft.Json;


namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    public class StagingEndpoint : IEndpoint
    {
        [JsonProperty("type", Order = 1)]
        private EndpointType _type;
        [JsonProperty("value", Order = 2)]
        private String _value;
        [JsonProperty("result_key", Order = 3)]
        private String _resultKey;

        public StagingEndpoint()
        {
        }

        public StagingEndpoint(EndpointType type, String value)
        {
            _type = type;
            _value = value;
        }

        public EndpointType getType()
        {
            return _type;
        }

        public void setType(EndpointType type)
        {
            _type = type;
        }

        public String getValue()
        {
            return _value;
        }

        public void setValue(String value)
        {
            _value = value;
        }

        public String getResultKey()
        {
            return _resultKey;
        }

        public void setResultKey(String resultKey)
        {
            _resultKey = resultKey;
        }

        public String GetDebugString(String indent)
        {
            String sRetval = "";
            sRetval += indent + "(" + getType() + ", ";
            sRetval += (getValue() == null ? "NULL" : getValue()) + ", ";
            sRetval += (getResultKey() == null ? "NULL" : getResultKey()) + ")";
            return sRetval;
        }

    }
}

