// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.DecisionEngine;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingKeyMapping : IKeyMapping
    {
        [JsonProperty("from", Order = 1)]
        String _from;
        [JsonProperty("to", Order = 2)]
        String _to;

        int miHashCode;

        public StagingKeyMapping()
        {
        }

        public StagingKeyMapping(String from, String to)
        {
            _from = from;
            _to = to;
            ComputeHashCode();
        }

        public String getFrom()
        {
            return _from;
        }

        public void setFrom(String from)
        {
            _from = from;
            ComputeHashCode();
        }

        public String getTo()
        {
            return _to;
        }

        public void setTo(String to)
        {
            _to = to;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingKeyMapping that = (StagingKeyMapping)o;

            return Equals(_from, that._from) && Equals(_to, that._to);
        }

        public override int GetHashCode()
        {
            return miHashCode;
        }

        private void ComputeHashCode()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_from);
            MyStringBuilder.Append(_to);
            miHashCode = MyStringBuilder.ToString().GetHashCode();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }
    }
}


