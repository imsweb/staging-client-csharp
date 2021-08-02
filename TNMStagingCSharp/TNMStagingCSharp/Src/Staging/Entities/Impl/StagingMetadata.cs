/*
 * Copyright (C) 2021 Information Management Services, Inc.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace TNMStagingCSharp.Src.Staging.Entities.Impl
{
    [JsonConverter(typeof(StagingMetadataDeserializer))]
    public class StagingMetadata : Metadata
    {
        [JsonProperty("name", Order = 1)]
        private String _name;
        [JsonProperty("start", Order = 2)]
        private int _start;
        [JsonProperty("end", Order = 3)]
        private int _end;

        int miHashCode;

        public StagingMetadata()
        {
        }

        public StagingMetadata(String name)
        {
            _name = name;
        }

        public StagingMetadata(String name, int start)
        {
            _name = name;
            _start = start;
        }

        public StagingMetadata(String name, int start, int end)
        {
            _name = name;
            _start = start;
            _end = end;
        }

        public String getName()
        {
            return _name;
        }

        public void setName(String name)
        {
            _name = name;
        }

        public int getStart()
        {
            return _start;
        }

        public void setStart(int start)
        {
            _start = start;
        }

        public int getEnd()
        {
            return _end;
        }

        public void setEnd(int end)
        {
            _end = end;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingMetadata that = (StagingMetadata)o;

            return Equals(_name, that._name) &&
                   Equals(_start, that._start) &&
                   Equals(_end, that._end);
        }

        public override int GetHashCode()
        {
            return miHashCode;
        }

        public void ComputeHashCode()
        {
            miHashCode = GetHashString().GetHashCode();
        }

        public String GetHashString()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");

            MyStringBuilder.Append(_name);
            MyStringBuilder.Append(_start);
            MyStringBuilder.Append(_end);

            return MyStringBuilder.ToString();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }
    }
}
